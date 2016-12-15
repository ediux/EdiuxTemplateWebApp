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
    }
}