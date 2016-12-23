using EdiuxTemplateWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EdiuxTemplateWebApp.Filters
{
    public class DbAuthorizeAttribute : AuthorizeAttribute
    {
        private IApplicationRoleRepository roleRepo;
        private IApplicationUserRepository userRepo;
        private ISystem_ControllerActionsRepository actionRepo;
        private ISystem_ControllersRepository ctrlRepo;

        public DbAuthorizeAttribute()
        {
            roleRepo = RepositoryHelper.GetApplicationRoleRepository();
            userRepo = RepositoryHelper.GetApplicationUserRepository(roleRepo.UnitOfWork);
            actionRepo = RepositoryHelper.GetSystem_ControllerActionsRepository(userRepo.UnitOfWork);
            ctrlRepo = RepositoryHelper.GetSystem_ControllersRepository(userRepo.UnitOfWork);
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {
                System_Applications appInfo = null;

                if (filterContext.Controller.ViewBag.ApplicationInfo != null)
                {
                    appInfo = (System_Applications)filterContext.Controller.ViewBag.ApplicationInfo;
                }


                Task<ApplicationUser> currentUserTask = null;

                currentUserTask = userRepo.FindByNameAsync(filterContext.HttpContext.User.Identity.Name);
                currentUserTask.Wait();                

                //加入取得目前登入使用者的資訊
                filterContext.Controller.ViewBag.CurrentLoginedUser = currentUserTask.Result;

                if (actionRepo.IsUserVerified(filterContext,filterContext.ActionDescriptor))
                {
                    return;
                }

                filterContext.Result = new HttpUnauthorizedResult();
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
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