using EdiuxTemplateWebApp.Helpers;
using EdiuxTemplateWebApp.Models;
using EdiuxTemplateWebApp.Models.AspNetModels;
using Microsoft.AspNet.Identity.Owin;
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

        public ApplicationIdentifyAttribute(Type appType)
        {
            _appType = appType; 
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
           
            IApplicationStore<aspnet_Applications,Guid> store = filterContext.HttpContext.GetOwinContext().Get<IEdiuxAspNetSqlUserStore>();

            var getAppTask = store.GetCurrentApplicationInfoAsync();

            if (!getAppTask.IsCompleted)
            {
                getAppTask.Wait();
            }

            aspnet_Applications fromStore = getAppTask.Result;

            if (fromStore == null)
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

            filterContext.Controller.ViewBag.ApplicationInfo = fromStore;
        }
    }
}