using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EdiuxTemplateWebApp.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;

namespace EdiuxTemplateWebApp.Controllers
{
    public class MenusController : BaseController
    {
        private IMenusRepository _menuRepo;
        private ISystem_ControllerActionsRepository _actionsRepo;

        private ApplicationUserManager _userManager;

        public MenusController()
        {            
            _menuRepo = RepositoryHelper.GetMenusRepository();
            _actionsRepo = RepositoryHelper.GetSystem_ControllerActionsRepository(_menuRepo.UnitOfWork);
        }

        public MenusController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
            _menuRepo = RepositoryHelper.GetMenusRepository(userManager.UnitOfWork);
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Menus
        public ActionResult Index()
        {
            var menus = _menuRepo.All();
            return View(menus.ToList());
        }

        // GET: Menus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Menus menus = _menuRepo.Get(id);
            if (menus == null)
            {
                return HttpNotFound();
            }
            return View(menus);
        }

        // GET: Menus/Create
        public ActionResult Create()
        {
            string appNameSapce = typeof(MvcApplication).Namespace;

            ViewBag.ParentMenuId = new SelectList(_menuRepo.All().Where(w => w.Void == false).ToList(), "Id", "Name");
            ViewBag.System_ControllerActionsId = new SelectList(
                _actionsRepo
                .All()
                .Where(w=>w.System_Controllers.Namespace.Contains(appNameSapce))
               .Select(s => new ControllerActionViewModel() {
                   Id = s.Id,
                   Name = s.Name + "(" + s.System_Controllers.Name + ")"
               }).ToList(), "Id", "Name");
            var newmodel = new Menus();
            ISystem_ApplicationsRepository sysAppRepo = RepositoryHelper.GetSystem_ApplicationsRepository(_menuRepo.UnitOfWork);
            System_Applications app = sysAppRepo.All().SingleOrDefault(s => s.Name == typeof(MvcApplication).Namespace);

            newmodel.ApplicationId = (app != null) ? app.Id : 0;
            return View(newmodel);         
        }

        // POST: Menus/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,IconCSS,IsExternalLinks,ExternalURL,Void,ParentMenuId,CreateUserId,CreateTime,LastUpdateUserId,LastUpdateTime,AllowAnonymous,System_ControllerActionsId,Order,ApplicationId")] Menus menus)
        {

            if (ModelState.IsValid)
            {
                menus.LastUpdateUserId = menus.CreateUserId = User.Identity.GetUserId<int>();
                menus.LastUpdateTime = menus.CreateTime = DateTime.Now;
                menus.Void = false;
               
                _menuRepo.Add(menus);
                _menuRepo.UnitOfWork.Commit();
                return RedirectToAction("Index");
            }
            string appNameSapce = typeof(MvcApplication).Namespace;
            ViewBag.ParentMenuId = new SelectList(_menuRepo
                .All()               
                .Where(w => w.Void == false)
                .ToList(), "Id", "Name");
            //ViewBag.System_ControllerActionsId = new SelectList(_actionRepo.All().Where(w => w.Void == false).ToList(), "Id", "Name");
            ViewBag.System_ControllerActionsId = new SelectList(
                _actionsRepo
                .All()
                .Where(w => w.System_Controllers.Namespace.Contains(appNameSapce))
               .Select(s => new ControllerActionViewModel()
               {
                   Id = s.Id,
                   Name = s.Name + "(" + s.System_Controllers.Name + ")"
               }).ToList(), "Id", "Name");
            return View(menus);

        }

        // GET: Menus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Menus menu = _menuRepo.Get(id);
            ViewBag.ParentMenuId = new SelectList(_menuRepo
                .All()
                .ToList(),
                "Id",
                "Name",
                menu.ParentMenuId);
            //ViewBag.System_ControllerActionsId = new SelectList(_actionRepo.All().Where(w => w.Void == false).ToList(), "Id", "Name");
            ViewBag.System_ControllerActionsId = new SelectList(_actionsRepo.Where(w => w.Void == false)
               .Select(s => new ControllerActionViewModel() {
                   Id = s.Id,
                   Name = s.Name + "(" + s.System_Controllers.Name + ")" })
                   .ToList(),
                   "Id",
                   "Name",
                   menu.System_ControllerActionsId);

