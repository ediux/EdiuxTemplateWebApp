using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using EdiuxTemplateWebApp.Models.AspNetModels;
using EdiuxTemplateWebApp.Helpers;
using EdiuxTemplateWebApp.Filters;

namespace EdiuxTemplateWebApp.Controllers
{
    public class MenusController : BaseController
    {
        private IMenusRepository menuRepo;
        private Iaspnet_ApplicationsRepository appRepo;
        private Iaspnet_PathsRepository pathRepo;

        private ApplicationUserManager _userManager;
        private aspnet_Applications appInfo;
        public MenusController()
        {
            appInfo = this.getApplicationInfo();
            appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();
            menuRepo = RepositoryHelper.GetMenusRepository(appRepo.UnitOfWork);
            pathRepo = RepositoryHelper.Getaspnet_PathsRepository(appRepo.UnitOfWork);
        }

        public MenusController(ApplicationUserManager userManager) : this()
        {
            UserManager = userManager;
            menuRepo = RepositoryHelper.GetMenusRepository();
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
            var menus = menuRepo.All();
            return View(menus.ToList());
        }

        // GET: Menus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Menus menus = menuRepo.Get(id);
            if (menus == null)
            {
                return HttpNotFound();
            }
            return View(menus);
        }

        [MappingPathId]
        [MappingParentMenuId]
        // GET: Menus/Create
        public ActionResult Create()
        {
            string appNameSapce = typeof(MvcApplication).Namespace;
            var newmodel = new Menus();
            newmodel.ApplicationId = (appInfo != null) ? appInfo.ApplicationId : Guid.Empty;
            return View(newmodel);
        }

        // POST: Menus/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [MappingPathId]
        [MappingParentMenuId]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,IconCSS,IsExternalLinks,ExternalURL,Void,ParentMenuId,CreateUserId,CreateTime,LastUpdateUserId,LastUpdateTime,AllowAnonymous,System_ControllerActionsId,Order,ApplicationId")] Menus menus)
        {
            
            if (ModelState.IsValid)
            {
                menus.LastUpdateUserId = menus.CreateUserId = User.Identity.GetUserGuid();
                menus.LastUpdateTime = menus.CreateTime = DateTime.Now;
                menus.Void = false;

                menuRepo.Add(menus);
                menuRepo.UnitOfWork.Commit();
                return RedirectToAction("Index");
            }
            return View(menus);

        }

        // GET: Menus/Edit/5
        [MappingPathId]
        [MappingParentMenuId]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Menus menu = menuRepo.Get(id);

