using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;


namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class EFRepository<T> : IRepositoryBase<T> where T : class
    {
        public EFRepository()
        {
            _unitofwork = RepositoryHelper.GetUnitOfWork();
            _depencyRepos = new Dictionary<string, IDepencyRepositoryBase>();
        }



        private IDbSet<T> _objectset;
        protected IDbSet<T> ObjectSet
        {
            get
            {
                if (_objectset == null)
                {
                    _objectset = UnitOfWork.Context.ObjectContext.CreateObjectSet<T>() as IDbSet<T>;
                }
                return _objectset;
            }
        }

        protected Dictionary<string, IDepencyRepositoryBase> _depencyRepos;

        public IDictionary<string, IDepencyRepositoryBase> DependcyRepository
        {
            get
            {
                return _depencyRepos;
            }
        }

        private IUnitOfWork _unitofwork;
        public IUnitOfWork UnitOfWork
        {
            get { return _unitofwork; }

            set { _unitofwork = value; }
        }

        public virtual IQueryable<T> All()
        {
            return ObjectSet.AsQueryable();
        }

        public virtual IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return ObjectSet.Where(expression);
        }

        public virtual T Add(T entity)
        {
            return ObjectSet.Add(entity);
        }

        public virtual void Delete(T entity)
        {
            ObjectSet.Remove(entity);
        }

        public virtual Task<IQueryable<T>> AllAsync()
        {
            return Task.FromResult(All());
        }

        public virtual IList<T> BatchAdd(IEnumerable<T> entities)
        {
            return ((DbSet<T>)ObjectSet).AddRange(entities).ToList();
        }

        public virtual T Get(params object[] values)
        {
            return ObjectSet.Find(values);
        }

        public virtual Task<T> GetAsync(params object[] values)
        {
            return Task.FromResult(Get(values));
        }

        public virtual T Reload(T entity)
        {
            UnitOfWork.Entry(entity).Reload();
            return entity;
        }

        public virtual async Task<T> ReloadAsync(T entity)
        {
            await UnitOfWork.Entry(entity).ReloadAsync();
            return entity;
        }

        public IQueryable<JR> Join<JT, TKey, JR>(IRepositoryBase<JT> joinFromRepository, Expression<Func<T, TKey>> keyselect, Expression<Func<JT, TKey>> OuterKey, Expression<Func<T, JT, JR>> ResultSelector) where JT : class
        {
            return ObjectSet.Join(joinFromRepository.All().AsEnumerable(), keyselect, OuterKey, ResultSelector);
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

        public void RegisterDependcyRepository(IDepencyRepositoryBase repository)
        {
            Type _type = repository.GetType();
            if (!_depencyRepos.ContainsKey(_type.Name))
            {
                _depencyRepos.Add(_type.Name, repository);
            }
        }

        public void UnRegisterDepencyRepository(Type repositoryType)
        {
            Type _type = repositoryType;
            if (_depencyRepos.ContainsKey(_type.Name))
            {
                _depencyRepos.Remove(_type.Name);
            }
        }

        public void Attach(T entity)
        {
            ObjectSet.Attach(entity);
        }

        public virtual T ConvertFrom<TResult>(TResult entity)
        {
            Type _type = typeof(TResult);

            if (_type.FindInterfaces(new TypeFilter((x, y) => (Type)y == x), typeof(IConvertible)).Any())
            {
                return (T)Convert.ChangeType(entity, typeof(T));
            }

            Dictionary<string, PropertyInfo> props = entity.GetProperties();

            if (props.Count > 0)
            {
                Type _targetType = typeof(T);
                T returnValue = Activator.CreateInstance<T>();

                foreach (string key in props.Keys)
                {
                    var value = props[key].GetValue(entity);
                    var targetProp = _targetType.GetProperty(key);

                    if (targetProp != null)
                    {
                        targetProp.SetValue(returnValue, targetProp);
                    }

                }
                return returnValue;
            }

            return Activator.CreateInstance<T>();
        }

        public R CopyTo<R>(T entity)
        {
            Dictionary<string, PropertyInfo> props = entity.GetProperties();
            if (props.Count > 0)
            {
                Type _targetType = typeof(R);
                R targetEntity = Activator.CreateInstance<R>();

                foreach (var key in props.Keys)
                {
                    var targetprop = _targetType.GetProperty(key);
                    if (targetprop == null)
                        continue;
                    targetprop.SetValue(targetEntity, props[key].GetValue(entity));
                }
                return targetEntity;
            }
            return default(R);


        }

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    UnitOfWork.Commit();
             
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
