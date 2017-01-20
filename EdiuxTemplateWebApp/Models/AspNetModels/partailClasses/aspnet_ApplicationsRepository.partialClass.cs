using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity.Core.Objects;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_ApplicationsRepository : EFRepository<aspnet_Applications>, Iaspnet_ApplicationsRepository
    {
        public override aspnet_Applications Add(aspnet_Applications entity)
        {
            try
            {
                ObjectParameter applicationId = new ObjectParameter("ApplicationId", typeof(Guid));
                InternalDatabaseAlias.aspnet_Applications_CreateApplication(entity.ApplicationName, applicationId);
                return Get(applicationId.Value);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public aspnet_Applications FindById(Guid applicationId)
        {
            try
            {
                return Get(applicationId);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public aspnet_Applications FindByName(string applicationName)
        {
            try
            {
                string loweredApplicationName = applicationName.ToLowerInvariant();
                return Where(s => s.ApplicationName == applicationName
                    || s.LoweredApplicationName == loweredApplicationName).SingleOrDefault();
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }
        public Task<aspnet_Applications> FindByIdAsync(Guid applicationId)
        {
            try
            {
                return Task.FromResult(FindById(applicationId));
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<aspnet_Applications>(ex);
            }
        }

        public Task<aspnet_Applications> FindByNameAsync(string applicationName)
        {
            try
            {                
                return Task.FromResult(FindByName(applicationName));
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<aspnet_Applications>(ex);
            }
        }

        protected override void Dispose(bool disposing)
        {            
            base.Dispose(disposing);
        }
    }

    public partial interface Iaspnet_ApplicationsRepository : IRepositoryBase<aspnet_Applications>
    {
        aspnet_Applications FindById(Guid applicationId);
        aspnet_Applications FindByName(string applicationName);
        Task<aspnet_Applications> FindByIdAsync(Guid applicationId);
        Task<aspnet_Applications> FindByNameAsync(string applicationName);
    }
}