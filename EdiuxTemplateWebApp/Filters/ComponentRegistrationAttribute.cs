using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using EdiuxTemplateWebApp.Models;
using System.Data.Entity.Validation;
using System.Web.Routing;
using System.Reflection;

namespace EdiuxTemplateWebApp.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ComponentRegistrationAttribute : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                System_Applications appInfo = null;

                ISystem_ApplicationsRepository appRepo = RepositoryHelper.GetSystem_ApplicationsRepository();

                if (filterContext.Controller.ViewBag.ApplicationInfo != null)
                {
                    appInfo = (System_Applications)filterContext.Controller.ViewBag.ApplicationInfo;
                }

                string appName = HttpRuntime.AppDomainAppVirtualPath.Trim('/').Replace("/", ".");

                if (string.IsNullOrEmpty(appName))
                {
                    if (System.Web.Configuration.WebConfigurationManager.AppSettings.AllKeys.Contains("AppName"))
                    {
                        appName = System.Web.Configuration.WebConfigurationManager.AppSettings["AppName"];
                    }
                }

                if (string.IsNullOrEmpty(appName))
                    appName = typeof(MvcApplication).Namespace;

                if (appInfo == null)
                {
                    appInfo = appRepo.All().SingleOrDefault(w => w.Name.Equals(appName, StringComparison.InvariantCultureIgnoreCase));

                    if (appInfo == null)
                    {
                        appInfo = new System_Applications();
                        appInfo.Name = appName;
                        appInfo.LoweredName = appName.ToLowerInvariant();
                        appInfo.Description = string.Format("名稱為「{0}」的MVC 5 應用程式", appName);
                        appInfo.Namespace = typeof(MvcApplication).Namespace;                                               
                        appInfo = appRepo.Add(appInfo);
                        appRepo.UnitOfWork.Commit();
                        appInfo = appRepo.Reload(appInfo);
                    }
                }

                Type ControllerType = filterContext.ActionDescriptor.ControllerDescriptor.ControllerType;

                string ctrlName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                string clsName = ControllerType.Name;
                string namespaceName = ControllerType.Namespace;

                ISystem_ControllersRepository CtrRepo = RepositoryHelper.GetSystem_ControllersRepository(appRepo.UnitOfWork);

                ISystem_ControllerActionsRepository ActionRepo = RepositoryHelper.GetSystem_ControllerActionsRepository(CtrRepo.UnitOfWork);

                int currentUserId = filterContext.HttpContext.User.Identity.GetUserId<int>();
              
                System_Controllers ctrl = null;

                ctrl = CtrRepo.All().FirstOrDefault(w =>
                w.Namespace.Equals(namespaceName, StringComparison.InvariantCultureIgnoreCase)
                && w.ClassName.Equals(clsName, StringComparison.InvariantCultureIgnoreCase));

                if (ctrl == null)
                {                    
                    ctrl = CtrRepo.ComponentRegistration(ControllerType);   //元件註冊                   
                    CtrRepo.UnitOfWork.Commit();
                    ctrl = CtrRepo.Reload(ctrl);
                    ctrl.ApplicationId = appInfo.Id;
                    CtrRepo.UnitOfWork.Context.Entry(ctrl).State = System.Data.Entity.EntityState.Modified;
                    CtrRepo.UnitOfWork.Commit();
                }

                if (ctrl.ApplicationId.HasValue == false)
                {
                    ctrl.ApplicationId = appInfo.Id;
                    CtrRepo.UnitOfWork.Context.Entry(ctrl).State = System.Data.Entity.EntityState.Modified;
                    CtrRepo.UnitOfWork.Commit();
                }

                string actionname = filterContext.ActionDescriptor.ActionName;

                System_ControllerActions action = ActionRepo.All().FirstOrDefault(
                    w => w.Name.Equals(actionname, StringComparison.InvariantCultureIgnoreCase)
                    && w.ControllerId == ctrl.Id);

                if (action == null)
                {
                    action = ActionRepo.ComponentRegistration(ctrl, filterContext.ActionDescriptor);                    
                    ActionRepo.UnitOfWork.Commit();
                    action = ActionRepo.Reload(action);
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                if (ex is DbEntityValidationException)
                {
                    filterContext.Controller.TempData["Exception"] = ex;

                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {
                        { "controller", "Error" },
                        { "action", "DbEntityValidationError" },
                        { "actionName",filterContext.ActionDescriptor.ActionName},
                        { "controllerName",filterContext.ActionDescriptor.ControllerDescriptor.ControllerName },
                    });

                    return;
                }
                throw ex;
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

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ApiComponentRegistrationAttribut : FilterAttribute, System.Web.Http.Filters.IFilter
    {
    }
}