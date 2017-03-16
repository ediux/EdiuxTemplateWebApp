using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_RolesRepository : EFRepository<aspnet_Roles>, Iaspnet_RolesRepository
    {
        public aspnet_Roles Add(aspnet_Applications application, string Name, string Desctiption = "")
        {
            try
            {
                UnitOfWork.Repositories.aspnet_Roles_CreateRole(application.ApplicationName, Name);
                return FindByName(application.ApplicationId, Name);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }

        }

        public override void Delete(aspnet_Roles entity)
        {
            try
            {
                int returnCode = InternalDatabaseAlias.aspnet_Roles_DeleteRole(entity.aspnet_Applications.ApplicationName, entity.Name, true);

                if (returnCode != 0)
                    throw new Exception("Has an error in database.");
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
        }

        public IQueryable<aspnet_Roles> All(aspnet_Applications application)
        {
            try
            {
                return Where(w => w.ApplicationId == application.ApplicationId);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public aspnet_Roles FindById(Guid applicationId, Guid roleId)
        {
            try
            {
                return All().SingleOrDefault(s => s.ApplicationId == applicationId && s.Id == roleId);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }
        public aspnet_Roles FindByName(Guid applicationId, string Name)
        {
            try
            {
                string loweredName = Name.ToLowerInvariant();
                return All().SingleOrDefault(s => s.ApplicationId == applicationId && (s.Name == Name || s.LoweredRoleName == loweredName));
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task CreateAsync(aspnet_Roles role)
        {
            try
            {
                return Task.FromResult(Add(role.aspnet_Applications, role.Name, role.Description));
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
                Delete(role);
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
                aspnet_Roles foundRole = Where(w => w.Name == roleName || w.LoweredRoleName == roleName).SingleOrDefault();

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
            try
            {
                UnitOfWork.Context.Entry(role).State = System.Data.Entity.EntityState.Modified;
                return UnitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }
    }

    public partial interface Iaspnet_RolesRepository : IRepositoryBase<aspnet_Roles>
    {
        aspnet_Roles Add(aspnet_Applications application, string Name, string Desctiption = "");
        Task CreateAsync(aspnet_Roles role);
        Task UpdateAsync(aspnet_Roles role);
        Task DeleteAsync(aspnet_Roles role);
        aspnet_Roles FindById(Guid applicationId, Guid roleId);
        aspnet_Roles FindByName(Guid applicationId, string Name);
        Task<aspnet_Roles> FindByIdAsync(Guid roleId);
        Task<aspnet_Roles> FindByNameAsync(string roleName);
    }
}
