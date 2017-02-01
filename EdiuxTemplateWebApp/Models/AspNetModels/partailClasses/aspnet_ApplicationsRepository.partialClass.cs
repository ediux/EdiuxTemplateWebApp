using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Data;
using System.Data.SqlClient;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_ApplicationsRepository : EFRepository<aspnet_Applications>, Iaspnet_ApplicationsRepository
    {
        public override IQueryable<aspnet_Applications> Where(Expression<Func<aspnet_Applications, bool>> expression)
        {
            return All().Where(expression);
        }
        public override IQueryable<aspnet_Applications> All()
        {
            UnitOfWork.Context.Configuration.LazyLoadingEnabled = false;

            IQueryable<aspnet_Applications> loadAllQueryable = (from a in InternalDatabaseAlias.aspnet_Applications
                                                                join m in InternalDatabaseAlias.aspnet_Membership on a.ApplicationId equals m.ApplicationId
                                                                join u in InternalDatabaseAlias.aspnet_Users on a.ApplicationId equals u.ApplicationId
                                                                join r in InternalDatabaseAlias.aspnet_Roles on a.ApplicationId equals r.ApplicationId
                                                                join p in InternalDatabaseAlias.aspnet_Paths on a.ApplicationId equals p.ApplicationId
                                                                join v in InternalDatabaseAlias.aspnet_VoidUsers on a.ApplicationId equals v.ApplicationId
                                                                join menu in InternalDatabaseAlias.Menus on a.ApplicationId equals menu.ApplicationId
                                                                select a).AsQueryable();

            return loadAllQueryable;

        }
        public override aspnet_Applications Get(params object[] values)
        {
            return All().SingleOrDefault(w => values.Any(a => a.Equals(w.ApplicationId)
            || a.Equals(w.ApplicationName) || a.Equals(w.LoweredApplicationName)));
        }
        public override aspnet_Applications Add(aspnet_Applications entity)
        {
            try
            {
                return CreateApplication(entity.ApplicationName);
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
                aspnet_Applications appInfo = Get(applicationId);

                return appInfo;
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
                aspnet_Applications appInfo = All().Where(s => s.ApplicationName == applicationName
                    || s.LoweredApplicationName == loweredApplicationName).SingleOrDefault();


                return appInfo;
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

        public aspnet_Applications CreateApplication(string applicationName)
        {
            try
            {
                ObjectParameter applicationId = new ObjectParameter("ApplicationId", typeof(Guid));
                InternalDatabaseAlias.aspnet_Applications_CreateApplication(applicationName, applicationId);
                return Get(applicationId.Value);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }

    public partial interface Iaspnet_ApplicationsRepository : IRepositoryBase<aspnet_Applications>
    {
        aspnet_Applications CreateApplication(string applicationName);
        aspnet_Applications FindById(Guid applicationId);
        aspnet_Applications FindByName(string applicationName);
        Task<aspnet_Applications> FindByIdAsync(Guid applicationId);
        Task<aspnet_Applications> FindByNameAsync(string applicationName);
    }
}