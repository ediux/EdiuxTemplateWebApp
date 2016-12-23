using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Reflection;

namespace EdiuxTemplateWebApp.Models
{
    public partial class System_ControllerActionsRepository : EFRepository<System_ControllerActions>, ISystem_ControllerActionsRepository
    {
        public System_ControllerActions ComponentRegistration( System_Controllers Controller, MethodBase action)
        {

            try
            {
                if (Controller == null)
                    throw new ArgumentNullException(nameof(Controller));

                if (action == null)
                    throw new ArgumentNullException(nameof(action));

                System_ControllerActions actiondata = new Models.System_ControllerActions();
                actiondata.ControllerId = Controller.Id;
                actiondata.CreateTime = DateTime.Now;
                actiondata.CreateUserId = 0;
                actiondata.LastUpdateTime = DateTime.Now;
                actiondata.LastUpdateUserId = 0;
                actiondata.Name = action.Name;
                actiondata.Void = false;
                actiondata.AllowAnonymous = action.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();
               
                actiondata = Add(actiondata);

                return actiondata;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public System_ControllerActions ComponentRegistration(System_Controllers Controller, ActionDescriptor action)
        {
            try
            {
                if (Controller == null)
                    throw new ArgumentNullException(nameof(Controller));

                if (action == null)
                    throw new ArgumentNullException(nameof(action));

                System_ControllerActions actiondata = new Models.System_ControllerActions();
                actiondata.ControllerId = Controller.Id;
                actiondata.CreateTime = DateTime.Now;
                actiondata.CreateUserId = 0;
                actiondata.LastUpdateTime = DateTime.Now;
                actiondata.LastUpdateUserId = 0;
                actiondata.Name = action.ActionName;
                actiondata.Void = false;
                actiondata.AllowAnonymous = action.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();

                actiondata = Add(actiondata);

                return actiondata;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }

        }

        public bool IsUserVerified(ControllerContext filterContent, ActionDescriptor actionDesc)
        {
            System_Applications appInfo = null;
            ApplicationUser currentLoginedUser = null;

            if (filterContent.Controller.ViewBag.ApplicationInfo != null)
            {
                appInfo = (System_Applications)filterContent.Controller.ViewBag.ApplicationInfo;
            }
            else
            {
                return false;
            }

            if (filterContent.Controller.ViewBag.CurrentLoginedUser != null)
            {
                currentLoginedUser = (ApplicationUser)filterContent.Controller.ViewBag.CurrentLoginedUser;
            }
            else
            {
                int currentLoginedUserId = getCurrentLoginedUserId();
                
                return false;
            }

            //取得目前的控制器方法
            string actionName = actionDesc.ActionName;
            string controllerName = actionDesc.ControllerDescriptor.ControllerName;
            string controllerClassName = actionDesc.ControllerDescriptor.ControllerType.Name;
            string controllerNamespace = actionDesc.ControllerDescriptor.ControllerType.Namespace;

            //在資料庫中尋找動作方法資訊
            IQueryable<System_ControllerActions> actions = All().Where(s => s.Name == actionName);

            if (actions.Any())
            {
                IQueryable<ApplicationRole> roles = actions.SelectMany(s => s.ApplicationRole).Distinct();

                if (roles.Any())
                {
                    IQueryable<ApplicationUser> users = roles.SelectMany(w => w.ApplicationUser).Distinct();

                    if(users.Where(w=>w.Id == currentLoginedUser.Id).Any())
                    {
                        return true;
                    }
                }

                if (actions.Any(w => w.AllowAnonymous == true))
                {
                    return true;
                }
            }

            ISystem_ControllersRepository ctrlRepo = RepositoryHelper.GetSystem_ControllersRepository(UnitOfWork);

            if (ctrlRepo.IsUserVerified(filterContent, actionDesc))
            {
                return true;
            }
         
            //&& s.System_Controllers.ClassName.Equals(controllerClassName)
            //&& s.System_Controllers.Namespace.Equals(controllerNamespace));

            if (action != null)
            {
                //如果有授權這個角色使用動作
                if (action.ApplicationRole.SelectMany(s => s.ApplicationUser)
                    .Where(s => s.Id == currentLoginedUser.Id).Any())
                    return true;

                if (action.AllowAnonymous)
                    return;

                if (action.System_Controllers.AllowAnonymous)
                    return;

                //取得可以使用控制器方法的使用者(依據角色)
                IEnumerable<ApplicationUser> hasAuthorizedUsers = action.ApplicationRole.SelectMany(s => s.ApplicationUser);

                if (currentUserTask.Result != null)
                {
                    ApplicationUser currentUser = currentUserTask.Result;

                    if (hasAuthorizedUsers.Any(w => w.Id == currentUser.Id))
                    {
                        //如果有找到該使用者授權關聯
                        return;
                    }

                    if (currentUser.ApplicationRole != null)
                    {
                        //檢查角色授權(選單)
                        if (currentUser.ApplicationRole.Count > 0)
                        {

                            if (action.Menus != null)
                            {
                                if (action.Menus
                                    .SelectMany(s => s.ApplicationRole)
                                    .Where(w => w.ApplicationUser.Where(s => s.Id == currentUserTask.Id).Any()).Any())
                                {
                                    return;
                                }

                                var userMenus = currentUserTask.Result.ApplicationRole
                                   .SelectMany(s => s.Menus);

                                if (userMenus != null && userMenus.Any())
                                {
                                    userMenus = userMenus.Where(w => w.System_ControllerActions != null);
                                    if (userMenus
                                           .Where(w => w.System_ControllerActions.Name == actionName
                                           && w.System_ControllerActions.System_Controllers.ClassName == controllerClassName
                                           && w.System_ControllerActions.System_Controllers.Namespace == controllerNamespace)
                                           .Any(m => m.AllowAnonymous == true))
                                    {
                                        return;
                                    }
                                }

                            }

                        }
                    }
                }

            }

            if (actionDesc.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any())
                return true;

            if (actionDesc.ControllerDescriptor.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any())
                return true;

            if (filterContent.HttpContext.User.Identity.Name.Equals("root", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public void ScanForComponentRegistration(Type ControllerType, System_Controllers Controller)
        {
            try
            {
                if (ControllerType == null)
                    throw new ArgumentNullException(nameof(ControllerType));

                System_Controllers ctrcls = Controller;

                if (ctrcls == null)
                    throw new ArgumentNullException(nameof(Controller));

                var actions = ControllerType.GetMethods();

                if (actions.Any())
                {
                    actions = actions.Where(w =>
                    w.ReturnType == typeof(ActionResult)
                    || w.ReturnType == typeof(Task)
                    || w.ReturnType == typeof(Task<ActionResult>)
                    || w.ReturnType == typeof(Task<JsonResult>)
                    || w.ReturnType == typeof(JsonResult)).ToArray();

                    foreach (var action in actions)
                    {
                        try
                        {
                            if (action.GetCustomAttributes(true).OfType<HttpPostAttribute>().Any())
                                continue;

                            if (Where(w => w.Name == action.Name && w.System_Controllers.Id == ctrcls.Id).Any())
                                continue;

                            System_ControllerActions actiondata = ComponentRegistration(ctrcls, action);

                        }
                        catch (Exception ex)
                        {
                            WriteErrorLog(ex);
                            continue;
                        }

                    }

                    UnitOfWork.Commit();
                }

            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
            }
        }
    }

    public partial interface ISystem_ControllerActionsRepository : IRepositoryBase<System_ControllerActions>
    {
        void ScanForComponentRegistration(Type ControllerType, System_Controllers Controller);
        System_ControllerActions ComponentRegistration( System_Controllers Controller, ActionDescriptor action);
        System_ControllerActions ComponentRegistration( System_Controllers Controller, MethodBase action);

        bool IsUserVerified(ControllerContext filterContent, ActionDescriptor actionDesc);
    }
}