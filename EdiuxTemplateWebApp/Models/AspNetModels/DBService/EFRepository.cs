using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace EdiuxTemplateWebApp.Models.AspNetModels
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
                return Task.Run(() => ObjectSet.AsQueryable());
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
                return Task.Run(() => ObjectSet.Find(values));
            }
    
            public virtual T Reload(T entity)
            {
                UnitOfWork.Context.Entry(entity).Reload();
                return entity;
            }
    
            public virtual async Task<T> ReloadAsync(T entity)
            {
               await UnitOfWork.Context.Entry(entity).ReloadAsync();
                return entity;
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
