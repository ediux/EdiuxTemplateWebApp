using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using System.Web.Routing;
using EdiuxTemplateWebApp.Models.AspNetModels;
using System.Runtime.Caching;

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

                Iaspnet_ApplicationsRepository appRepo = getApplicationInformation(filterContext, ref appInfo);

                string url = getCurrentControllerAndActionUrl(filterContext);

                aspnet_Paths pathInfo;

                Iaspnet_PathsRepository pathRepo = RepositoryHelper.Getaspnet_PathsRepository(appRepo.UnitOfWork);

                if (checkPathIsRegistered(appInfo, url, pathRepo) == false)
                {
                    pathInfo = registerPath(appInfo, url, pathRepo);
                }
                else
                {
                    pathInfo = getPathInformation(appInfo, url, pathRepo);
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

        private static aspnet_Paths registerPath(aspnet_Applications appInfo, string url, Iaspnet_PathsRepository pathRepo)
        {
            aspnet_Paths pathInfo = pathRepo.Add(new aspnet_Paths()
            {
                ApplicationId = appInfo.ApplicationId,
                LoweredPath = url.ToLowerInvariant(),
                Path = url,
                PathId = Guid.NewGuid()
            });
            pathRepo.UnitOfWork.Commit();
            return pathInfo;
        }

        private static aspnet_Paths getPathInformation(aspnet_Applications appInfo, string url, Iaspnet_PathsRepository pathRepo)
        {
            aspnet_Paths pathInfo;
            string loweredUrl = url.ToLowerInvariant();
            pathInfo = pathRepo.Where(w => (w.Path == url || w.LoweredPath == loweredUrl)
			 && w.ApplicationId == appInfo.ApplicationId).SingleOrDefault();
            return pathInfo;
        }

        private static bool checkPathIsRegistered(aspnet_Applications appInfo, string url, Iaspnet_PathsRepository pathRepo)
        {
            return pathRepo.Where(w => w.Path == url && w.ApplicationId == appInfo.ApplicationId).Any();
        }

        private static string getCurrentControllerAndActionUrl(ActionExecutingContext filterContext)
        {
            return UrlHelper.GenerateUrl("Default", filterContext.ActionDescriptor.ActionName, filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                filterContext.RouteData.Values, RouteTable.Routes, filterContext.RequestContext, true);
        }

        private static Iaspnet_ApplicationsRepository getApplicationInformation(ActionExecutingContext filterContext, ref aspnet_Applications appInfo)
        {
            Iaspnet_ApplicationsRepository appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();

            if (filterContext.Controller.ViewBag.ApplicationInfo != null)
            {
                appInfo = filterContext.Controller.ViewBag.ApplicationInfo;
            }
            else
            {
                if (appInfo == null)
                {
                    string applicationName = Startup.getApplicationNameFromConfiguationFile();
                    appInfo = appRepo.FindByName(applicationName).Single();
                    MemoryCache.Default.Add("ApplicationInfo", appInfo, DateTime.UtcNow.AddMinutes(38400));
                }
            }

            return appRepo;
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