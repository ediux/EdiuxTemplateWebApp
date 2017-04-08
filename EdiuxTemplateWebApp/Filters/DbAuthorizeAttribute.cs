using EdiuxTemplateWebApp.Models;
using EdiuxTemplateWebApp.Models.AspNetModels;
using EdiuxTemplateWebApp.Models.Shared;
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
        private Iaspnet_RolesRepository roleRepo;
        private Iaspnet_UsersRepository userRepo;

        private Iaspnet_ApplicationsRepository appRepo;

        public DbAuthorizeAttribute()
        {
            appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();
            roleRepo = RepositoryHelper.Getaspnet_RolesRepository(appRepo.UnitOfWork);
            userRepo = RepositoryHelper.Getaspnet_UsersRepository(roleRepo.UnitOfWork);

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

                aspnet_Applications appInfo = filterContext.Controller.getApplicationInfo();

                if (appInfo == null)
                {
                    Exception customerException = new Exception("Can't fetch application information from database.");

                    filterContext.Controller.TempData.Add("Exception", customerException);

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

                var loginUser = filterContext.RequestContext.HttpContext.User.Identity.GetUserId();

                if (appInfo.aspnet_Paths.Any(a => a.Path == filterContext.RequestContext.HttpContext.Request.Path))
                {
                    var pageInfo = appInfo.aspnet_Paths.SingleOrDefault(a => a.Path == filterContext.RequestContext.HttpContext.Request.Path);

                    if (pageInfo == null)
                    {
                        filterContext.Result = new HttpNotFoundResult(filterContext.RequestContext.HttpContext.Request.Path + " is not found.");
                        return;
                    }

                    if (pageInfo.aspnet_PersonalizationPerUser.Any(a => a.aspnet_Users.Id == loginUser))
                    {
                        byte[] data = pageInfo.aspnet_PersonalizationPerUser.SingleOrDefault(a => a.aspnet_Users.Id == loginUser).PageSettings;

                        if (data != null && data.Length > 0)
                        {
                            var pagePermission = data.Deserialize<PagePermissionForUserModel>();
                            if (pagePermission.CanAccess)
                            {
                                return;
                            }
                        }
                    }
                }
                //if (appInfo.isActionInApplication(filterContext.ActionDescriptor))
                //{
                //    System_ControllerActions actionInfo = appInfo.getMVCActionInfo(filterContext.ActionDescriptor);

                //    var users = appInfo.getUsers(filterContext.HttpContext.User.Identity.GetUserId<int>());

                //    if (users == null)
                //    {
                //        if(filterContext.HttpContext.User.Identity.IsAuthenticated && filterContext.HttpContext.User.Identity.Name=="root")
                //        {
                //            return;
                //        }

                //        filterContext.Result = new HttpUnauthorizedResult();
                //        return;
                //    }

                //    if (users.Length >= 1)
                //    {
                //        var loginedUser = users.OrderBy(o => o.Id).First();

                //        if (filterContext.Controller.ViewBag.CurrentLoginedUser == null)
                //        {
                //            //加入取得目前登入使用者的資訊
                //            filterContext.Controller.ViewBag.CurrentLoginedUser = loginedUser;
                //        }

                //        if (actionInfo.isUserAuthorizend(loginedUser) == false)
                //        {
                //            filterContext.Result = new HttpUnauthorizedResult();
                //            return;
                //        }

                //        //base.OnAuthorization(filterContext);
                //        return;
                //    }
                //}



                if (filterContext.ActionDescriptor.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any())
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

                if (ex is DbEntityValidationException)
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