            if (menu == null)
            {
                return HttpNotFound();
            }
            return View(menu);
        }

        // POST: Menus/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [MappingPathId]
        [MappingParentMenuId]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,IconCSS,IsExternalLinks,ExternalURL,Void,ParentMenuId,CreateUserId,CreateTime,LastUpdateUserId,LastUpdateTime,AllowAnonymous,System_ControllerActionsId,Order")] Menus menus)
        {
            if (ModelState.IsValid)
            {
                this.ApplyXSSProtected(menus);

                Menus menuindb = menuRepo.Get(menus.Id);

                menuindb.AfterBreak = menus.AfterBreak;
                menuindb.AllowAnonymous = menus.AllowAnonymous;
                menuindb.Description = menus.Description;
                menuindb.DisplayName = menus.DisplayName;
                menuindb.IconCSS = menus.IconCSS;
                menuindb.IsExternalLinks = menus.IsExternalLinks;
                menuindb.IsNoActionPage = menus.IsNoActionPage;
                menuindb.IsRightAligned = menus.IsRightAligned;
                menuindb.LastUpdateTime = DateTime.UtcNow;
                menuindb.LastUpdateUserId = User.Identity.GetUserGuid();
                menuindb.Name = menus.Name;
                menuindb.Order = menus.Order;
                menuindb.ParentMenuId = menus.ParentMenuId;
                menuindb.PathId = menus.PathId;
                menuindb.Void = menus.Void;

                menuRepo.UnitOfWork.Entry(menuindb).State = EntityState.Modified;
                menuRepo.UnitOfWork.Commit();

                return RedirectToAction("MenuList");
            }

            return View(menus);
        }

        // GET: Menus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Menus menus = menuRepo.Get(id);
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
            Menus menu = menuRepo.Get(id);

            menu.Void = true;
            menu.LastUpdateUserId = User.Identity.GetUserGuid();
            menu.LastUpdateTime = DateTime.UtcNow;

            menuRepo.UnitOfWork.Entry(menu).State = EntityState.Modified;
            menuRepo.UnitOfWork.Commit();

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
        [OutputCache(Duration = 1800)]
        public ActionResult MenuBar()
        {
            //選單列

            if (ViewBag.ApplicationInfo != null)
            {
                Iaspnet_UsersRepository userRepo = RepositoryHelper.Getaspnet_UsersRepository(appRepo.UnitOfWork);

                var currentLoginedUser = userRepo.GetUserByName(appInfo.ApplicationName, User.Identity.GetUserName(), DateTime.UtcNow, true);

                if (currentLoginedUser != null)
                {
                    return View("_MenuBarPartial", currentLoginedUser);
                }
            }

            return View("_MenuBarPartial", new List<Menus>());

        }


        public ActionResult ListMenuInRoles(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Menus foundMenu = appInfo.Menus.SingleOrDefault(w => w.Id == id);

            if (foundMenu == null)
            {
                return HttpNotFound();
            }

            aspnet_Users loginUser = UserManager.FindByName(User.Identity.Name);

            //IApplicationRoleRepository roleRepo = RepositoryHelper.GetApplicationRoleRepository(menuRepo.UnitOfWork);

            //Menus menu = menuRepo.Get(id);
            //var roles = roleRepo.getUnSelectedRoles(menu.ApplicationRole);

            ViewBag.RoleId = new SelectList(foundMenu.aspnet_Roles.Select(s => new { Id = s.Id, Name = s.Name }), "Id", "Name");
            ViewBag.MenuId = id;

            return View(foundMenu.aspnet_Roles.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListMenuInRoles(int? id, FormCollection collection)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Menus menu = menuRepo.Get(id);

            if (string.IsNullOrEmpty(collection["RoleId"]) == false)
            {
                Guid RoleId = Guid.Empty;

                if (Guid.TryParse(collection["RoleId"], out RoleId) == false)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                aspnet_Roles role = appInfo.aspnet_Roles.SingleOrDefault(w => w.Id == RoleId);

                menu.aspnet_Roles.Add(role);

                menuRepo.UnitOfWork.Entry(menu.aspnet_Roles).State = EntityState.Modified;
                menuRepo.UnitOfWork.Entry(menu).State = EntityState.Modified;
                menuRepo.UnitOfWork.Commit();
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

            Menus menu = menuRepo.Get(id);

            if (string.IsNullOrEmpty(collection["RoleId"]) == false)
            {
                int RoleId = 0;

                if (int.TryParse(collection["RoleId"], out RoleId) == false)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Iaspnet_RolesRepository roleRepo = RepositoryHelper.Getaspnet_RolesRepository(menuRepo.UnitOfWork);

                aspnet_Roles role = roleRepo.Get(RoleId);

                menu.aspnet_Roles.Add(role);

                menuRepo.UnitOfWork.Entry(menu.aspnet_Roles).State = EntityState.Modified;
                menuRepo.UnitOfWork.Entry(menu).State = EntityState.Modified;
                menuRepo.UnitOfWork.Commit();
            }

            return RedirectToAction("Edit", new { id = id });
        }

        public ActionResult RemoveMenuFromRole(int? id, Guid? RoleId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (RoleId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Menus menu = menuRepo.Get(id);



            aspnet_Roles role = menu.aspnet_Roles.SingleOrDefault(s => s.Id == RoleId);

            menu.LastUpdateUserId = User.Identity.GetUserGuid();
            menu.LastUpdateTime = DateTime.UtcNow;
            menu.aspnet_Roles.Remove(role);

            menuRepo.UnitOfWork.Entry(menu.aspnet_Roles).State = EntityState.Modified;
            menuRepo.UnitOfWork.Entry(menu).State = EntityState.Modified;
            menuRepo.UnitOfWork.Commit();

            return RedirectToAction("Edit", new { id = id });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //menuRepo.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
