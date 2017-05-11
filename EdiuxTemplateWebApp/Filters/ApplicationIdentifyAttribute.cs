using EdiuxTemplateWebApp.Models;
using EdiuxTemplateWebApp.Models.AspNetModels;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EdiuxTemplateWebApp.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ApplicationIdentifyAttribute : ActionFilterAttribute, IActionFilter
    {
        public ApplicationIdentifyAttribute()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
           
            IApplicationStore<aspnet_Applications,Guid> store = filterContext.HttpContext.GetOwinContext().Get<IEdiuxAspNetSqlUserStore>();

            store.Initialization(filterContext);
            //var getAppTask = store.GetEntityByQueryAsync(store.GetApplicationNameFromConfiguratinFile());

            //if (!getAppTask.IsCompleted)
            //{
            //    getAppTask.Wait();
            //}

            //aspnet_Applications fromStore = getAppTask.Result;

            if (filterContext.Controller.ViewBag.ApplicationInfo == null)
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

            filterContext.Controller.ViewBag.WebSiteName = ConfigHelper.GetConfig("WebSiteName");
        }
    }
}