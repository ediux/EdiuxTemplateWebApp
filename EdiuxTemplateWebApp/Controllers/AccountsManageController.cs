using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EdiuxTemplateWebApp.Models;
using PagedList;

namespace EdiuxTemplateWebApp.Controllers
{
    public class AccountsManageController : Controller
    {
        private IApplicationUserRepository db = RepositoryHelper.GetApplicationUserRepository();

        // GET: AccountsManage
        public ActionResult Index(int? pageid, int? pageSize)
        {
            return View(db.Where(w => w.Void == false).OrderBy(o => o.Id).ToPagedList(pageid ?? 1, pageSize ?? 10));           
        }

        // GET: AccountsManage/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Get(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // GET: AccountsManage/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AccountsManage/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserName,Password,PasswordHash,SecurityStamp,TwoFactorEnabled,Void,DisplayName,EMail,EMailConfirmed,PhoneNumber,PhoneConfirmed,CreateUserId,CreateTime,LastUpdateUserId,LastUpdateTime,LastActivityTime,LastUnlockedTime,LastLoginFailTime,AccessFailedCount,LockoutEnabled,LockoutEndDate,ResetPasswordToken")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                db.Add(applicationUser);
                db.UnitOfWork.Commit();
                return RedirectToAction("Index");
            }

            return View(applicationUser);
        }

        // GET: AccountsManage/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Get(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: AccountsManage/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserName,Password,PasswordHash,SecurityStamp,TwoFactorEnabled,Void,DisplayName,EMail,EMailConfirmed,PhoneNumber,PhoneConfirmed,CreateUserId,CreateTime,LastUpdateUserId,LastUpdateTime,LastActivityTime,LastUnlockedTime,LastLoginFailTime,AccessFailedCount,LockoutEnabled,LockoutEndDate,ResetPasswordToken")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                db.UnitOfWork.Context.Entry(applicationUser).State = EntityState.Modified;
                db.UnitOfWork.Commit();
                return RedirectToAction("Index");
            }
            return View(applicationUser);
        }

        // GET: AccountsManage/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Get(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: AccountsManage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ApplicationUser applicationUser = db.Get(id);
            db.Delete(applicationUser);
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
