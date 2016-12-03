using Microsoft.AspNet.Identity;
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
                if (ObjectSet.Local.Count > 0)
                    return ObjectSet.Local.AsQueryable();

                ObjectSet.Load();            
                return ObjectSet.Local.AsQueryable();
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public virtual IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            try
            {
                //使用EF的快取
                IQueryable<T> getFromL1Cache = ObjectSet.Local.AsQueryable().Where(expression);

                //Local快取(L1)有結果          
                if (getFromL1Cache != null && getFromL1Cache.Count() > 0)
                {                    
                    return getFromL1Cache;
                }

                getFromL1Cache.Load();  //對資料庫進行查詢並將結果快取到記憶體內 DB->L1->L2      
                    
                getFromL1Cache = ObjectSet.Local.AsQueryable().Where(expression);

                return getFromL1Cache;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
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
                entity = ObjectSet.Add(entity); //=> add to L1               
                return entity;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
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

            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }

        }

        public virtual Task<IQueryable<T>> AllAsync()
        {
            try
            {
                return Task.FromResult(All());
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }

        }

        public virtual IList<T> BatchAdd(IEnumerable<T> entities)
        {
            try
            {
                return ((DbSet<T>)ObjectSet).AddRange(entities).ToList();
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }

        }

        public virtual T Get(params object[] values)
        {
            try
            {
                return ObjectSet.Find(values);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }

        }

        public virtual Task<T> GetAsync(params object[] values)
        {
            try
            {
                return Task.FromResult(Get(values)); 
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public virtual T Reload(T entity)
        {
            try
            {                
                UnitOfWork.Context.Entry(entity).Reload();
                return entity;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }

        }

        public virtual Task<T> ReloadAsync(T entity)
        {
            try
            {
                return Task.FromResult(Reload(entity));
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }

        }

        #region Local Cache Supports
        private Func<T, object> pkeySetting;
        protected void SetPrimaryKey<TPKey>(Func<T, object> primaryKeys)
        {
            pkeySetting = primaryKeys;
        }

        public virtual void ClearCache(string key)
        {
            try
            {
                UnitOfWork.Invalidate(key); //移除快取
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
        }

        public virtual System.Collections.ObjectModel.ObservableCollection<T> GetCache()
        {
            try
            {

                string KeyName = nameof(T);

                if (UnitOfWork.IsSet(KeyName) == false)
                {
                    //IQueryable<T> asyncResult = ObjectSet.AsQueryable();
                    ObjectSet.Load();//<--直接載入記憶體副本 Db=>L1
                    System.Collections.ObjectModel.ObservableCollection<T> _cache = new System.Collections.ObjectModel.ObservableCollection<T>(ObjectSet.Local.AsEnumerable()); //L1 Save=> RAM
                    UnitOfWork.Set(KeyName, _cache, CacheExpiredTime); //快取保留30分鐘
                    return _cache;
                }
                else
                {
                    System.Collections.ObjectModel.ObservableCollection<T> _cache =
                        UnitOfWork.Get(KeyName) as System.Collections.ObjectModel.ObservableCollection<T>;
                    return _cache;
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
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
                WriteErrorLog(ex);
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
                WriteErrorLog(ex);
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
                WriteErrorLog(ex);
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
                    WriteErrorLog(ex);
                    throw ex;
                }

            }
        }

        protected virtual void WriteErrorLog(Exception ex)
        {
            if (System.Web.HttpContext.Current == null)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
            else
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
        #endregion

        #region Helper Function
        protected virtual int getCurrentLoginedUserId()
        {
            try
            {
                if (System.Web.HttpContext.Current != null)
                {
                    return System.Web.HttpContext.Current.User.Identity.GetUserId<int>();
                }                

                return 0;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
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