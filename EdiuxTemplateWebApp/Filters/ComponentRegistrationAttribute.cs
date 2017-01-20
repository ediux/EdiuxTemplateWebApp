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
using EdiuxTemplateWebApp.Models.AspNetModels;

namespace EdiuxTemplateWebApp.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ComponentRegistrationAttribute : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                aspnet_Applications appInfo = null;

                Iaspnet_ApplicationsRepository appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();

                if (filterContext.Controller.ViewBag.ApplicationInfo != null)
                {
                    appInfo = filterContext.Controller.ViewBag.ApplicationInfo;
                }

                if (appInfo == null)
                {
                    Filters.ApplicationIdentifyAttribute appIdentifyAttr = new ApplicationIdentifyAttribute(typeof(MvcApplication));
                    appIdentifyAttr.OnActionExecuting(filterContext);
                }

                //Type ControllerType = filterContext.ActionDescriptor.ControllerDescriptor.ControllerType;

                //string ctrlName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                //string clsName = ControllerType.Name;
                //string namespaceName = ControllerType.Namespace;

                string url = UrlHelper.GenerateUrl("Default", filterContext.ActionDescriptor.ActionName, filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                    filterContext.RouteData.Values, RouteTable.Routes, filterContext.RequestContext, true);

                aspnet_Paths pathInfo;

                Iaspnet_PathsRepository pathRepo = RepositoryHelper.Getaspnet_PathsRepository(appRepo.UnitOfWork);

                if (pathRepo.Where(w => w.Path == url && w.ApplicationId == appInfo.ApplicationId).Any() == false)
                {
                    pathInfo = pathRepo.Add(new aspnet_Paths()
                    {
                        ApplicationId = appInfo.ApplicationId,
                        LoweredPath = url.ToLowerInvariant(),
                        Path = url,
                        PathId = Guid.NewGuid()
                    });

                    pathRepo.UnitOfWork.Commit();
                }
                else
                {
                    string loweredUrl = url.ToLowerInvariant();
                    pathInfo = pathRepo.Where(w => (w.Path == url || w.LoweredPath == loweredUrl)
                    && w.ApplicationId == appInfo.ApplicationId).SingleOrDefault();
                }

                filterContext.Controller.ViewBag.PathInfo = pathInfo;
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