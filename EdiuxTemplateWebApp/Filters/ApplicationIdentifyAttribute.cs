using EdiuxTemplateWebApp.Models.AspNetModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EdiuxTemplateWebApp.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ApplicationIdentifyAttribute : ActionFilterAttribute, IActionFilter
    {
        private Type _appType;
        private Iaspnet_ApplicationsRepository appRepo;

        public ApplicationIdentifyAttribute(Type appType)
        {
            _appType = appType;
            appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            aspnet_Applications fromCache = MemoryCache.Default.Get(Startup.ApplicationInfoKey) as aspnet_Applications;

            if (fromCache == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {
                        { "controller", "Error" },
                        { "action", "Index" },
                        { "actionName",filterContext.ActionDescriptor.ActionName},
                        { "controllerName",filterContext.ActionDescriptor.ControllerDescriptor.ControllerName },
                    });
                return;
            }

            filterContext.Controller.ViewBag.ApplicationInfo = fromCache;
        }
    }
}