using EdiuxTemplateWebApp.Models.AspNetModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace EdiuxTemplateWebApp.Models
{
    public class StoreBase<T, TKey> : IStoreBase<T, TKey>
        where T : class
    {
        IRepositoryBase<T> repository;

        public StoreBase()
        {
            repository = GetRepository();
        }

        protected virtual IRepositoryBase<T> GetRepository()
        {
            Type repoType = typeof(T);
            string repositoryFunctionName = string.Format("Get{0}Repository", repoType.Name);
            Type repoHelper = typeof(RepositoryHelper);
            var callfunc = repoHelper.GetMethod(repositoryFunctionName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            return (IRepositoryBase<T>)callfunc.Invoke(null, new object[] { });
        }

        public virtual void Add(T entity)
        {
            repository.Add(entity);
            repository.UnitOfWork.Commit();
        }

        public Task AddAsync(T entity)
        {
            try
            {
                Add(entity);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }

        public virtual T CreateDataEntityInstance()
        {
            Type createEntityFuncType = typeof(T);
            var func = createEntityFuncType.GetMethod("Create", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            return (T)func.Invoke(null, new object[] { });
        }

        public Task<T> CreateDataEntityInstanceAsync()
        {
            try
            {
                return Task.FromResult(CreateDataEntityInstance());
            }
            catch (Exception ex)
            {
                return Task.FromException<T>(ex);
            }
        }

        public bool Delete(T entity)
        {
            repository.Delete(entity);
            repository.UnitOfWork.Commit();

            return (repository.Get(IdentifyPrimaryKey(entity)) == null);
        }

        protected object[] IdentifyPrimaryKey(T entity)
        {

            ObjectContext objectContext = repository.UnitOfWork.Context.ObjectContext;
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

        public bool Delete(IEnumerable<T> entities)
        {
            repository.UnitOfWork.TranscationMode = true;

            bool isbreak = false;
            foreach (var removeitem in entities)
            {
                try
                {
                    repository.Delete(removeitem);
                }
                catch
                {
                    isbreak = true;                   
                    break;
                }

            }

            if (isbreak)
            {
                return false;
            }

            repository.UnitOfWork.TranscationMode = false;
            repository.UnitOfWork.Commit();

            return true;
        }

        public Task<bool> DeleteAsync(T entity)
        {
            try
            {
                return Task.FromResult(Delete(entity));
            }
            catch (Exception ex)
            {
                return Task.FromException<bool>(ex);
            }
        }

        public Task<bool> DeleteAsync(IEnumerable<T> entities)
        {
            try
            {
                return Task.FromResult(Delete(entities));
            }
            catch (Exception ex)
            {
                return Task.FromException<bool>(ex);
            }
        }

        public IEnumerable<T> FindDataEntitiesByQuery(TKey key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TResult> FindEntitiesByQuery<TResult>(TKey key, Expression<Func<T, TResult>> selector = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TResult>> FindEntitiesByQueryAsync<TResult>(TKey key, Expression<Func<T, TResult>> selector = null)
        {
            throw new NotImplementedException();
        }

        public T GetEntityByQuery(TKey key)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetEntityByQueryAsync(TKey key)
        {
            throw new NotImplementedException();
        }

        public void Initialization()
        {
            throw new NotImplementedException();
        }

        public Task InitializationAsync()
        {
            throw new NotImplementedException();
        }

        public bool IsExisted(Expression<Func<T, bool>> filiter)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsExistedAsync(Expression<Func<T, bool>> filiter)
        {
            throw new NotImplementedException();
        }

        public bool Update(T entity)
        {
            throw new NotImplementedException();
        }

        public bool Update(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }
    }
}