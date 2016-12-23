using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EdiuxTemplateWebApp.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ApplicationIdentifyAttribute : ActionFilterAttribute, IActionFilter
    {
        private Type _appType;
        private Models.ISystem_ApplicationsRepository appRepo;

        public ApplicationIdentifyAttribute(Type appType)
        {
            _appType = appType;
            appRepo = Models.RepositoryHelper.GetSystem_ApplicationsRepository();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string appNamespace = _appType.Namespace;

            Models.System_Applications appInfo = appRepo.All().SingleOrDefault(w => w.Namespace == appNamespace);

            if (appInfo == null)
            {
                appInfo = new Models.System_Applications() { Id = 0, Namespace = appNamespace, Name = _appType.Name, LoweredName = _appType.Name.ToLowerInvariant(), Description = "" };
                appRepo.Add(appInfo);
                appRepo.UnitOfWork.Commit();
                appInfo = appRepo.Reload(appInfo);               
            }
            
            filterContext.Controller.ViewBag.ApplicationInfo = appInfo;
        }
    }
}