using EdiuxTemplateWebApp.Models;
using EdiuxTemplateWebApp.Models.AspNetModels;
using EdiuxTemplateWebApp.Models.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OriginalHttpContext = System.Web.HttpContext;
namespace EdiuxTemplateWebApp.Controllers
{
    public abstract class BaseController : Controller
    {

        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        private IEdiuxAspNetSqlUserStore store;
        private IOwinContext owin;

        private aspnet_Users _loginUser;

        public BaseController()
        {
            owin = OriginalHttpContext.Current.GetOwinContext();
            store = owin.Get<IEdiuxAspNetSqlUserStore>();
        }

        public BaseController(ApplicationUserManager userManager, ApplicationRoleManager roleManager) : this()
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public BaseController(ApplicationUserManager userManager) : this()
        {
            UserManager = userManager;
            RoleManager = owin.Get<ApplicationRoleManager>();
        }

        protected IOwinContext OwinContext
        {
            get
            {
                return owin;
            }
        }

        /// <summary>
        /// 取得目前註冊的應用程式資訊物件。
        /// </summary>
        internal aspnet_Applications ApplicationInformation
        {
            get
            {
                var applicationData = ViewBag.ApplicationInfo as aspnet_Applications;

                if (applicationData == null)
                {
                    applicationData = store.GetCurrentApplicationInfoAsync().Result;
                }

                return applicationData;
            }
        }

        /// <summary>
        /// 取得目前網頁路徑資訊物件。
        /// </summary>
        internal aspnet_Paths PagePathInformation
        {
            get
            {
                var currentPathInfo = ViewBag.PathInfo as aspnet_Paths;

                if (currentPathInfo == null)
                {
                    currentPathInfo = (from path in ApplicationInformation.aspnet_Paths
                                       where path.LoweredPath == Request.Path
                                       select path).SingleOrDefault();
                    
                }

                return currentPathInfo;
            }
        }

        /// <summary>
        /// 取得目前系統的使用者管理員物件。
        /// </summary>
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? owin.GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        /// <summary>
        /// 取得目前系統的角色管理員物件。
        /// </summary>
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? owin.Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public IEdiuxAspNetSqlUserStore Store
        {
            get
            {
                return store;
            }
        }

        internal aspnet_Users LoginedUser
        {
            get
            {
                if (_loginUser == null)
                {
                    _loginUser = store.GetUserByIdAsync(User.Identity.GetUserId()).Result;
                }

                return _loginUser;
            }

        }

        private PageSettingByUserViewModel pageSettings;

        public PageSettingByUserViewModel PageSettings
        {
            get
            {
                if (pageSettings == null)
                {
                    var settingData = (from settings in PagePathInformation.aspnet_PersonalizationPerUser
                                       where settings.UserId == _loginUser.Id
                                       select settings).SingleOrDefault();

                    if (settingData != null)
                    {
                        pageSettings = settingData.PageSettings.Deserialize<PageSettingByUserViewModel>();
                        return pageSettings;
                    }

                    pageSettings = new PageSettingByUserViewModel(PagePathInformation.aspnet_PersonalizationAllUsers, LoginedUser.Id);
                }

                return pageSettings;

            }
        }

        public virtual void SaveProfile()
        {
            var settingData = (from settings in PagePathInformation.aspnet_PersonalizationPerUser
                               where settings.UserId == _loginUser.Id
                               select settings).SingleOrDefault();

            if (settingData != null)
            {
                settingData.PageSettings = pageSettings.Serialize();
                settingData.LastUpdatedDate = DateTime.UtcNow;

                store.UpdateAsync(ApplicationInformation);
            }
            else
            {
                settingData = new aspnet_PersonalizationPerUser();
                settingData.Id = Guid.NewGuid();
                settingData.LastUpdatedDate = DateTime.UtcNow;
                settingData.PageSettings = PageSettings.Serialize();
                settingData.PathId = PagePathInformation.PathId;
                settingData.UserId = _loginUser.Id;

                PagePathInformation.aspnet_PersonalizationPerUser.Add(settingData);

                Iaspnet_PathsRepository pathRepo = owin.Get<Iaspnet_PathsRepository>();
                pathRepo.Update(PagePathInformation); 
            }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
#if !DEBUG
            Elmah.ErrorSignal.FromCurrentContext().Raise(filterContext.Exception);

            if (filterContext.Exception is System.Data.Entity.Validation.DbEntityValidationException)
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
#if !DEBUG

#else
            base.HandleUnknownAction(actionName);
#endif
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


            }

            base.Dispose(disposing);
        }
    }
}