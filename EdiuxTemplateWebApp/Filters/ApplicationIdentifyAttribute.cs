using EdiuxTemplateWebApp.Models.AspNetModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;

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
            aspnet_Applications fromCache = MemoryCache.Default.Get("ApplicatinInfo") as aspnet_Applications;

            string appName = string.Empty;

            if (fromCache == null)
            {
                if (System.Web.Configuration.WebConfigurationManager.AppSettings.AllKeys.Contains("ApplicationName"))
                {
                    appName = System.Web.Configuration.WebConfigurationManager.AppSettings["ApplicationName"];
                }
                else
                {
                    appName = typeof(MvcApplication).Namespace;
                }

                fromCache = new aspnet_Applications();
                fromCache.ApplicationId = Guid.NewGuid();
                fromCache.ApplicationName = appName;
                fromCache.Description = appName;
                fromCache.LoweredApplicationName = appName.ToLowerInvariant();

                fromCache = appRepo.Add(fromCache);
                
                filterContext.Controller.ViewBag.ApplicationInfo = fromCache;

                MemoryCache.Default.Set("ApplicatinInfo", fromCache, DateTime.Now.AddMinutes(30));
            }

            filterContext.Controller.ViewBag.ApplicationInfo = fromCache;
        }
    }
}