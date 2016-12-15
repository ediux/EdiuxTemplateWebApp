using EdiuxTemplateWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EdiuxTemplateWebApp.Filters
{
    public class DbAuthorizeAttribute : AuthorizeAttribute
    {
        private IApplicationRoleRepository roleRepo;

        public DbAuthorizeAttribute()
        {
            roleRepo = RepositoryHelper.GetApplicationRoleRepository();

        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            string actionName = filterContext.ActionDescriptor.ActionName;
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string controllerClassName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerType.Name;
            string controllerNamespace = filterContext.ActionDescriptor.ControllerDescriptor.ControllerType.Namespace;

            Task<ApplicationUser> currentUser = null;


            IApplicationUserRepository userRepo = RepositoryHelper.GetApplicationUserRepository();

            ISystem_ControllerActionsRepository actionRepo = RepositoryHelper.GetSystem_ControllerActionsRepository(userRepo.UnitOfWork);
            ISystem_ControllersRepository ctrlRepo = RepositoryHelper.GetSystem_ControllersRepository(userRepo.UnitOfWork);

            currentUser = userRepo.FindByNameAsync(filterContext.HttpContext.User.Identity.Name);

            currentUser.Wait();
            //currentUser.Start();

            System_ControllerActions action = actionRepo.All().FirstOrDefault(s => s.Name == actionName
            && s.System_Controllers.ClassName.Equals(controllerClassName)
            && s.System_Controllers.Namespace.Equals(controllerNamespace));

            if (action != null)
            {
                //如果有授權這個角色使用動作
                if (action.ApplicationRole.SelectMany(s => s.ApplicationUser)
                    .Where(s => s.Id == currentUser.Id).Any())
                    return;

                if (action.AllowAnonymous)
                    return;

                if (action.System_Controllers.AllowAnonymous)
                    return;

                //檢查角色授權(選單)
                if (currentUser.Result.ApplicationRole.Count > 0)
                {
                    if (action.Menus != null)
                    {
                        if (action.Menus
                            .SelectMany(s => s.ApplicationRole)
                            .Where(w => w.ApplicationUser.Where(s => s.Id == currentUser.Id).Any()).Any())
                        {
                            return;
                        }

                        var userMenus = currentUser.Result.ApplicationRole
                           .SelectMany(s => s.Menus);

                        if (userMenus != null && userMenus.Any())
                        {
                            userMenus = userMenus.Where(w => w.System_ControllerActions != null);
                            if (userMenus
                                   .Where(w => w.System_ControllerActions.Name == actionName
                                   && w.System_ControllerActions.System_Controllers.ClassName == controllerClassName
                                   && w.System_ControllerActions.System_Controllers.Namespace == controllerNamespace)
                                   .Any(m => m.AllowAnonymous == true))
                            {
                                return;
                            }
            


                        }

                    }

                }
            }



            if (filterContext.ActionDescriptor.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any())
                return;

            if (filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any())
                return;

            if (filterContext.HttpContext.User.Identity.Name.Equals("root", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            filterContext.Result = new HttpUnauthorizedResult();
            //base.OnAuthorization(filterContext);
        }

    }
}