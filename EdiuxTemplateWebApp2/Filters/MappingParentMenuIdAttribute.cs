using EdiuxTemplateWebApp.Models.ApplicationLevelModels;
using EdiuxTemplateWebApp.Models.AspNetModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EdiuxTemplateWebApp.Filters
{
    public class MappingParentMenuIdAttribute : ActionFilterAttribute, IActionFilter
    {
        private IMenusRepository _menuRepo;

        public MappingParentMenuIdAttribute() : base()
        {
            _menuRepo = RepositoryHelper.GetMenusRepository();
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            filterContext.Controller.ViewBag.ParentMenuId = new SelectList(_menuRepo.All().Where(w => w.Void == false).ToList(), "Id", "Name");
        }
    }
}