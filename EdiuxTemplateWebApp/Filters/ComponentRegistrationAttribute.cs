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
                EdiuxAspNetSqlUserStore store = ioc.Get<EdiuxAspNetSqlUserStore>();

                var waitResult = store.GetCurrentApplicationInfoAsync();

                if (waitResult.Status != System.Threading.Tasks.TaskStatus.RanToCompletion)
                {
                    waitResult.Wait();
                }

                aspnet_Applications appInfo = store.GetCurrentApplicationInfoAsync().Result;

                string url = GetCurrentControllerAndActionUrl(filterContext);

                aspnet_Paths pathInfo = null;

                if (CheckPathIsRegistered(appInfo, url, ioc) == false)
                {
                    pathInfo = RegisterPath(appInfo, url, filterContext, ioc);
                }
                else
                {
                    pathInfo = GetPathInformation(appInfo, url, ioc);
                }

                filterContext.Controller.ViewBag.PathInfo = pathInfo;
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

        private static aspnet_Paths RegisterPath(aspnet_Applications appInfo, string url, ActionExecutingContext filterContext, IOwinContext owin)
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
                Area = filterContext.RouteData.DataTokens["area"].ToString(),
                ArgumentObject = filterContext.ActionParameters,
                
                AllowAnonymous = true
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

            }
            #endregion

            BaseSetting.AllowExcpetionUsers.Add("root", true);
            BaseSetting.Area = filterContext.RouteData.DataTokens["area"].ToString();
            BaseSetting.ArgumentObject = filterContext.ActionParameters;
            BaseSetting.CommonSettings = new Dictionary<string, object>();
            BaseSetting.ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            BaseSetting.CSS = string.Empty;
            BaseSetting.Description = url;

            if (filterContext.Controller.ViewBag.Title != null)
            {
                BaseSetting.Title = filterContext.Controller.ViewBag.Title as string;
            }
            else
            {
                BaseSetting.Title = "未命名";
            }

            pathInfo.aspnet_PersonalizationAllUsers.PathId = pathInfo.PathId;
            pathInfo.aspnet_PersonalizationAllUsers.LastUpdatedDate = DateTime.UtcNow;
            pathInfo.aspnet_PersonalizationAllUsers.PageSettings = BaseSetting.Serialize();

            #region 建立頁面設定檔

            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                aspnet_PersonalizationPerUser perUserSetting = new aspnet_PersonalizationPerUser();

                perUserSetting.Id = Guid.NewGuid();
                perUserSetting.LastUpdatedDate = DateTime.UtcNow;
                perUserSetting.PathId = pathInfo.PathId;
                perUserSetting.UserId = filterContext.HttpContext.User.Identity.GetUserId();

                PageSettingByUserViewModel perUserModel = new PageSettingByUserViewModel();
                perUserModel.ActionName = filterContext.ActionDescriptor.ActionName;
                perUserModel.AllowAnonymous = false;



                var allowUsers = Users.ToLowerInvariant().Split(',');

                foreach (var u in allowUsers)
                {
                    perUserModel.AllowExcpetionUsers.Add(u, true);
                }

                perUserModel.Area = filterContext.RouteData.DataTokens["area"].ToString();
                perUserModel.ArgumentObject = null;
                perUserModel.CommonSettings = new Dictionary<string, object>();
                perUserModel.ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                perUserModel.CSS = "";
                perUserModel.Description = pathInfo.Path;

                perUserSetting.PageSettings = perUserModel.Serialize();

                pathInfo.aspnet_PersonalizationPerUser.Add(perUserSetting);
            }
            else
            {
                aspnet_PersonalizationPerUser perUserSetting = new aspnet_PersonalizationPerUser();
                perUserSetting.Id = Guid.NewGuid();
                perUserSetting.LastUpdatedDate = DateTime.UtcNow;
                perUserSetting.PathId = pathInfo.PathId;
                perUserSetting.UserId = filterContext.HttpContext.User.Identity.GetUserId();

                PageSettingByUserViewModel perUserModel = new PageSettingByUserViewModel();
                perUserModel.ActionName = filterContext.ActionDescriptor.ActionName;
                perUserModel.AllowAnonymous = false;

                var allowRoles = Roles.ToLowerInvariant().Split(',');

                foreach (var r in allowRoles)
                {
                    perUserModel.AllowExcpetionRoles.Add(r, true);
                }


                var allowUsers = Users.ToLowerInvariant().Split(',');

                foreach (var u in allowUsers)
                {
                    perUserModel.AllowExcpetionUsers.Add(u, true);
                }

                perUserModel.Area = filterContext.RouteData.DataTokens["area"].ToString();
                perUserModel.ArgumentObject = null;
                perUserModel.CommonSettings = new Dictionary<string, object>();
                perUserModel.ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                perUserModel.CSS = "";
                perUserModel.Description = pathInfo.Path;

                perUserSetting.PageSettings = perUserModel.Serialize();

                pathInfo.aspnet_PersonalizationPerUser.Add(perUserSetting);
            }

            #endregion

            Iaspnet_PathsRepository pathRepo = owin.Get<Iaspnet_PathsRepository>();

            pathInfo = pathRepo.Add(pathInfo);
            pathRepo.UnitOfWork.Commit();

            return pathInfo;
        }

        private static aspnet_Paths GetPathInformation(aspnet_Applications appInfo, string url, IOwinContext owin)
        {
            aspnet_Paths pathInfo;
            string loweredUrl = url.ToLowerInvariant();
            var pathRepo = owin.Get<Iaspnet_PathsRepository>();
            pathInfo = pathRepo.Where(w => (w.Path == url || w.LoweredPath == loweredUrl)
             && w.ApplicationId == appInfo.ApplicationId).SingleOrDefault();
            return pathInfo;
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