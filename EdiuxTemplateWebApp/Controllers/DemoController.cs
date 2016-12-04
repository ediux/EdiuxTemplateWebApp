using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EdiuxTemplateWebApp.Controllers
{
    [AllowAnonymous]
    public class DemoController : Controller
    {
        // GET: Demo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult General()
        {
            return View();
        }

        public ActionResult Buttons()
        {
            return View();
        }

        public ActionResult Tabs()
        {
            return View();
        }

        public ActionResult Accordions()
        {
            return View();
        }

        public ActionResult NestableList()
        {
            return View();
        }
        public ActionResult Grid()
        {
            return View();
        }

        public ActionResult Dialogs()
        {
            return View();
        }

        public ActionResult FormElements()
        {
            return View();
        }

        public ActionResult FormValidation()
        {
            return View();
        }
    }
}