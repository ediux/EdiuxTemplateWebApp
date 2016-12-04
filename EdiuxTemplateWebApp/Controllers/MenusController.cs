using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EdiuxTemplateWebApp.Models;

namespace EdiuxTemplateWebApp.Controllers
{
    public class MenusController : Controller
    {
        private AspNetDbEntities db = new AspNetDbEntities();

        // GET: Menus
        public ActionResult Index()
        {
            var menus = db.Menus.Include(m => m.Menus2).Include(m => m.System_ControllerActions);
            return View(menus.ToList());
        }

        // GET: Menus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Menus menus = db.Menus.Find(id);
            if (menus == null)
            {
                return HttpNotFound();
            }
            return View(menus);
        }

        // GET: Menus/Create
        public ActionResult Create()
        {
            ViewBag.ParentMenuId = new SelectList(db.Menus, "Id", "Name");
            ViewBag.System_ControllerActionsId = new SelectList(db.System_ControllerActions, "Id", "Name");
            return View();
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
                db.Menus.Add(menus);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ParentMenuId = new SelectList(db.Menus, "Id", "Name", menus.ParentMenuId);
            ViewBag.System_ControllerActionsId = new SelectList(db.System_ControllerActions, "Id", "Name", menus.System_ControllerActionsId);
            return View(menus);
        }

        // GET: Menus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Menus menus = db.Menus.Find(id);
            if (menus == null)
            {
                return HttpNotFound();
            }
            ViewBag.ParentMenuId = new SelectList(db.Menus, "Id", "Name", menus.ParentMenuId);
            ViewBag.System_ControllerActionsId = new SelectList(db.System_ControllerActions, "Id", "Name", menus.System_ControllerActionsId);
            return View(menus);
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
                db.Entry(menus).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ParentMenuId = new SelectList(db.Menus, "Id", "Name", menus.ParentMenuId);
            ViewBag.System_ControllerActionsId = new SelectList(db.System_ControllerActions, "Id", "Name", menus.System_ControllerActionsId);
            return View(menus);
        }

        // GET: Menus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Menus menus = db.Menus.Find(id);
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
            Menus menus = db.Menus.Find(id);
            db.Menus.Remove(menus);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
