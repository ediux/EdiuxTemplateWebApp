using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Collections.ObjectModel;

namespace EdiuxTemplateWebApp.Models
{
    public partial class ApplicationUserRepository : EFRepository<ApplicationUser>, IApplicationUserRepository
        , Interfaces.IDataRepository<ApplicationUser>
    {
        private const string KeyName = nameof(ApplicationUser);

        public override IQueryable<ApplicationUser> All()
        {
            //檢查是否有快取，有就快取先行否則從資料庫載入

            if (UnitOfWork.IsSet(KeyName) == false)
            {
                Task<List<ApplicationUser>> asyncResult = ObjectSet.Where(p => p.Void == false).ToListAsync();
                asyncResult.Wait();
                UnitOfWork.Set(KeyName, asyncResult.Result, 30); //快取保留30分鐘
                return asyncResult.Result.AsQueryable();
            }
            else
            {
                List<ApplicationUser> _cache =
                    UnitOfWork.Get(KeyName) as List<ApplicationUser>;
                return _cache.AsQueryable();
            }
        }

        public override ApplicationUser Add(ApplicationUser entity)
        {
            List<ApplicationUser> _cache =
                   UnitOfWork.Get(KeyName) as List<ApplicationUser>;
            if (_cache != null)
            {
                _cache.Add(entity);
                UnitOfWork.Set(KeyName, _cache, 30);
            }            
            return base.Add(entity);
        }

        public override void Delete(ApplicationUser entity)
        {
            List<ApplicationUser> _cache =
       UnitOfWork.Get(KeyName) as List<ApplicationUser>;
            if (_cache != null)
            {
                _cache.Remove(entity);
                UnitOfWork.Set(KeyName, _cache, 30);
            }
            base.Delete(entity);
        }

        public void ClearCache(string key)
        {            
            UnitOfWork.Invalidate(key);
        }

        public Task CreateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> FindByIdAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ApplicationUser> GetCache()
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsInRoleAsync(ApplicationUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(ApplicationUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }
    }

    public partial interface IApplicationUserRepository : IRepositoryBase<ApplicationUser>
    {
        Task CreateAsync(ApplicationUser user);
        Task DeleteAsync(ApplicationUser user);
        Task<ApplicationUser> FindByIdAsync(int userId);
        Task<ApplicationUser> FindByNameAsync(string userName);
        Task<IList<string>> GetRolesAsync(ApplicationUser user);
        Task<bool> IsInRoleAsync(ApplicationUser user, string roleName);
        Task RemoveFromRoleAsync(ApplicationUser user, string roleName);
        Task UpdateAsync(ApplicationUser user);
    }
}