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

namespace EdiuxTemplateWebApp.Controllers
{
    public class MenusController : Controller
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
            ViewBag.ParentMenuId = new SelectList(_menuRepo.All().Where(w => w.Void == false).ToList(), "Id", "Name");
            ViewBag.System_ControllerActionsId = new SelectList(
                _actionsRepo
                .All()
               .Select(s => new ControllerActionViewModel() {
                   Id = s.Id,
                   Name = s.Name + "(" + s.System_Controllers.Name + ")"
               }).ToList(), "Id", "Name");
            var newmodel = new Menus();
            return View(newmodel);         
        }

        // POST: Menus/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,IconCSS,IsExternalLinks,ExternalURL,Void,ParentMenuId,CreateUserId,CreateTime,LastUpdateUserId,LastUpdateTime,AllowAnonymous,System_ControllerActionsId,Order")] Menus menus)
        {

            if (ModelState.IsValid)
            {
                menus.LastUpdateUserId = menus.CreateUserId = User.Identity.GetUserId<int>();
                menus.LastUpdateTime = menus.CreateTime = DateTime.Now;
                menus.Void = false;

                _menuRepo.Add(menus);
                _menuRepo.UnitOfWork.Commit();
                return RedirectToAction("MenuList");
            }

            ViewBag.ParentMenuId = new SelectList(_menuRepo
                .All()
                .Where(w => w.Void == false)
                .ToList(), "Id", "Name");
            //ViewBag.System_ControllerActionsId = new SelectList(_actionRepo.All().Where(w => w.Void == false).ToList(), "Id", "Name");
            ViewBag.System_ControllerActionsId = new SelectList(_actionsRepo.All().Where(w => w.Void == false)
               .Select(s => new ControllerActionViewModel() { Id = s.Id, Name = s.Name + "(" + s.System_Controllers.Name + ")" }).ToList(), "Id", "Name");
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
        
        public ActionResult NotificationMenuBar(int? id)
        {
            //通知列
            return View();
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
        [OutputCache(Duration =1800,Location = System.Web.UI.OutputCacheLocation.ServerAndClient)]
        public ActionResult MenuBar()
        {
            //選單列
            return View("_MenuBarPartial", 
                _menuRepo.getMenusbyCurrentLoginUser(typeof(MvcApplication)).ToList());
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _menuRepo.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
