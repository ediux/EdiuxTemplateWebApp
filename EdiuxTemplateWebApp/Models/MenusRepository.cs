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

                System_Applications app = sysAppRepo.All().SingleOrDefault(s => s.Name == typeof(MvcApplication).Name);

                int appId = (app != null) ? app.Id : 0;
               
                var result = base.All()
                    .Include(m => m.ChildMenus)
                    .Include(m => m.System_ControllerActions)
                    .Where(w => w.Void == false 
                    && w.ApplicationId==appId)
                    .AsQueryable();

                if (result.Count() > 0)
                    return result;

                result.Load();

                return result;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }

        }

        public IQueryable<Menus> getMenusbyCurrentLoginUser(Type AppRuntimeType)
        {
            try
            {
                IApplicationUserRepository userRepo = RepositoryHelper.GetApplicationUserRepository(UnitOfWork);
                ISystem_ApplicationsRepository sysAppRepo = RepositoryHelper.GetSystem_ApplicationsRepository(UnitOfWork);

                System_Applications app = sysAppRepo.All().SingleOrDefault(s => s.Name == AppRuntimeType.Name);



                int currentUserId = getCurrentLoginedUserId();
                string cacheKeyName = string.Format("UserMenu_{0}", currentUserId);

                if (UnitOfWork.IsSet(cacheKeyName))
                {
                    //如果有快取選單直接取出回傳
                    return (UnitOfWork.Get(cacheKeyName) as List<Menus>).AsQueryable();
                }

                if (app == null)
                {
                    UnitOfWork.Set(cacheKeyName, new List<Menus>(), CacheExpiredTime);
                    return (UnitOfWork.Get(cacheKeyName) as List<Menus>).AsQueryable();
                }

                Task<ApplicationUser> findbyIdTask = userRepo.FindByIdAsync(currentUserId);
                findbyIdTask.Wait();

                ApplicationUser user = findbyIdTask.Result;

                var anonymousMenus = ObjectSet.Where(w =>
                    w.ApplicationId == app.Id
                    && w.AllowAnonymous == true
                    && w.Void == false
                    && (w.ParentMenuId == null
                    || w.ParentMenuId == 0)).OrderBy(o => o.Order);

                if (user != null)
                {
                    var getmenus = user.ApplicationRole
                        .SelectMany(s => s.Menus)
                        .OrderBy(o => o.Order);

                    if (getmenus != null)
                    {
                        var getauthMenus = getmenus.Where(w => w.ApplicationId == app.Id
                        && w.Void == false
                        && w.AllowAnonymous == false);

                        if (getauthMenus != null && getauthMenus.Any())
                        {
                            getauthMenus = getauthMenus.
                                Where(s => s.System_ControllerActions != null);

                            if (getauthMenus != null && getauthMenus.Any())
                            {
                                getauthMenus = getauthMenus.
                                Where(s => s.System_ControllerActions.System_Controllers != null);

                                if (getauthMenus != null && getauthMenus.Any())
                                {
                                    getauthMenus = getauthMenus.
                                        Where(s => s.System_ControllerActions.System_Controllers.Namespace.Contains(AppRuntimeType.Namespace));

                                    if (getauthMenus != null && getauthMenus.Any())
                                    {
                                        var cacheSet = getauthMenus.Union(anonymousMenus);
                                        UnitOfWork.Set(cacheKeyName, cacheSet.ToList(), CacheExpiredTime);
                                        return cacheSet.AsQueryable();
                                    }
                                }
                            }
                        }
                    }
                }

                UnitOfWork.Set(cacheKeyName, anonymousMenus.ToList(), 30); //將選單存入快取

                return anonymousMenus;

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
        IQueryable<Menus> getMenusbyCurrentLoginUser(Type AppRuntimeType);
    }
}