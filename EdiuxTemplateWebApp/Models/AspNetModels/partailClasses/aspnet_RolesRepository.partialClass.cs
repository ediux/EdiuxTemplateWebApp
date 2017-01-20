using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_RolesRepository : EFRepository<aspnet_Roles>, Iaspnet_RolesRepository
    {
        public Task CreateAsync(aspnet_Roles role)
        {
            try
            {
                int returnCode = InternalDatabaseAlias.aspnet_Roles_CreateRole(role.aspnet_Applications.ApplicationName, role.Name);

                if (returnCode != 0)
                    return Task.FromException(new Exception("Has an error in database."));
                                
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }
        }

        public Task DeleteAsync(aspnet_Roles role)
        {
            try
            {
                int returnCode = InternalDatabaseAlias.aspnet_Roles_DeleteRole(role.aspnet_Applications.ApplicationName,role.Name,true);

                if (returnCode != 0)
                    return Task.FromException(new Exception("Has an error in database."));

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }
        }

        public Task<aspnet_Roles> FindByIdAsync(Guid roleId)
        {
            try
            {
                aspnet_Roles foundRole = Get(roleId);

                return Task.FromResult(foundRole);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<aspnet_Roles>(ex);
            }
        }

        public Task<aspnet_Roles> FindByNameAsync(string roleName)
        {
            try
            {
                aspnet_Roles foundRole = Where(w=>w.Name==roleName || w.LoweredRoleName == roleName).SingleOrDefault();

                return Task.FromResult(foundRole);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<aspnet_Roles>(ex);
            }
        }

        public Task UpdateAsync(aspnet_Roles role)
        {
            throw new NotImplementedException();
        }
    }

    public partial interface Iaspnet_RolesRepository : IRepositoryBase<aspnet_Roles>
    {
        Task CreateAsync(aspnet_Roles role);
        Task UpdateAsync(aspnet_Roles role);
        Task DeleteAsync(aspnet_Roles role);
        Task<aspnet_Roles> FindByIdAsync(Guid roleId);
        Task<aspnet_Roles> FindByNameAsync(string roleName);
    }
}