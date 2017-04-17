using EdiuxTemplateWebApp.Models.AspNetModels;
using EdiuxTemplateWebApp.Models.Identity;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace EdiuxTemplateWebApp.Filters
{
    public class DbAuthorizeAttribute : AuthorizeAttribute
    {
        private Iaspnet_RolesRepository roleRepo;
        private Iaspnet_UsersRepository userRepo;

        private Iaspnet_ApplicationsRepository appRepo;

        public DbAuthorizeAttribute()
        {
            appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();
            roleRepo = RepositoryHelper.Getaspnet_RolesRepository(appRepo.UnitOfWork);
            userRepo = RepositoryHelper.Getaspnet_UsersRepository(roleRepo.UnitOfWork);

        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {
                #region 檢核
                if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLowerInvariant() == "error")
                {
                    return;
                }

                if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLowerInvariant() == "elmah")
                {
                    if (filterContext.HttpContext.User.Identity.IsAuthenticated && filterContext.HttpContext.User.Identity.Name == "root")
                    {
                        return;
                    }
                }

                aspnet_Applications appInfo = filterContext.Controller.getApplicationInfo();

                if (appInfo == null)
                {
                    Exception customerException = new Exception("Can't fetch application information from database.");

                    filterContext.Controller.TempData["Exception"] = customerException;

                    throw customerException;
                }
                #endregion

                #region 取得目前所在虛擬路徑的資訊
                aspnet_Paths pathInfo = null;

                if (appInfo.aspnet_Paths.Any(a => a.Path == filterContext.RequestContext.HttpContext.Request.Path))
                {
                    #region 取出路徑資訊

                    pathInfo = (from a in appInfo.aspnet_Paths
                                where a.Path == filterContext.RequestContext.HttpContext.Request.Path
                                select a).Single();

                    var loginUserId = filterContext.RequestContext.HttpContext.User.Identity.GetUserId();

                    #region 檢查是否有目前使用者對應的設定紀錄
                    if (pathInfo.aspnet_PersonalizationPerUser.Any(a => a.aspnet_Users.Id == loginUserId))
                    {
                        //有紀錄
                        //當前使用者的設定檔
                        var pageUserSettings = pathInfo.aspnet_PersonalizationPerUser.Single(a => a.aspnet_Users.Id == loginUserId);

                        PageSettingByUserViewModel userSettingModel = new PageSettingByUserViewModel(pageUserSettings);

                        #region 權限

                        #region 檢查是否匿名存取
                        if (userSettingModel.AllowAnonymous)
                        {
                            return;
                        }
                        #endregion

                        #region 檢查是否可以執行

                        var UserPermission = userSettingModel.Permission;

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

                        var checkIsInRoles = (from e in userSettingModel.AllowExcpetionRoles
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
                        var checkIsInUsers = (from eu in userSettingModel.AllowExcpetionUsers
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
                    #endregion

                    #region 檢查是否有共用的設定檔
                    if (pathInfo.aspnet_PersonalizationAllUsers != null)
                    {
                        PageSettingsBaseModel GlobalSettings
                            = pathInfo.aspnet_PersonalizationAllUsers.PageSettings.Deserialize<PageSettingsBaseModel>();

                        var checkSettings = GlobalSettings.AllowExcpetionRoles
                            .Select(s => s.Key)
                            .Except(Roles.Split(','));

                        if (checkSettings.Any())
                        {
                            foreach (var adddiff in checkSettings)
                            {
                                GlobalSettings.AllowExcpetionRoles.Add(adddiff, true);
                            }

                            GlobalSettings.AllowAnonymous = false;
                        }

                        if (GlobalSettings.AllowAnonymous)
                        {
                            return;
                        }

                        var username = filterContext.RequestContext.HttpContext.User.Identity.GetUserName();

                        if (GlobalSettings.AllowExcpetionUsers[username])
                        {
                            return;
                        }

                        if (GlobalSettings.AllowExcpetionRoles.Any())
                        {
                            var UserInfo = appInfo.aspnet_Users.Where(w => w.Id == loginUserId).Single();
                            var checkRoleCanAccess = (from r in UserInfo.aspnet_Roles
                                                      from e in GlobalSettings.AllowExcpetionRoles
                                                      where r.LoweredRoleName == e.Key.ToLowerInvariant()
                                                      select r);
                            if (checkRoleCanAccess.Any())
                            {
                                return;
                            }
                        }
                    }
   
                    #endregion

                    #endregion
                }

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