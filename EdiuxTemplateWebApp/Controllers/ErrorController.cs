using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EdiuxTemplateWebApp.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index(string actionName,string controllerName,int? statusCode,Exception ex)
        {
            ViewBag.statusCode = statusCode;

            if (ex == null)
                ex = Server.GetLastError();

            return View("Error", new HandleErrorInfo(ex, actionName, controllerName));
        }

        public ActionResult DbEntityValidationError(string actionName, string controllerName)
        {
            ViewBag.statusCode = 500;
            Exception ex = null;

            if (ex == null)
                ex = Server.GetLastError();

            if (ex == null)
                ex = TempData["Exception"] as DbEntityValidationException;

            return View("DbEntityValidationException", new HandleErrorInfo(ex, actionName, controllerName));
        }
    }
}