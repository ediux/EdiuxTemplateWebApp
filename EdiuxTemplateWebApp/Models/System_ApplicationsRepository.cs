using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models
{   
	public  partial class System_ApplicationsRepository : EFRepository<System_Applications>, ISystem_ApplicationsRepository
	{
        public override IQueryable<System_Applications> All()
        {
            try
            {
                return GetCache().AsQueryable();
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
           
        }

        public System_Applications getInfoByType(Type appType)
        {
            string appNamespace = appType.Namespace;

            Models.System_Applications appInfo = All().SingleOrDefault(w => w.Namespace == appNamespace);

            if (appInfo == null)
            {
                appInfo = new Models.System_Applications() { Id = 0, Namespace = appNamespace, Name = appType.Name, LoweredName = appType.Name.ToLowerInvariant(), Description = "" };
                Add(appInfo);
                UnitOfWork.Commit();
                appInfo = Reload(appInfo);
            }

            return appInfo;
        }
    }

	public  partial interface ISystem_ApplicationsRepository : IRepositoryBase<System_Applications>
	{
        System_Applications getInfoByType(Type appType);
    }
}