using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EdiuxTemplateWebApp.Controllers
{
    public abstract class BaseController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public BaseController()
        {

            _userManager = System.Web.HttpContext.Current.GetOwinContext().Get<ApplicationUserManager>();
            _signInManager = System.Web.HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();

        }

        public BaseController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}