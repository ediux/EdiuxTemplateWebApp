using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.Shared
{
    public interface IStoreBase
    {
        bool IsExisted<T>(Expression<Func<T, bool>> filiter) where T : class;

        Task<bool> IsExistedAsync<T>(Expression<Func<T, bool>> filiter) where T : class;

        T CreateDataEntityInstance<T>() where T : class;

        Task<T> CreateDataEntityInstanceAsync<T>() where T : class;

        T GetDataEntityInstanceByQuery<T, TKey>(TKey key) where T : class;

        Task<T> GetDataEntityInstanceByQueryAsync<T, TKey>(TKey key) where T : class;

        IEnumerable<T> FindDataEntitiesByQuery<T, TKey>(TKey key) where T : class;

        void AddToStore<T>(T entity) where T : class;

        Task AddToStoreAsync<T>(T entity) where T : class;


    }
}
