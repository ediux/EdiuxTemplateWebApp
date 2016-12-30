using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity;

namespace EdiuxTemplateWebApp.Models
{
    public partial class MenusRepository : EFRepository<Menus>, IMenusRepository
    {
        public override IQueryable<Menus> All()
        {
            try
            {
                ISystem_ApplicationsRepository sysAppRepo = RepositoryHelper.GetSystem_ApplicationsRepository(UnitOfWork);

                string typeName = typeof(MvcApplication).Namespace;

                System_Applications app = sysAppRepo.All().SingleOrDefault(s => s.Name == typeof(MvcApplication).Namespace);

                int appId = (app != null) ? app.Id : 0;

                var result = base.All()
                    .Where(w => w.Void == false
                    && w.ApplicationId == appId)
                    .Include(m => m.System_Applications)
                    .Include(m => m.ChildMenus)
                    .Include(m => m.ApplicationRole);                    

                result.Load();

                if (UnitOfWork.IsSet(nameof(Menus)))
                {
                    var cache = GetCache();
                    System.Collections.ObjectModel.ObservableCollection<Menus> newCache =
                        new System.Collections.ObjectModel.ObservableCollection<Menus>(
                            cache.Union(ObjectSet.Local).AsEnumerable());
                    UnitOfWork.Set(nameof(ApplicationUser), newCache, CacheExpiredTime);
                }

                return result;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }

        }

    }

    public partial interface IMenusRepository : IRepositoryBase<Menus>
    {
    }
}