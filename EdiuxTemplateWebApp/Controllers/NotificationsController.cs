using EdiuxTemplateWebApp.Models.AspNetModels;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace EdiuxTemplateWebApp.Controllers
{
    public class NotificationsController : BaseController
    {
        private Iaspnet_WebEvent_EventsRepository db;
        private Iaspnet_UsersRepository userRepo;

     
        public NotificationsController()
        {
            db = OwinContext.Get<Iaspnet_WebEvent_EventsRepository>();
            userRepo = OwinContext.Get<Iaspnet_UsersRepository>();
        }
        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult NotificationsBar()
        {
            Guid currentUserId = User.Identity.GetUserId();
            var system_Notifications = db.All()
                .OrderByDescending(o => o.EventId);

            return PartialView(system_Notifications.ToList());
        }
        // GET: Notifications
        public ActionResult Index()
        {
            var system_Notifications = db.All();
              
            return View(system_Notifications.ToList());
        }

        // GET: Notifications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            aspnet_WebEvent_Events system_Notifications = db.Get(id);
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
        public ActionResult Create([Bind(Include = "Id,FromUserId,TargetUserId,Subject,Message,MessageLink,Read,RelayNotificationId,CreateTime,RelayTime")] aspnet_WebEvent_Events system_Notifications)
        {
            if (ModelState.IsValid)
            {
                db.Add(system_Notifications);
                db.UnitOfWork.Commit();
                return RedirectToAction("Index");
            }

          
            return View(system_Notifications);
        }

        // GET: Notifications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            aspnet_WebEvent_Events system_Notifications = db.Get(id);
            if (system_Notifications == null)
            {
                return HttpNotFound();
            }
         
            return View(system_Notifications);
        }

        // POST: Notifications/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FromUserId,TargetUserId,Subject,Message,MessageLink,Read,RelayNotificationId,CreateTime,RelayTime")] aspnet_WebEvent_Events system_Notifications)
        {
            if (ModelState.IsValid)
            {
                db.UnitOfWork.Entry(system_Notifications).State = EntityState.Modified;
                db.UnitOfWork.Commit();
                return RedirectToAction("Index");
            }
            //ViewBag.FromUserId = new SelectList(userRepo.All(), "Id", "UserName", system_Notifications.FromUserId);
            //ViewBag.TargetUserId = new SelectList(userRepo.All(), "Id", "UserName", system_Notifications.TargetUserId);
            return View(system_Notifications);
        }

        // GET: Notifications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            aspnet_WebEvent_Events system_Notifications = db.Get(id);
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
            aspnet_WebEvent_Events system_Notifications = db.Get(id);
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
