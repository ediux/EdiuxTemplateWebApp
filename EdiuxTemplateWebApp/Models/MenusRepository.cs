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
                var result = base.All().Include(m => m.ChildMenus).Include(m => m.System_ControllerActions)
                     .Where(w => w.Void == false)
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

        public IQueryable<Menus> getMenusbyCurrentLoginUser()
        {
            try
            {
                IApplicationUserRepository userRepo = RepositoryHelper.GetApplicationUserRepository(UnitOfWork);

                int currentUserId = getCurrentLoginedUserId();
                string cacheKeyName = string.Format("UserMenu_{0}", currentUserId);

                if (UnitOfWork.IsSet(cacheKeyName))
                {
                    //如果有快取選單直接取出回傳
                    return (UnitOfWork.Get(cacheKeyName) as List<Menus>).AsQueryable();
                }

                Task<ApplicationUser> findbyIdTask = userRepo.FindByIdAsync(currentUserId);
                findbyIdTask.Wait();

                ApplicationUser user = findbyIdTask.Result;

                if (user != null)
                {
                    var getmenus = user.ApplicationRole
                        .SelectMany(s => s.Menus)
                        .Where(w => w.Void == false && w.AllowAnonymous == false)
                        .Union(ObjectSet.Where(w => w.AllowAnonymous == true
                    && w.Void == false
                    && (w.ParentMenuId == null
                    || w.ParentMenuId == 0))).Distinct().OrderBy(o => o.Order);

                    UnitOfWork.Set(cacheKeyName, getmenus.ToList(), 30);    //將選單存入快取

                    return getmenus.AsQueryable();
                }

                var anonoymousMenus = ObjectSet.Where(w => w.AllowAnonymous == true
                    && w.Void == false
                    && (w.ParentMenuId == null
                    || w.ParentMenuId == 0)).OrderBy(o => o.Order);

                UnitOfWork.Set(cacheKeyName, anonoymousMenus.ToList(), 30); //將選單存入快取

                return anonoymousMenus;

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
        IQueryable<Menus> getMenusbyCurrentLoginUser();
    }
}