using EdiuxTemplateWebApp.Models.AspNetModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EdiuxTemplateWebApp.Filters
{
    public class MappingPathIdAttribute : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            aspnet_Applications appInfo = filterContext.Controller.ViewBag.ApplicationInfo as aspnet_Applications;

            filterContext.Controller.ViewBag.PathId = new SelectList(
                appInfo.aspnet_Paths
               .Select(s => new
               {
                   Id = s.PathId,
                   Path = s.Path
               }).ToList(), "Id", "Path");
        }
    }
}