            if (menu == null)
            {
                return HttpNotFound();
            }
            return View(menu);
        }

        // POST: Menus/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,IconCSS,IsExternalLinks,ExternalURL,Void,ParentMenuId,CreateUserId,CreateTime,LastUpdateUserId,LastUpdateTime,AllowAnonymous,System_ControllerActionsId,Order")] Menus menus)
        {
            if (ModelState.IsValid)
            {
                this.ApplyXSSProtected(menus);
                Menus menuindb = _menuRepo.Get(menus.Id);

                menuindb.CloneFrom(menus);

                _menuRepo.UnitOfWork.Context.Entry(menuindb).State = EntityState.Modified;
                _menuRepo.UnitOfWork.Commit();

                return RedirectToAction("MenuList");
            }
            ViewBag.ParentMenuId = new SelectList(_menuRepo.All().Where(w => w.Void == false).ToList(), "Id", "Name", menus.ParentMenuId);
            ViewBag.System_ControllerActionsId = new SelectList(_actionsRepo.All().Where(w => w.Void == false)
               .Select(s => new ControllerActionViewModel() {
                   Id = s.Id,
                   Name = s.Name + "(" + s.System_Controllers.Name + ")" })
                   .ToList(), "Id", "Name", menus.System_ControllerActionsId);
            return View(menus);
        }

        // GET: Menus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Menus menus = _menuRepo.Get(id);
            if (menus == null)
            {
                return HttpNotFound();
            }
            return View(menus);
        }

        // POST: Menus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Menus menu = _menuRepo.Get(id);

            menu.Void = true;
            menu.LastUpdateUserId = User.Identity.GetUserId<int>();
            menu.LastUpdateTime = DateTime.UtcNow;

            _menuRepo.UnitOfWork.Context.Entry(menu).State = EntityState.Modified;
            _menuRepo.UnitOfWork.Commit();

            return RedirectToAction("Index");
        }
        
        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult SingInOutShortcutMenu(int? id)
        {
            //登出/登入快速選單
            return PartialView("_LoginPartial");
        }

        [AllowAnonymous]
        [ChildActionOnly]
        [OutputCache(Duration =1800)]
        public ActionResult MenuBar()
        {
            //選單列

            if(ViewBag.ApplicationInfo!=null)
            {
                var appInfo = (System_Applications)ViewBag.ApplicationInfo;
                var currentLoginedUser = appInfo.getUserByName(User.Identity.Name);
         

                if (currentLoginedUser!=null && currentLoginedUser.Length >= 1)
                {
                    return View("_MenuBarPartial", appInfo.getMenusbyCurrentLoginUser(currentLoginedUser.First()).ToList());
                }                
            }

            return View("_MenuBarPartial",new List<Menus>());

        }

        public ActionResult ListMenuInRoles(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            IApplicationRoleRepository roleRepo = RepositoryHelper.GetApplicationRoleRepository(_menuRepo.UnitOfWork);

            Menus menu = _menuRepo.Get(id);
            var roles = roleRepo.getUnSelectedRoles(menu.ApplicationRole);

            ViewBag.RoleId = new SelectList(roles.ToList(), "Id", "Name");
            ViewBag.MenuId = id;

            if (menu == null)
            {
                return HttpNotFound();
            }

            return View(menu.ApplicationRole.Where(w => w.Void == false).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListMenuInRoles(int? id,FormCollection collection)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Menus menu = _menuRepo.Get(id);

            if (string.IsNullOrEmpty(collection["RoleId"]) == false)
            {
                int RoleId = 0;

                if (int.TryParse(collection["RoleId"], out RoleId) == false)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                IApplicationRoleRepository roleRepo = RepositoryHelper.GetApplicationRoleRepository(_menuRepo.UnitOfWork);
                ISystem_ControllersRepository ctrlRepo = RepositoryHelper.GetSystem_ControllersRepository(_menuRepo.UnitOfWork);

                ApplicationRole role = roleRepo.Get(RoleId);

                System_ControllerActions action = _actionsRepo.Get(menu.System_ControllerActionsId ?? 0);

                if (action != null)
                {
                    System_Controllers ctrl = ctrlRepo.Get(action.ControllerId ?? 0);

                    if (ctrl != null)
                    {
                        if (ctrl.System_ControllerActions.Any())
                        {
                            role.System_ControllerActions.Clear();

                            foreach (var item in ctrl.System_ControllerActions)
                            {
                                role.System_ControllerActions.Add(item);
                            }

                        }
                    }
                }

                menu.ApplicationRole.Add(role);

                _menuRepo.UnitOfWork.Context.Entry(role).State = EntityState.Modified;
                _menuRepo.UnitOfWork.Context.Entry(menu).State = EntityState.Modified;
                _menuRepo.UnitOfWork.Commit();
            }

            return RedirectToAction("ListMenuInRoles", new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMenuInRole(int? id, FormCollection collection)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Menus menu = _menuRepo.Get(id);

            if (string.IsNullOrEmpty(collection["RoleId"]) == false)
            {
                int RoleId = 0;

                if (int.TryParse(collection["RoleId"], out RoleId) == false)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                IApplicationRoleRepository roleRepo = RepositoryHelper.GetApplicationRoleRepository(_menuRepo.UnitOfWork);
                ISystem_ControllersRepository ctrlRepo = RepositoryHelper.GetSystem_ControllersRepository(_menuRepo.UnitOfWork);

                ApplicationRole role = roleRepo.Get(RoleId);

                System_ControllerActions action = _actionsRepo.Get(menu.System_ControllerActionsId ?? 0);

                if (action != null)
                {
                    System_Controllers ctrl = ctrlRepo.Get(action.ControllerId ?? 0);

                    if (ctrl != null)
                    {
                        if (ctrl.System_ControllerActions.Any())
                        {
                            role.System_ControllerActions.Clear();

                            foreach (var item in ctrl.System_ControllerActions)
                            {
                                role.System_ControllerActions.Add(item);
                            }

                        }
                    }
                }

                menu.ApplicationRole.Add(role);

                _menuRepo.UnitOfWork.Context.Entry(role).State = EntityState.Modified;
                _menuRepo.UnitOfWork.Context.Entry(menu).State = EntityState.Modified;
                _menuRepo.UnitOfWork.Commit();
            }

            return RedirectToAction("Edit", new { id = id });
        }

        public ActionResult RemoveMenuFromRole(int? id, int? RoleId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (RoleId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Menus menu = _menuRepo.Get(id);

            IApplicationRoleRepository _roleRepo = RepositoryHelper.GetApplicationRoleRepository(_menuRepo.UnitOfWork);
            ISystem_ControllersRepository ctrlRepo = RepositoryHelper.GetSystem_ControllersRepository(_menuRepo.UnitOfWork);

            ApplicationRole role = _roleRepo.Get(RoleId);

            menu.LastUpdateUserId = User.Identity.GetUserId<int>();
            menu.LastUpdateTime = DateTime.Now;
            menu.ApplicationRole.Remove(role);

            System_ControllerActions action = _actionsRepo.Get(menu.System_ControllerActionsId ?? 0);

            if (action != null)
            {
                System_Controllers ctrl = ctrlRepo.Get(action.ControllerId ?? 0);

                if (ctrl != null)
                {
                    if (ctrl.System_ControllerActions.Any())
                    {
                        role.System_ControllerActions.Clear();
                    }
                }
            }

            _menuRepo.UnitOfWork.Context.Entry(role).State = EntityState.Modified;
            _menuRepo.UnitOfWork.Context.Entry(menu).State = EntityState.Modified;
            _menuRepo.UnitOfWork.Commit();

            return RedirectToAction("Edit", new { id = id });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //_menuRepo.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
