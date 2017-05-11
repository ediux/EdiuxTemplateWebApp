using EdiuxTemplateWebApp.Models;
using EdiuxTemplateWebApp.Models.AspNetModels;
using EdiuxTemplateWebApp.Models.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EdiuxTemplateWebApp.Filters
{
    public class DbAuthorizeAttribute : AuthorizeAttribute
    {
        private IEdiuxAspNetSqlUserStore Store;

        private IPathStore<aspnet_Paths, Guid> PathStore;
        private ICustomUserStore UserStore;
        private IRoleStore<aspnet_Roles, Guid> RoleStore;
        private IApplicationStore<aspnet_Applications, Guid> AppStore;

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {
                Store = filterContext.HttpContext.GetOwinContext().Get<IEdiuxAspNetSqlUserStore>();

                PathStore = Store;
                UserStore = Store;
                RoleStore = Store;
                AppStore = Store;

                //appRepo = filterContext.HttpContext.GetOwinContext().Get<Iaspnet_ApplicationsRepository>();
                //roleRepo = filterContext.HttpContext.GetOwinContext().Get<Iaspnet_RolesRepository>();
                //userRepo = filterContext.HttpContext.GetOwinContext().Get<Iaspnet_UsersRepository>();

                //roleRepo.UnitOfWork = userRepo.UnitOfWork = appRepo.UnitOfWork;

                string LoweredControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLowerInvariant();
                string LoweredCurrentLoginedUserName = filterContext.HttpContext.User.Identity.GetUserName().ToLowerInvariant();
                bool IsAuthenticated = filterContext.HttpContext.User.Identity.IsAuthenticated;
                var loginUserId = filterContext.RequestContext.HttpContext.User.Identity.GetUserId();

                var GetUserTask = UserStore.GetUserByIdAsync(loginUserId);

                if (GetUserTask.Status != System.Threading.Tasks.TaskStatus.RanToCompletion)
                {
                    GetUserTask.Wait();
                }

                filterContext.Controller.ViewBag.LoginedUser = UserStore.GetUserByIdAsync(loginUserId).Result;
                
                #region 檢核
                if (LoweredControllerName == "error")
                {
                    return;
                }

                if (LoweredControllerName == "elmah")
                {
                    if (IsAuthenticated && LoweredCurrentLoginedUserName == "root")
                    {
                        return;
                    }
                }

                if (IsAuthenticated && LoweredCurrentLoginedUserName == "root")
                {
                    return;
                }

                aspnet_Applications appInfo = AppStore.GetApplicationFromConfiguratinFile();

                if (appInfo == null)
                {
                    Exception customerException = new Exception("Can't fetch application information from database.");

                    filterContext.Controller.TempData["Exception"] = customerException;

                    throw customerException;
                }
                #endregion

                #region 取得目前所在虛擬路徑的資訊
                aspnet_Paths pathInfo = PathStore.GetEntityByQuery(filterContext.RequestContext.HttpContext.Request.Path);

                if (pathInfo != null)
                {
                    #region 檢查是否有目前使用者對應的設定紀錄
                    if (pathInfo.aspnet_PersonalizationPerUser.Any(a => a.aspnet_Users.Id == loginUserId))
                    {
                        //有紀錄
                        //當前使用者的設定檔
                        var pageUserSettings = pathInfo.aspnet_PersonalizationPerUser.Single(a => a.aspnet_Users.Id == loginUserId);

                        //PageSettingByUserViewModel userSettingModel = null;

                        if (pageUserSettings != null)
                        {
                            //pageUserSettings.SettingsserSettingModel = pageUserSettings.PageSettings.Deserialize<PageSettingByUserViewModel>();

                            #region 權限

                            #region 檢查是否匿名存取
                            if (pageUserSettings.Settings.AllowAnonymous)
                            {
                                return;
                            }
                            #endregion

                            #region 檢查是否可以執行

                            var UserPermission = pageUserSettings.Settings.Permission;

                            if (UserPermission != null)
                            {
                                if (UserPermission.ExecuteFeature)
                                {
                                    return;
                                }
                            }
                            #endregion

                            #region 檢查是否在例外角色清單
                            var allowRoles = Roles.ToLowerInvariant().Split(',');

                            var checkIsInRoles = (from e in pageUserSettings.Settings.AllowExcpetionRoles
                                                  from a in allowRoles
                                                  from r in appInfo.aspnet_Roles
                                                  from u in r.aspnet_Users
                                                  where e.Key == a && e.Key == r.LoweredRoleName
                                                  && u.Id == loginUserId
                                                  select e.Key);

                            if (checkIsInRoles.Any())
                            {
                                return;
                            }
                            #endregion

                            #region 檢查是否在例外使用者清單中
                            var allowUsers = Users.ToLowerInvariant().Split(',');
                            var checkIsInUsers = (from eu in pageUserSettings.Settings.AllowExcpetionUsers
                                                  from lu in allowUsers
                                                  from au in appInfo.aspnet_Users
                                                  where eu.Key == lu && au.LoweredUserName == eu.Key
                                                  && au.Id == loginUserId
                                                  select eu);

                            if (checkIsInUsers.Any())
                            {
                                return;
                            }
                            #endregion

                            #endregion
                        }
                    }
                    #endregion

                    #region 檢查是否有共用的設定檔
                    if (pathInfo.aspnet_PersonalizationAllUsers != null)
                    {
                        PageSettingsBaseModel GlobalSettings
                            = pathInfo.aspnet_PersonalizationAllUsers.Settings;

                        var checkSettings = GlobalSettings.AllowExcpetionRoles
                            .Select(s => s.Key).Distinct()
                            .Except(Roles.Split(','));

                        if (GlobalSettings.AllowAnonymous)
                        {
                            return;
                        }

                        var username = filterContext.RequestContext.HttpContext.User.Identity.GetUserName();

                        if (GlobalSettings.AllowExcpetionUsers.ContainsKey(username))
                        {
                            if (GlobalSettings.AllowExcpetionUsers[username])
                                return;
                        }

                        if (GlobalSettings.AllowExcpetionRoles.Any())
                        {
                            var UserInfo = appInfo.aspnet_Users.Where(w => w.Id == loginUserId).Single();
                            var checkRoleCanAccess = (from r in UserInfo.aspnet_Roles
                                                      from e in GlobalSettings.AllowExcpetionRoles
                                                      where r.LoweredRoleName == e.Key.ToLowerInvariant()
                                                      select r).Distinct();

                            if (checkRoleCanAccess.Any())
                            {
                                return;
                            }
                        }
                    }

                    #endregion
                }

                //if (appInfo.aspnet_Paths.Any(a => a.Path == filterContext.RequestContext.HttpContext.Request.Path))
                //{
                //    #region 取出路徑資訊

                //    pathInfo = (from a in appInfo.aspnet_Paths
                //                where a.Path == filterContext.RequestContext.HttpContext.Request.Path
                //                select a).Single();
                //    #endregion
                //}

                #endregion

                base.OnAuthorization(filterContext);

            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);

                filterContext.Controller.TempData["Exception"] = ex;

                if (ex is DbEntityValidationException)
                {
                    filterContext.Controller.TempData["Exception"] = ex;

                    if (filterContext.IsChildAction == false)
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                        {
                            { "controller", "Error" },
                            { "action", "DbEntityValidationError" },
                            { "actionName",filterContext.ActionDescriptor.ActionName},
                            { "controllerName",filterContext.ActionDescriptor.ControllerDescriptor.ControllerName }

                        });
                        return;
                    }

                }

                if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLowerInvariant() == "elmah")
                {
                    if (filterContext.HttpContext.User.Identity.IsAuthenticated && filterContext.HttpContext.User.Identity.Name == "root")
                    {
                        return;
                    }
                }

                if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLowerInvariant() == "error")
                {
                    return;
                }

                if (filterContext.IsChildAction == false)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {
                        { "controller", "Error" },
                        { "action", "Index" },
                        { "actionName",filterContext.ActionDescriptor.ActionName},
                        { "controllerName",filterContext.ActionDescriptor.ControllerDescriptor.ControllerName },
                        { "statusCode", 500 },

                    });
                }

                throw ex;
            }
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