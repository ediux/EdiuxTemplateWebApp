using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using System.Web.Routing;
using EdiuxTemplateWebApp.Models.AspNetModels;
using System.Runtime.Caching;
using Microsoft.AspNet.Identity.Owin;
using EdiuxTemplateWebApp.Models;
using Microsoft.Owin;
using EdiuxTemplateWebApp.Models.Identity;
using System.Collections.Generic;

namespace EdiuxTemplateWebApp.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ComponentRegistrationAttribute : ActionFilterAttribute, IActionFilter
    {
        public string Title { get; set; }

        public string CSS { get; set; }

        public string Description { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                //1.取得OWIN的IOC環境
                //2.先取得Store存取服務層
                //3.取得Application資訊
                //4.從Application資訊取得系統已註冊的頁面
                //5.判斷目前所在URL是否已註冊
                //5.1 如已存在，直接返回
                //5.2 如不存在,新增資料到資料庫
                IOwinContext ioc = filterContext.HttpContext.GetOwinContext();
                IEdiuxAspNetSqlUserStore store = ioc.Get<IEdiuxAspNetSqlUserStore>();
                IApplicationStore<aspnet_Applications, Guid> AppStore = store;
                IPageStore<aspnet_Paths, Guid> PathStore = store;
                IProfileStore<PageSettingByUserViewModel, Guid> ProfileStore = store;

                var waitResult = AppStore.GetCurrentApplicationInfoAsync();

                if (waitResult.Status != System.Threading.Tasks.TaskStatus.RanToCompletion)
                {
                    waitResult.Wait();
                }

                var checkIsRegisterTask = PathStore.CheckPathHasRegisteredAsync(filterContext.Controller);

                if (!checkIsRegisterTask.IsCompleted)
                {
                    checkIsRegisterTask.Wait();
                }

                if (checkIsRegisterTask.Result)
                {
                    filterContext.Controller.ViewBag.PathInfo = PathStore.GetAsync(filterContext.Controller);
                }
                else
                {
                    var runRegisterTask = PathStore.RegisterControllerAsync(filterContext.Controller);

                    if (!runRegisterTask.IsCompleted)
                    {
                        runRegisterTask.Wait();
                    }

                    filterContext.Controller.ViewBag.PathInfo = PathStore.GetAsync(filterContext.Controller);
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);

                if (ex is DbEntityValidationException)
                {
                    filterContext.Controller.ViewBag.Exception = ex as DbEntityValidationException;
                }
                else
                {
                    filterContext.Controller.ViewBag.Exception = ex;
                }
                throw ex;
            }

        }

        private aspnet_Paths RegisterPath(aspnet_Applications appInfo, string url, ActionExecutingContext filterContext, IOwinContext owin)
        {
            aspnet_Paths pathInfo = new aspnet_Paths()
            {
                ApplicationId = appInfo.ApplicationId,
                LoweredPath = url.ToLowerInvariant(),
                Path = url,
                PathId = Guid.NewGuid()
            };

            pathInfo.aspnet_PersonalizationAllUsers = new aspnet_PersonalizationAllUsers();

            PageSettingsBaseModel BaseSetting = new PageSettingsBaseModel()
            {
                ActionName = filterContext.ActionDescriptor.ActionName,
                ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                Area = filterContext.RouteData.DataTokens.ContainsKey("area") ? filterContext.RouteData.DataTokens["area"].ToString() : string.Empty,
                ArgumentObject = filterContext.ActionParameters,
                CommonSettings = new Dictionary<string, object>(),
                CSS = CSS,
                Description = Description,
                MenuId = Guid.Empty,
                Title = Title,
                AllowAnonymous = true,
            };

            #region 尋找原始的授權屬性


            var auth = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AuthorizeAttribute), true)
                .Select(s => (AuthorizeAttribute)s).ToList().SingleOrDefault();

            if (auth != null)
            {
                var allowRoles = auth.Roles.ToLowerInvariant().Split(',');

                foreach (var r in allowRoles)
                {
                    BaseSetting.AllowExcpetionRoles.Add(r, true);
                }

                var allowUsers = auth.Users.ToLowerInvariant().Split(',');

                foreach (var u in allowUsers)
                {
                    BaseSetting.AllowExcpetionUsers.Add(u, true);
                }
            }

            #endregion

            #region 尋找自訂的授權屬性
            var adbuth = filterContext.ActionDescriptor.GetCustomAttributes(typeof(DbAuthorizeAttribute), true)
                .Select(s => (DbAuthorizeAttribute)s).ToList().SingleOrDefault();

            if (auth != null)
            {
                var allowRoles = auth.Roles.ToLowerInvariant().Split(',');

                foreach (var r in allowRoles)
                {
                    BaseSetting.AllowExcpetionRoles.Add(r, true);
                }

                var allowUsers = auth.Users.ToLowerInvariant().Split(',');

                foreach (var u in allowUsers)
                {
                    BaseSetting.AllowExcpetionUsers.Add(u, true);
                }
            }
            #endregion

            if (filterContext.Controller.ViewBag.Title != null)
            {
                BaseSetting.Title = filterContext.Controller.ViewBag.Title as string;
            }

            pathInfo.aspnet_PersonalizationAllUsers.PageSettings = BaseSetting.Serialize();
            pathInfo.aspnet_PersonalizationAllUsers.PathId = pathInfo.PathId;
            pathInfo.aspnet_PersonalizationAllUsers.LastUpdatedDate = DateTime.UtcNow;

            #region 建立頁面設定檔
            Guid userId = filterContext.HttpContext.User.Identity.GetUserId();

            aspnet_PersonalizationPerUser perUserSetting = new aspnet_PersonalizationPerUser()
            {
                Id = Guid.NewGuid(),
                LastUpdatedDate = DateTime.UtcNow,
                PathId = pathInfo.PathId,
                UserId = userId
            };

            PageSettingByUserViewModel perUserModel = new PageSettingByUserViewModel(pathInfo.aspnet_PersonalizationAllUsers, userId);

            perUserSetting.PageSettings = perUserModel.Serialize();

            pathInfo.aspnet_PersonalizationPerUser.Add(perUserSetting);

            #endregion

            Iaspnet_PathsRepository pathRepo = owin.Get<Iaspnet_PathsRepository>();

            pathInfo = pathRepo.Add(pathInfo);
            pathRepo.UnitOfWork.Commit();

            return pathInfo;
        }

        private aspnet_Paths GetPathInformation(aspnet_Applications appInfo, string url, ActionExecutingContext filterContext, IOwinContext owin)
        {
          
        }

        private static bool CheckPathIsRegistered(aspnet_Applications appInfo, string url, IOwinContext owin)
        {
            Iaspnet_PathsRepository pathRepo = owin.Get<Iaspnet_PathsRepository>();
            return pathRepo.Where(w => w.Path == url && w.ApplicationId == appInfo.ApplicationId).Any();
        }

        private static string GetCurrentControllerAndActionUrl(ActionExecutingContext filterContext)
        {
            return UrlHelper.GenerateUrl("Default", filterContext.ActionDescriptor.ActionName, filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                filterContext.RouteData.Values, RouteTable.Routes, filterContext.RequestContext, true);
        }

        private static Iaspnet_ApplicationsRepository GetApplicationInformation(ActionExecutingContext filterContext, ref aspnet_Applications appInfo)
        {
            IUnitOfWork uow = filterContext.RequestContext.HttpContext.GetOwinContext().Get<IUnitOfWork>();
            EdiuxAspNetSqlUserStore store = filterContext.RequestContext.HttpContext.GetOwinContext().Get<EdiuxAspNetSqlUserStore>();

            Iaspnet_ApplicationsRepository appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository(uow);

            if (filterContext.Controller.ViewBag.ApplicationInfo != null)
            {
                appInfo = filterContext.Controller.ViewBag.ApplicationInfo;
            }
            else
            {
                if (appInfo == null)
                {
                    var asyncTask = store.GetApplicationNameFromConfiguratinFileAsync();
                    asyncTask.Start();
                    asyncTask.Wait();

                    string applicationName = asyncTask.Result;
                    appInfo = appRepo.FindByName(applicationName).Single();
                    MemoryCache.Default.Add("ApplicationInfo", appInfo, DateTime.UtcNow.AddMinutes(38400));
                }
            }

            return appRepo;
        }

        protected virtual void WriteErrorLog(Exception ex)
        {
            if (System.Web.HttpContext.Current == null)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
            else
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
    }
}