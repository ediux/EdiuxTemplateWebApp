using EdiuxTemplateWebApp.Models.AspNetModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models
{
    public class StoreBase<T, TKey> : IStoreBase<T, TKey>
        where T : class
    {
        IRepositoryBase<T> repository;

        public StoreBase()
        {
            repository = GetRepository();
            Initialization();
        }

        protected virtual IRepositoryBase<T> GetRepository()
        {
            Type repoType = typeof(T);
            string repositoryFunctionName = string.Format("Get{0}Repository", repoType.Name);
            Type repoHelper = typeof(RepositoryHelper);
            var callfunc = repoHelper.GetMethod(repositoryFunctionName, BindingFlags.Static | BindingFlags.Public);
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
            var func = createEntityFuncType.GetMethod("Create", BindingFlags.Static | BindingFlags.Public);
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

        public virtual bool Delete(T entity)
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

        protected MemberInfo[] IdentifyPrimaryKeyMembers()
        {
            ObjectContext objectContext = repository.UnitOfWork.Context.ObjectContext;
            ObjectSet<T> set = objectContext.CreateObjectSet<T>();
            IEnumerable<string> keyNames = set.EntitySet.ElementType
                                                        .KeyMembers
                                                        .Select(k => k.Name);

            Type entityreflection = typeof(T);

            var pkeys = entityreflection.GetMembers()
                .Join(keyNames, (x) => x.Name, (y) => y, (k, t) => k);

            return pkeys.ToArray();
        }

        public virtual bool Delete(IEnumerable<T> entities)
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

        public virtual IEnumerable<T> FindDataEntitiesByQuery(TKey key)
        {
            ParameterExpression numParam = Expression.Parameter(typeof(TKey), "x");

            Expression<Func<T, bool>> wherefiliter = Expression.Lambda<Func<T, bool>>(Expression.Equal(Expression.MakeMemberAccess(numParam, IdentifyPrimaryKeyMembers()
                .Where(w => w.MemberType == MemberTypes.Property && w.ReflectedType == typeof(TKey)).Single()), Expression.Constant(key)));

            return repository.Where(wherefiliter).ToList();
        }

        public virtual IEnumerable<TResult> FindEntitiesByQuery<TResult>(TKey key, Expression<Func<T, TResult>> selector = null)
        {
            ParameterExpression numParam = Expression.Parameter(typeof(TKey), "x");

            Expression<Func<T, bool>> wherefiliter = Expression.Lambda<Func<T, bool>>(Expression.Equal(Expression.MakeMemberAccess(numParam, IdentifyPrimaryKeyMembers()
                .Where(w => w.MemberType == MemberTypes.Property && w.ReflectedType == typeof(TKey)).Single()), Expression.Constant(key)));

            if (selector != null)
            {
                return repository.Where(wherefiliter).Select(selector);
            }

            return repository.Where(wherefiliter).Select(s => (TResult)Convert.ChangeType(s, typeof(TKey)));
        }

        public Task<IEnumerable<TResult>> FindEntitiesByQueryAsync<TResult>(TKey key, Expression<Func<T, TResult>> selector = null)
        {
            try
            {
                return Task.FromResult(FindEntitiesByQuery(key, selector));
            }
            catch (Exception ex)
            {
                return Task.FromException<IEnumerable<TResult>>(ex);
            }
        }

        public virtual T GetEntityByQuery(TKey key)
        {
            return repository.Get(key);
        }

        public Task<T> GetEntityByQueryAsync(TKey key)
        {
            try
            {
                return Task.FromResult(GetEntityByQuery(key));
            }
            catch (Exception ex)
            {
                return Task.FromException<T>(ex);
            }
        }

        public virtual void Initialization()
        {
            
        }

        public Task InitializationAsync()
        {
            try
            {
                Initialization();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }

        public virtual bool IsExisted(Expression<Func<T, bool>> filiter)
        {
            return repository.Where(filiter).Any();
        }

        public Task<bool> IsExistedAsync(Expression<Func<T, bool>> filiter)
        {
            try
            {
                return Task.FromResult(IsExisted(filiter));
            }
            catch (Exception ex)
            {
                return Task.FromException<bool>(ex);
            }
        }

        public virtual bool Update(T entity)
        {
            throw new NotImplementedException();
        }

        public virtual bool Update(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(T entity)
        {
            try
            {
                return Task.FromResult(Update(entity));
            }
            catch (Exception ex)
            {
                return Task.FromException<bool>(ex);
            }
        }

        public Task<bool> UpdateAsync(IEnumerable<T> entities)
        {
            try
            {
                return Task.FromResult(Update(entities));
            }
            catch (Exception ex)
            {
                return Task.FromException<bool>(ex);
            }
        }
    }
}