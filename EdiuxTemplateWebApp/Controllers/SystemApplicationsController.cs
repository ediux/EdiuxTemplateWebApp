using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EdiuxTemplateWebApp.Models.AspNetModels;
using Microsoft.AspNet.Identity.Owin;

namespace EdiuxTemplateWebApp.Controllers
{
    public class SystemApplicationsController : BaseController
    {
        private Iaspnet_ApplicationsRepository appRepo;

        public SystemApplicationsController()
        {
            appRepo = OwinContext.Get<Iaspnet_ApplicationsRepository>();
        }

        // GET: SystemApplications
        public ActionResult Index()
        {
            return View(appRepo.All().ToList());
        }

        // GET: SystemApplications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            aspnet_Applications system_Applications = appRepo.Get(id);
            if (system_Applications == null)
            {
                return HttpNotFound();
            }
            return View(system_Applications);
        }

        // GET: SystemApplications/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SystemApplications/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] aspnet_Applications system_Applications)
        {
            if (ModelState.IsValid)
            {
                appRepo.Add(system_Applications);
                appRepo.UnitOfWork.Commit();
                return RedirectToAction("Index");
            }

            return View(system_Applications);
        }

        // GET: SystemApplications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            aspnet_Applications system_Applications = appRepo.Get(id);
            if (system_Applications == null)
            {
                return HttpNotFound();
            }
            return View(system_Applications);
        }

        // POST: SystemApplications/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] aspnet_Applications system_Applications)
        {
            if (ModelState.IsValid)
            {
                appRepo.Update(system_Applications); 
                return RedirectToAction("Index");
            }
            return View(system_Applications);
        }

        // GET: SystemApplications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            aspnet_Applications system_Applications = appRepo.Get(id);
            if (system_Applications == null)
            {
                return HttpNotFound();
            }
            return View(system_Applications);
        }

        // POST: SystemApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            aspnet_Applications system_Applications = appRepo.Get(id);
            appRepo.Delete(system_Applications);
            appRepo.UnitOfWork.Commit();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                appRepo.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
