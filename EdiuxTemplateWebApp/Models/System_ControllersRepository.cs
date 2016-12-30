using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using EdiuxTemplateWebApp.Models;

namespace EdiuxTemplateWebApp.Models
{
    public partial class System_ControllersRepository : EFRepository<System_Controllers>, ISystem_ControllersRepository
    {
        public System_Controllers ComponentRegistration(Type ControllerType)
        {
            try
            {
                if (ControllerType == null)
                    throw new ArgumentNullException(nameof(ControllerType));

                System_Controllers ctrcls = new Models.System_Controllers();

                ctrcls.ClassName = ControllerType.Name;
                ctrcls.Name = ControllerType.Name.Replace("Controller", "");
                ctrcls.CreateTime = DateTime.Now;
                ctrcls.CreateUserId = 0;
                ctrcls.LastUpdateTime = DateTime.Now;
                ctrcls.LastUpdateUserId = 0;
                ctrcls.Namespace = ControllerType.Namespace;
                ctrcls.Void = false;
                ctrcls.AllowAnonymous = ControllerType.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();


                Add(ctrcls);
                return ctrcls;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
        }

        //public bool IsUserVerified(ControllerContext filterContent, ActionDescriptor actionDesc)
        //{
        //    System_Applications appInfo = null;
        //    ApplicationUser currentLoginedUser = null;

        //    if (filterContent.Controller.ViewBag.ApplicationInfo != null)
        //    {
        //        appInfo = (System_Applications)filterContent.Controller.ViewBag.ApplicationInfo;
        //    }
        //    else
        //    {
        //        return false;
        //    }

        //    if (filterContent.Controller.ViewBag.CurrentLoginedUser != null)
        //    {
        //        currentLoginedUser = (ApplicationUser)filterContent.Controller.ViewBag.CurrentLoginedUser;
        //    }
        //    else
        //    {
        //        int currentLoginedUserId = filterContent.HttpContext.User.Identity.GetUserId<int>();
        //        IApplicationUserRepository userRepo = RepositoryHelper.GetApplicationUserRepository(UnitOfWork);
        //        currentLoginedUser = userRepo.Get(currentLoginedUserId);
        //    }
        //    string controllerName = actionDesc.ControllerDescriptor.ControllerName;
        //    string controllerClassName = actionDesc.ControllerDescriptor.ControllerType.Name;
        //    string controllerNamespace = actionDesc.ControllerDescriptor.ControllerType.Namespace;

        //    System_Controllers currentControllerClassInfo;

        //    currentControllerClassInfo = All().SingleOrDefault(w => w.Name == controllerName && w.ClassName == controllerClassName && w.Namespace == controllerNamespace);

        //    if (currentControllerClassInfo == null)
        //    {
        //        filterContent.Controller.ViewBag.HasControllerAuthorized = false;
        //        filterContent.Controller.ViewBag.HasAuthorized = false;
        //        return false;
        //    }

        //    if (actionDesc.ControllerDescriptor.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any())
        //    {
        //        filterContent.Controller.ViewBag.HasControllerAuthorized = true;
        //        filterContent.Controller.ViewBag.HasAuthorized = true;
        //        return true;
        //    }
        //}

        public void ScanForComponentRegistration(Type WebAppType)
        {
            ISystem_ControllerActionsRepository actionRepo = RepositoryHelper.GetSystem_ControllerActionsRepository(UnitOfWork);
            var Controllers = WebAppType.Assembly.GetTypes().Where(s => s.BaseType == typeof(Controller) || s.BaseType == typeof(Controllers.BaseController));

            if (ObjectSet.Count() == 0)
            {
                //資料表為空

                if (Controllers.Any())
                {
                    foreach (var ctrl in Controllers)
                    {
                        ComponentRegistration(ctrl);
                    }

                    UnitOfWork.Commit();
                }
            }
            if (Controllers.Any())
            {
                foreach (var ctrl in Controllers)
                {
                    try
                    {
                        actionRepo.ScanForComponentRegistration(ctrl, All().SingleOrDefault(s => s.ClassName == ctrl.Name));
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    }

                }
            }
        }
    }

    public partial interface ISystem_ControllersRepository : IRepositoryBase<System_Controllers>
    {
        void ScanForComponentRegistration(Type WebAppType);
        System_Controllers ComponentRegistration(Type ControllerType);
        //bool IsUserVerified(ControllerContext filterContent, ActionDescriptor actionDesc);
    }
}