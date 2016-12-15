using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using EdiuxTemplateWebApp.Models;

namespace EdiuxTemplateWebApp.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ComponentRegistrationAttribute : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                Type ControllerType = filterContext.Controller.GetType();

                string appName = ControllerType.Assembly.GetName().Name;
                string ctrlName = ControllerType.Name;
                string namespaceName = ControllerType.Namespace;

                ISystem_ApplicationsRepository appRepo = RepositoryHelper.GetSystem_ApplicationsRepository();

                ISystem_ControllersRepository CtrRepo = RepositoryHelper.GetSystem_ControllersRepository(appRepo.UnitOfWork);

                ISystem_ControllerActionsRepository ActionRepo = RepositoryHelper.GetSystem_ControllerActionsRepository(CtrRepo.UnitOfWork);

                int currentUserId = filterContext.HttpContext.User.Identity.GetUserId<int>();

                System_Applications app = null;

                app = appRepo.All().SingleOrDefault(w => w.Name.Equals(appName, StringComparison.InvariantCultureIgnoreCase));

                if (app == null)
                {
                    app = new System_Applications();
                    app.Name = appName;
                    app = appRepo.Add(app);
                    appRepo.UnitOfWork.Commit();
                    app = appRepo.Reload(app);
                }

                System_Controllers ctrl = null;

                ctrl = CtrRepo.All().FirstOrDefault(w =>
                w.Namespace.Equals(namespaceName, StringComparison.InvariantCultureIgnoreCase)
                && w.ClassName.Equals(ctrlName, StringComparison.InvariantCultureIgnoreCase));

                if (ctrl == null)
                {
                    ctrl = CtrRepo.ComponentRegistration(ControllerType);   //元件註冊
                    CtrRepo.UnitOfWork.Commit();
                    ctrl = CtrRepo.Reload(ctrl);
                }

                string actionname = filterContext.ActionDescriptor.ActionName;

                System_ControllerActions action = ActionRepo.All().FirstOrDefault(
                    w => w.Name.Equals(actionname, StringComparison.InvariantCultureIgnoreCase)
                    && w.ControllerId == ctrl.Id);

                if (action == null)
                {
                    action = ActionRepo.ComponentRegistration(ctrl, filterContext.ActionDescriptor);
                    ActionRepo.UnitOfWork.Commit();
                    action = ActionRepo.Reload(action);
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
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

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ApiComponentRegistrationAttribut : FilterAttribute, System.Web.Http.Filters.IFilter
    {
    }
}