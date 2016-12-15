using System;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace EdiuxTemplateWebApp.Filters
{
    public class ElmahErrorAttribute : ExceptionFilterAttribute
    {

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception != null)
                Elmah.ErrorSignal.FromCurrentContext().Raise(actionExecutedContext.Exception);

            base.OnException(actionExecutedContext);
        }
    }
}