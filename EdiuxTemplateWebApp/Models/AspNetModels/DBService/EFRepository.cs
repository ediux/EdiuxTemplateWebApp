using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
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
            UnitOfWork = RepositoryHelper.GetUnitOfWork();
        }

        IDbSet<T> _objectset;

        protected IDbSet<T> ObjectSet
        {
            get
            {
                if (_objectset == null)
                {
                    _objectset = UnitOfWork.Set<T>();
                }

                return _objectset;
            }
        }

        public IUnitOfWork UnitOfWork
        {
            get;
            set;
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
            var addEntity = ObjectSet.Add(entity);

            if (!UnitOfWork.TranscationMode)
            {
                UnitOfWork.Commit();
                addEntity = Reload(addEntity);
            }

            return addEntity;
        }

        public virtual void Delete(T entity)
        {
            ObjectSet.Remove(entity);

            if (!UnitOfWork.TranscationMode)
            {
                UnitOfWork.Commit();
            }
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
            var pkeys = IdentifyPrimaryKey(entity);
            return Get(pkeys);
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

            var props = entity.GetProperties();

            if (props.Count > 0)
            {
                Type _targetType = typeof(T);
                var returnValue = Activator.CreateInstance<T>();

                foreach (string key in props.Keys)
                {
                    var prop = props[key];
                    if (prop == null)
                    {
                        continue;
                    }
                    var value = prop.GetValue(entity);
                    var targetProp = _targetType.GetProperty(key);

                    if (targetProp != null)
                    {
                        targetProp.SetValue(returnValue, value);
                    }

                }
                return returnValue;
            }

            return Activator.CreateInstance<T>();
        }

        public R CopyTo<R>(T entity)
        {
            if (entity == null)
                return default(R);
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

        protected object[] IdentifyPrimaryKey(T entity)
        {

            ObjectContext objectContext = UnitOfWork.Context.ObjectContext;
            ObjectSet<T> set = objectContext.CreateObjectSet<T>();
            IEnumerable<string> keyNames = set.EntitySet.ElementType
                                                        .KeyMembers
                                                        .Select(k => k.Name);

            Type entityreflection = typeof(T);

            var pkeys = entityreflection.GetProperties()
                .Join(keyNames, (x) => x.Name, (y) => y, (k, t) => k)
                .Select(s => s.GetValue(entity));

            return pkeys.ToArray();
        }

        //      /// <summary>
        ///// Update the specified aspnet_Membership.
        ///// </summary>
        ///// <returns>The update.</returns>
        ///// <param name="entity">Aspnet membership.</param>

        /// <summary>
        /// Update the specified <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">要更新的資料實體。</param>
        public virtual void Update(T entity)
        {
            var source = Get(IdentifyPrimaryKey(entity));
            var sourceprops = source.GetProperties();
            var targetprops = entity.GetProperties();

            foreach(var targetprop in targetprops)
            {
                if (sourceprops.ContainsKey(targetprop.Key))
                {
                    var sourcevalue = sourceprops[targetprop.Key].GetValue(source);
                    var targetvalue = targetprop.Value.GetValue(entity);

                    if (!sourcevalue.Equals(targetvalue))
                    {
                        sourcevalue = targetvalue;
                        sourceprops[targetprop.Key].SetValue(source, sourcevalue);
                    }
                }
            }

            if (!UnitOfWork.TranscationMode)
            {
                UnitOfWork.Commit();
            }
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

        /// <summary>
        /// 執行Join查詢
        /// </summary>
        /// <typeparam name="TOuterSet"></typeparam>
        /// <typeparam name="TJoinResult"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="OuterSet"></param>
        /// <param name="leftKeySelector"></param>
        /// <param name="rightKeySelector"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public IQueryable<TJoinResult> Join<TOuterSet, TJoinResult, TKey>(IEnumerable<TOuterSet> OuterSet, Expression<Func<T, TKey>> leftKeySelector, Expression<Func<TOuterSet, TKey>> rightKeySelector, Expression<Func<T, TOuterSet, TJoinResult>> result)
        {
            return All().Join(OuterSet, leftKeySelector, rightKeySelector, result);
        }


        #endregion
    }
}
