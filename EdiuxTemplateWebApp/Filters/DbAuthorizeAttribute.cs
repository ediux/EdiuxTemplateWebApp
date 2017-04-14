using EdiuxTemplateWebApp.Models;
using EdiuxTemplateWebApp.Models.AspNetModels;
using EdiuxTemplateWebApp.Models.Identity;
using EdiuxTemplateWebApp.Models.Shared;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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

                    filterContext.Controller.TempData.Add("Exception", customerException);

                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                        {
                            { "controller", "Error" },
                            { "action", "Index" },
                            { "actionName",filterContext.ActionDescriptor.ActionName},
                            { "controllerName",filterContext.ActionDescriptor.ControllerDescriptor.ControllerName },
                            { "statusCode", System.Net.HttpStatusCode.BadRequest }

                        });

                    WriteErrorLog(customerException);
                    return;
                }

                #region 取得目前所在虛擬路徑的資訊

                if (appInfo.aspnet_Paths.Any(a => a.Path == filterContext.RequestContext.HttpContext.Request.Path))
                {
                    #region 取出路徑資訊
                    var pagePathInformation = appInfo.aspnet_Paths.Single(a => a.Path == filterContext.RequestContext.HttpContext.Request.Path);

                    var loginUserId = filterContext.RequestContext.HttpContext.User.Identity.GetUserId();

                    #region 檢查是否有目前使用者對應的設定紀錄
                    if (pagePathInformation.aspnet_PersonalizationPerUser.Any(a => a.aspnet_Users.Id == loginUserId))
                    {
                        //有紀錄

                        //當前使用者的設定檔
                        var pageUserSettings = pagePathInformation.aspnet_PersonalizationPerUser.Single(a => a.aspnet_Users.Id == loginUserId);

                        PageSettingByUserViewModel userSettingModel = new PageSettingByUserViewModel(pageUserSettings);

                        #region 檢查是否匿名存取
                        if (userSettingModel.AllowAnonymous)
                        {
                            return;
                        }
                        #endregion

                        #region 權限
                        var UserPermission = userSettingModel.Permission;

                        if (UserPermission != null)
                        {
                            if (UserPermission.ExecuteFeature || UserPermission.IsErrorPage || UserPermission.SharedView)
                            {
                                return;
                            }
                        }
                        else
                        {
                            UserPermission = new PagePermissionForUserModel();

                            UserPermission.ExecuteFeature = false;
                            UserPermission.ReadData = false;

                            userSettingModel.Permission = UserPermission;

                            pageUserSettings.PageSettings = userSettingModel.Serialize();
                            pageUserSettings.LastUpdatedDate = DateTime.UtcNow;

                            Iaspnet_PersonalizationPerUserRepository PersonalizationPerUserRepo =
                                RepositoryHelper.Getaspnet_PersonalizationPerUserRepository();

                            PersonalizationPerUserRepo.Update(pageUserSettings);
                            PersonalizationPerUserRepo.UnitOfWork.Commit();

                        }
                        #endregion

                    }
                    else
                    {
                        PageSettingByUserViewModel userSettingModel = new PageSettingByUserViewModel();

                        userSettingModel.Permission.ExecuteFeature = false;
                        userSettingModel.Permission.ReadData = false;

                        //未建立
                        var pageUserSettings = new aspnet_PersonalizationPerUser();

                        pageUserSettings.Id = Guid.NewGuid();
                        pageUserSettings.LastUpdatedDate = DateTime.UtcNow;
                        pageUserSettings.PathId = pagePathInformation.PathId;
                        pageUserSettings.PageSettings = userSettingModel.Serialize();
                        pageUserSettings.UserId = loginUserId;

                        Iaspnet_PersonalizationPerUserRepository PersonalizationPerUserRepo =
                             RepositoryHelper.Getaspnet_PersonalizationPerUserRepository();

                        PersonalizationPerUserRepo.Add(pageUserSettings);
                        PersonalizationPerUserRepo.UnitOfWork.Commit();


                    }
                    #endregion

                    #region 檢查是否有共用的設定檔
                    if (pagePathInformation.aspnet_PersonalizationAllUsers != null)
                    {
                        PageSettingsBaseModel GlobalSettings
                            = pagePathInformation.aspnet_PersonalizationAllUsers.PageSettings.Deserialize<PageSettingsBaseModel>();

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
                    else
                    {
                        if (pagePathInformation.aspnet_PersonalizationAllUsers == null)
                        {
                            var aspnet_PersonalizationAllUsers = new aspnet_PersonalizationAllUsers();
                            aspnet_PersonalizationAllUsers.LastUpdatedDate = DateTime.UtcNow;
                            aspnet_PersonalizationAllUsers.PageSettings = (new PageSettingsBaseModel()).Serialize();
                            aspnet_PersonalizationAllUsers.PathId = pagePathInformation.PathId;

                            Iaspnet_PersonalizationAllUsersRepository allUserRepo = RepositoryHelper.Getaspnet_PersonalizationAllUsersRepository();
                            allUserRepo.Add(aspnet_PersonalizationAllUsers);
                            allUserRepo.UnitOfWork.Commit();

                        }
                    }
                    #endregion

                    #endregion
                }

                #endregion

                if (filterContext.ActionDescriptor.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any())
                {
                    return;
                }

                if (filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any())
                {
                    return;
                }

                filterContext.Result = new HttpUnauthorizedResult();
                return;

            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);

                if (ex is DbEntityValidationException)
                {
                    filterContext.Controller.TempData["Exception"] = ex;

                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {
                        { "controller", "Error" },
                        { "action", "DbEntityValidationError" },
                        { "actionName",filterContext.ActionDescriptor.ActionName},
                        { "controllerName",filterContext.ActionDescriptor.ControllerDescriptor.ControllerName }

                    });

                    return;
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
                filterContext.Controller.TempData["Exception"] = ex;
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "Error" },
                    { "action", "Index" },
                    { "actionName",filterContext.ActionDescriptor.ActionName},
                    { "controllerName",filterContext.ActionDescriptor.ControllerDescriptor.ControllerName },
                    { "statusCode", 500 },

                });
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