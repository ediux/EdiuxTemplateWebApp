using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models
{
    public interface IStoreBase<T,in TKey> where T : class
    {
        bool IsExisted(Expression<Func<T, bool>> filiter);

        Task<bool> IsExistedAsync(Expression<Func<T, bool>> filiter);

        IEnumerable<T> FindDataEntitiesByQuery(TKey key);

        void Add(T entity);

        Task AddAsync(T entity);

        T GetEntityByQuery(TKey key);

        IEnumerable<TResult> FindEntitiesByQuery<TResult>(TKey key, Expression<Func<T, TResult>> selector = null);

        bool Update(T entity);

        bool Update(IEnumerable<T> entities);

        bool Delete(T entity);

        bool Delete(IEnumerable<T> entities);

        void Initialization();

        Task InitializationAsync();

        Task<T> GetEntityByQueryAsync(TKey key);

        Task<IEnumerable<TResult>> FindEntitiesByQueryAsync<TResult>(TKey key, Expression<Func<T, TResult>> selector = null);

        Task<bool> UpdateAsync(T entity);

        Task<bool> UpdateAsync(IEnumerable<T> entities);

        Task<bool> DeleteAsync(T entity);

        Task<bool> DeleteAsync(IEnumerable<T> entities);
    }
}
