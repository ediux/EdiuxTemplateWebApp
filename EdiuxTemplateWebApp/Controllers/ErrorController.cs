using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EdiuxTemplateWebApp.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index(string actionName,string controllerName)
        {
            return View("Error", new HandleErrorInfo(Server.GetLastError(), actionName, controllerName));
        }
    }
}