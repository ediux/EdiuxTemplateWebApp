using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EdiuxTemplateWebApp.Controllers
{
    public abstract class BaseController : Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
#if !DEBUG
            Elmah.ErrorSignal.FromCurrentContext().Raise(filterContext.Exception);

            if(filterContext.Exception is System.Data.Entity.Validation.DbEntityValidationException)
            {
                filterContext.ExceptionHandled = true;

                filterContext.Result = View("DbEntityValidationException", filterContext.Exception);
   
                return;
            }
#else
            base.OnException(filterContext);
#endif

        }

        protected override void HandleUnknownAction(string actionName)
        {
            base.HandleUnknownAction(actionName);
        }
    }
}