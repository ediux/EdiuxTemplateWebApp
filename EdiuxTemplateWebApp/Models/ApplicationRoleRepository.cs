using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity;

namespace EdiuxTemplateWebApp.Models
{
    public partial class ApplicationRoleRepository : EFRepository<ApplicationRole>, IApplicationRoleRepository
    {
#if CS6
        private const string KeyName = nameof(ApplicationRole);
#else
        private static string KeyName = typeof(ApplicationRole).Name;
#endif

        #region Role Store
        public Task CreateAsync(ApplicationRole role)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(ApplicationRole role)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationRole> FindByIdAsync(int roleId)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationRole> FindByNameAsync(string roleName)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(ApplicationRole role)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Helper Function
        #region Cache
        public void ClearCache(string key)
        {
            try
            {
                UnitOfWork.Invalidate(key);
            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                throw ex;
            }
        }

        public IQueryable<ApplicationRole> GetCache()
        {
            try
            {
                if (UnitOfWork.IsSet(KeyName) == false)
                {
                    IQueryable<ApplicationRole> asyncResult = ObjectSet
                        .Include(p => p.System_ControllerActions);

                    Task<List<ApplicationRole>> _cacheList = asyncResult.ToListAsync();
                    _cacheList.Wait();
                    UnitOfWork.Set(KeyName, _cacheList.Result, 30); //快取保留30分鐘
                    return asyncResult;
                }
                else
                {
                    List<ApplicationRole> _cache =
                        UnitOfWork.Get(KeyName) as List<ApplicationRole>;
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
        #endregion

        private void RemoveFromCache(ApplicationRole entity)
        {
            try
            {
                if (FromCache.Any(p => p.Id == entity.Id))
                {
                    List<ApplicationRole> cacheSet = FromCache;
                    cacheSet.Remove(FromCache.Find(p => p.Id == entity.Id));
                    UnitOfWork.Set(KeyName, cacheSet, 30);
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

        private void AddToCache(ApplicationRole entity)
        {
            try
            {
                //加入到記憶體快取
                List<ApplicationUser> memoryUsersCache = FromCache;
                if (memoryUsersCache.Any(w => w.Id == entity.Id))   //確認快取有該筆資料
                {
                    RemoveFromCache(entity);
                }
                memoryUsersCache.Add(entity);
                UnitOfWork.Set(KeyName, memoryUsersCache, 30);
            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                throw ex;
            }
        }

        private void UpdateCache(int id, ApplicationRole updated)
        {
            try
            {
                //加入到記憶體快取
                List<ApplicationUser> memoryUsersCache = FromCache;
                if (memoryUsersCache.Any(w => w.Id == id))   //確認快取有該筆資料
                {
                    RemoveFromCache(memoryUsersCache.Find(p => p.Id == id));    //移除快取資料
                }
                memoryUsersCache.Add(updated);
                UnitOfWork.Set(KeyName, memoryUsersCache, 30);
            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                throw ex;
            }
        }



        private List<ApplicationRole> FromCache
        {
            get
            {
                try
                {
                    List<ApplicationUser> _cache = GetCache().ToList();
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
    }

    public  partial interface IApplicationRoleRepository : IRepositoryBase<ApplicationRole>, Interfaces.IDataRepository<ApplicationRole>
    {
        #region Role Store
        Task CreateAsync(ApplicationRole role);
        Task UpdateAsync(ApplicationRole role);
        Task DeleteAsync(ApplicationRole role);
        Task<ApplicationRole> FindByIdAsync(int roleId);
        Task<ApplicationRole> FindByNameAsync(string roleName);

        #endregion
    }
}