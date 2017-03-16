using System;
using System.Collections.Generic;
using System.Linq;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{

    public partial class aspnet_ApplicationsRepository : EFRepository<aspnet_Applications>, Iaspnet_ApplicationsRepository
    {

        public aspnet_ApplicationsRepository() : base()
        {
            
        }

      
        public override IQueryable<aspnet_Applications> All()
        {
            UnitOfWork.LazyLoadingEnabled = false;

            IQueryable<aspnet_Applications> loadAllQueryable = (from a in ObjectSet
                                                                join m in UnitOfWork.Set<aspnet_Membership>() on a.ApplicationId equals m.ApplicationId
                                                                join u in UnitOfWork.Set<aspnet_Users>() on a.ApplicationId equals u.ApplicationId
                                                                join r in UnitOfWork.Set<aspnet_Roles>() on a.ApplicationId equals r.ApplicationId
                                                                join p in UnitOfWork.Set<aspnet_Paths>() on a.ApplicationId equals p.ApplicationId
                                                                join v in UnitOfWork.Set<aspnet_VoidUsers>() on a.ApplicationId equals v.ApplicationId
                                                                join menu in UnitOfWork.Set<Menus>() on a.ApplicationId equals menu.ApplicationId
                                                                select a).AsQueryable();

            return loadAllQueryable;

        }

        public IEnumerable<aspnet_Applications> FindById(Guid applicationId)
        {
            try
            {
                IEnumerable<aspnet_Applications> appInfos = All().Where(s => s.ApplicationId == applicationId);
                return appInfos;
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<aspnet_Applications> FindByName(string applicationName)
        {
            try
            {
                string loweredApplicationName = applicationName.ToLowerInvariant();
                IEnumerable<aspnet_Applications> appInfo = All().Where(s => s.ApplicationName == applicationName
                    || s.LoweredApplicationName == loweredApplicationName);
                return appInfo;
            }
            catch
            {
                throw;
            }
        }
    }

    public partial interface Iaspnet_ApplicationsRepository : IRepositoryBase<aspnet_Applications>
    {

        IEnumerable<aspnet_Applications> FindByName(string applicationName);
        IEnumerable<aspnet_Applications> FindById(Guid applicationId);
    }
}
