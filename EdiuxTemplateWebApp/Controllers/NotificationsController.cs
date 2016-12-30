using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EdiuxTemplateWebApp.Models;
using Microsoft.AspNet.Identity;

namespace EdiuxTemplateWebApp.Controllers
{
    public class NotificationsController : BaseController
    {
        private ISystem_NotificationsRepository db;
        private IApplicationUserRepository userRepo;

     
        public NotificationsController()
        {
            db = RepositoryHelper.GetSystem_NotificationsRepository();
            userRepo = RepositoryHelper.GetApplicationUserRepository(db.UnitOfWork);
        }
        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult NotificationsBar()
        {
            int currentUserId = User.Identity.GetUserId<int>();
            var system_Notifications = db.All()
                .Where(w => w.Read == false
                && w.TargetUserId == currentUserId)
                .OrderByDescending(o=>o.CreateTime)
                .Include(s => s.Sender)
                .Include(s => s.Recipient);
            return PartialView(system_Notifications.ToList());
        }
        // GET: Notifications
        public ActionResult Index()
        {
            var system_Notifications = db.All()
                .Include(s => s.Sender)
                .Include(s => s.Recipient);
            return View(system_Notifications.ToList());
        }

        // GET: Notifications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            System_Notifications system_Notifications = db.Get(id);
            if (system_Notifications == null)
            {
                return HttpNotFound();
            }
            return View(system_Notifications);
        }

        // GET: Notifications/Create
        public ActionResult Create()
        {
            ViewBag.FromUserId = new SelectList(userRepo.All().AsEnumerable(), "Id", "UserName");
            ViewBag.TargetUserId = new SelectList(userRepo.All().AsEnumerable(), "Id", "UserName");
            return View();
        }

        // POST: Notifications/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FromUserId,TargetUserId,Subject,Message,MessageLink,Read,RelayNotificationId,CreateTime,RelayTime")] System_Notifications system_Notifications)
        {
            if (ModelState.IsValid)
            {
                db.Add(system_Notifications);
                db.UnitOfWork.Commit();
                return RedirectToAction("Index");
            }

            ViewBag.FromUserId = new SelectList(userRepo.All(), "Id", "UserName", system_Notifications.FromUserId);
            ViewBag.TargetUserId = new SelectList(userRepo.All(), "Id", "UserName", system_Notifications.TargetUserId);
            return View(system_Notifications);
        }

        // GET: Notifications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            System_Notifications system_Notifications = db.Get(id);
            if (system_Notifications == null)
            {
                return HttpNotFound();
            }
            ViewBag.FromUserId = new SelectList(userRepo.All(), "Id", "UserName", system_Notifications.FromUserId);
            ViewBag.TargetUserId = new SelectList(userRepo.All(), "Id", "UserName", system_Notifications.TargetUserId);
            return View(system_Notifications);
        }

        // POST: Notifications/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FromUserId,TargetUserId,Subject,Message,MessageLink,Read,RelayNotificationId,CreateTime,RelayTime")] System_Notifications system_Notifications)
        {
            if (ModelState.IsValid)
            {
                db.UnitOfWork.Context.Entry(system_Notifications).State = EntityState.Modified;
                db.UnitOfWork.Commit();
                return RedirectToAction("Index");
            }
            ViewBag.FromUserId = new SelectList(userRepo.All(), "Id", "UserName", system_Notifications.FromUserId);
            ViewBag.TargetUserId = new SelectList(userRepo.All(), "Id", "UserName", system_Notifications.TargetUserId);
            return View(system_Notifications);
        }

        // GET: Notifications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            System_Notifications system_Notifications = db.Get(id);
            if (system_Notifications == null)
            {
                return HttpNotFound();
            }
            return View(system_Notifications);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            System_Notifications system_Notifications = db.Get(id);
            db.Delete(system_Notifications);
            db.UnitOfWork.Commit();
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
