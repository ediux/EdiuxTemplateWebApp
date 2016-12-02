using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models
{
    public partial class EFRepository<T> : IRepositoryBase<T> where T : class
    {
        public IUnitOfWork UnitOfWork { get; set; }

        private IDbSet<T> _objectset;
        protected IDbSet<T> ObjectSet
        {
            get
            {
                if (_objectset == null)
                {
                    _objectset = UnitOfWork.Context.Set<T>();
                }
                return _objectset;
            }
        }

        protected int CacheExpiredTime
        {
            get
            {
                int _time = 0;
                if (int.TryParse(System.Web.Configuration.WebConfigurationManager.AppSettings["CacheExpiredTime"], out _time) == false)
                {
                    _time = 30;
                }
                return _time;
            }
        }

        public virtual IQueryable<T> All()
        {
            try
            {
                //取得記憶體快取中的資料
                //只傳回未被標記為刪除的資料集合
                return GetCache();
            }
            catch (Exception ex)
            {
#if !DEBUG
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
#endif
                throw ex;
            }
        }

        public virtual IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            try
            {
                //記憶體快取先行的讀取
                IQueryable<T> cacheResult = GetCache().Where(expression);

                IQueryable<T> queryResultFromDatabase = ObjectSet.Where(expression);

                if (queryResultFromDatabase != null
                    && queryResultFromDatabase.Count() > 0)
                {
                    //如果資料庫有資料的話
                    var diffFromDatabase = queryResultFromDatabase.Except(cacheResult);

                    if (diffFromDatabase != null
                        && diffFromDatabase.Count() > 0)
                    {
                        foreach (var item in diffFromDatabase)
                        {
                            AddToCache(item);
                        }
                    }
                }
                cacheResult = cacheResult.Union(queryResultFromDatabase);
                //IQueryable<ApplicationUser> mergedSet = cacheResult.Union(base.Where(expression));  //
                //IQueryable<ApplicationUser> addtoCacheResult = mergedSet.Except(GetCache());
                //List<ApplicationUser> newcacheResult = GetFromCache();
                //newcacheResult.AddRange(addtoCacheResult.ToList());
                //return mergedSet;
                return cacheResult;
            }
            catch (Exception ex)
            {
#if !DEBUG
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
#endif
                throw ex;
            }
            //return ObjectSet.Where(expression);
        }

        public virtual T Add(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));  //C# 6.0 新語法

                //先取得目前快取
                entity = ObjectSet.Add(entity);
                AddToCache(entity);
                return entity;
            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#else
                System.Diagnostics.Debug.WriteLine(ex.Message);
#endif

                throw ex;
            }
            //return ObjectSet.Add(entity);
        }

        public virtual void Delete(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));  //C# 6.0 新語法

                ObjectSet.Remove(entity);
                FromCache.Remove(entity);

            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                throw ex;
            }

        }

        public virtual Task<IQueryable<T>> AllAsync()
        {
            return Task.FromResult(GetCache());
        }

        public virtual IList<T> BatchAdd(IEnumerable<T> entities)
        {
            if (entities.Count() > 0)
            {
                foreach (var item in entities)
                {
                    AddToCache(item);
                }
            }
            return ((DbSet<T>)ObjectSet).AddRange(entities).ToList();
        }

        public virtual T Get(params object[] values)
        {
            T entity = ObjectSet.Find(values);
            AddToCache(entity);
            return entity;
        }

        public virtual Task<T> GetAsync(params object[] values)
        {
            return Task.FromResult(Get(values));
        }

        public virtual T Reload(T entity)
        {
            UnitOfWork.Context.Entry(entity).Reload();
            UpdateCache(entity);
            return entity;
        }

        public virtual async Task<T> ReloadAsync(T entity)
        {
            await UnitOfWork.Context.Entry(entity).ReloadAsync();
            UpdateCache(entity);
            return entity;
        }

        #region Memory Cache Supports

        public virtual void ClearCache(string key)
        {
            UnitOfWork.Invalidate(key);
        }

        public virtual IQueryable<T> GetCache()
        {
            try
            {
                string KeyName = nameof(T);

                if (UnitOfWork.IsSet(KeyName) == false)
                {
                    IQueryable<T> asyncResult = ObjectSet.AsQueryable();

                    System.Collections.ObjectModel.ObservableCollection<T> _cacheList = ObjectSet.Local;

                    UnitOfWork.Set(KeyName, _cacheList, CacheExpiredTime); //快取保留30分鐘
                    return asyncResult;
                }
                else
                {
                    System.Collections.ObjectModel.ObservableCollection<T> _cache =
                        UnitOfWork.Get(KeyName) as System.Collections.ObjectModel.ObservableCollection<T>;
                    return _cache.AsQueryable();
                }
            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                throw ex;
            }
        }

        protected void RemoveFromCache(T entity)
        {
            try
            {
                if (FromCache.Contains(entity))
                {
                    List<T> cacheSet = FromCache;
                    cacheSet.Remove(entity);
                    UnitOfWork.Set(nameof(T), cacheSet, CacheExpiredTime);
                }
            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                throw ex;
            }
        }

        protected void AddToCache(T entity)
        {
            try
            {
                //加入到記憶體快取
                List<T> memoryUsersCache = FromCache;
                if (memoryUsersCache.Contains(entity))   //確認快取有該筆資料
                {
                    UpdateCache(entity);
                }
                else
                {
                    memoryUsersCache.Add(entity);
                }
                UnitOfWork.Set(nameof(T), memoryUsersCache, CacheExpiredTime);
            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                throw ex;
            }
        }

        protected void UpdateCache(T updated)
        {
            try
            {
                //加入到記憶體快取
                List<T> memoryUsersCache = FromCache;
                if (memoryUsersCache.Contains(updated))   //確認快取有該筆資料
                {
                    RemoveFromCache(updated);    //移除快取資料
                }
                memoryUsersCache.Add(updated);
                UnitOfWork.Set(nameof(T), memoryUsersCache, CacheExpiredTime);
            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                throw ex;
            }
        }

        protected List<T> FromCache
        {
            get
            {
                try
                {
                    List<T> _cache = GetCache().ToList();
                    return _cache;
                }
                catch (Exception ex)
                {
#if !TEST
                    Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                    throw ex;
                }

            }
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    UnitOfWork.Commit();
                    UnitOfWork.Context.Dispose();
                }

                // TODO: 釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下方的完成項。
                // TODO: 將大型欄位設為 null。

                disposedValue = true;
            }
        }

        // TODO: 僅當上方的 Dispose(bool disposing) 具有會釋放 Unmanaged 資源的程式碼時，才覆寫完成項。
        // ~EFRepository() {
        //   // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 加入這個程式碼的目的在正確實作可處置的模式。
        public void Dispose()
        {
            // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果上方的完成項已被覆寫，即取消下行的註解狀態。
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}