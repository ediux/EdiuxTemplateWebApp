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
            object fromCache = appRepo.UnitOfWork.Get("ApplicationInfo");

            if (fromCache == null)
            {
                fromCache = appRepo.getInfoByType(filterContext.Controller.GetType());
                appRepo.UnitOfWork.Set("ApplicatinInfo", fromCache, 30);
            }

            filterContext.Controller.ViewBag.ApplicationInfo = fromCache;
        }
    }
}