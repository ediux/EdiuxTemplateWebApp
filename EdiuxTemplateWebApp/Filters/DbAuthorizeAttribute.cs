using EdiuxTemplateWebApp.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EdiuxTemplateWebApp.Filters
{
    public class DbAuthorizeAttribute : AuthorizeAttribute
    {
        private IApplicationRoleRepository roleRepo;
        private IApplicationUserRepository userRepo;
        private ISystem_ControllerActionsRepository actionRepo;
        private ISystem_ControllersRepository ctrlRepo;
        private ISystem_ApplicationsRepository appRepo;

        public DbAuthorizeAttribute()
        {
            appRepo = RepositoryHelper.GetSystem_ApplicationsRepository();
            roleRepo = RepositoryHelper.GetApplicationRoleRepository(appRepo.UnitOfWork);
            userRepo = RepositoryHelper.GetApplicationUserRepository(roleRepo.UnitOfWork);
            actionRepo = RepositoryHelper.GetSystem_ControllerActionsRepository(userRepo.UnitOfWork);
            ctrlRepo = RepositoryHelper.GetSystem_ControllersRepository(userRepo.UnitOfWork);
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {
                if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLowerInvariant() == "error")
                {
                    return;
                }

                if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLowerInvariant() == "elmah")
                {
                    if (filterContext.HttpContext.User.Identity.IsAuthenticated && filterContext.HttpContext.User.Identity.Name == "root")
                    {
                        return;
                    }
                }

                System_Applications appInfo = null;

                if (filterContext.Controller.ViewBag.ApplicationInfo != null)
                {
                    appInfo = (System_Applications)filterContext.Controller.ViewBag.ApplicationInfo;
                }
                else
                {
                    appInfo = appRepo.getInfoByType(filterContext.ActionDescriptor.ControllerDescriptor.ControllerType);

                    if (appInfo == null)
                    {
                        Exception customerException = new Exception("Can't fetch application information from database.");

                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                        {
                            { "controller", "Error" },
                            { "action", "Index" },
                            { "actionName",filterContext.ActionDescriptor.ActionName},
                            { "controllerName",filterContext.ActionDescriptor.ControllerDescriptor.ControllerName },
                            { "statusCode", System.Net.HttpStatusCode.BadRequest },
                            { "ex", customerException }
                        });

                        WriteErrorLog(customerException);

                        return;
                    }

                    filterContext.Controller.ViewBag.ApplicationInfo = appInfo;
                }

                if (appInfo.isActionInApplication(filterContext.ActionDescriptor))
                {
                    System_ControllerActions actionInfo = appInfo.getMVCActionInfo(filterContext.ActionDescriptor);

                    var users = appInfo.getUsers(filterContext.HttpContext.User.Identity.GetUserId<int>());

                    if (users == null)
                    {
                        if(filterContext.HttpContext.User.Identity.IsAuthenticated && filterContext.HttpContext.User.Identity.Name=="root")
                        {
                            return;
                        }

                        filterContext.Result = new HttpUnauthorizedResult();
                        return;
                    }

                    if (users.Length >= 1)
                    {
                        var loginedUser = users.OrderBy(o => o.Id).First();

                        if (filterContext.Controller.ViewBag.CurrentLoginedUser == null)
                        {
                            //加入取得目前登入使用者的資訊
                            filterContext.Controller.ViewBag.CurrentLoginedUser = loginedUser;
                        }

                        if (actionInfo.isUserAuthorizend(loginedUser) == false)
                        {
                            filterContext.Result = new HttpUnauthorizedResult();
                            return;
                        }

                        //base.OnAuthorization(filterContext);
                        return;
                    }
                }

                if(filterContext.ActionDescriptor.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any())
                {
                    return;
                }

                if (filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any())
                {
                    return;
                }

                filterContext.Result = new HttpUnauthorizedResult();
                return;

            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);

                if(ex is DbEntityValidationException)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {
                        { "controller", "Error" },
                        { "action", "DbEntityValidationError" },
                        { "actionName",filterContext.ActionDescriptor.ActionName},
                        { "controllerName",filterContext.ActionDescriptor.ControllerDescriptor.ControllerName },
                        { "ex", ex }
                    });
                    return;
                }

                if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLowerInvariant() == "elmah")
                {
                    if (filterContext.HttpContext.User.Identity.IsAuthenticated && filterContext.HttpContext.User.Identity.Name == "root")
                    {
                        return;
                    }
                }

                if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLowerInvariant() == "error")
                {
                    return;
                }

                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "Error" },
                    { "action", "Index" },
                    { "actionName",filterContext.ActionDescriptor.ActionName},
                    { "controllerName",filterContext.ActionDescriptor.ControllerDescriptor.ControllerName },
                    { "statusCode", 500 },
                    { "ex", ex }
                });
            }
        }

        protected virtual void WriteErrorLog(Exception ex)
        {
            if (System.Web.HttpContext.Current == null)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
            else
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
    }
}