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

        #endregion
    }

    public  partial interface IApplicationRoleRepository : IRepositoryBase<ApplicationRole>
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