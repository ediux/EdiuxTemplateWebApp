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
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Controllers
{
    public class AccountsManageController : Controller
    {
        private IApplicationUserRepository db;
        private IApplicationRoleRepository roleRepo;

        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

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

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? new ApplicationRoleManager(new EdiuxAspNetSqlUserStore(HttpContext.GetOwinContext().Get<IUnitOfWork>()));
            }
            private set
            {
                _roleManager = value;
            }
        }

        public AccountsManageController()
        {
            db = RepositoryHelper.GetApplicationUserRepository();
            roleRepo = RepositoryHelper.GetApplicationRoleRepository(db.UnitOfWork);
        }

        // GET: AccountsManage
        public ActionResult Index(int? pageid, int? pageSize)
        {
            int iSize = pageSize ?? 5;
            if (iSize == -1)
            {
                var result = db.Where(w => w.Void == false).OrderBy(o => o.Id);
                return View(result.ToPagedList(1, result.Count()));
            }
            return View(db.Where(w => w.Void == false).OrderBy(o => o.Id).ToPagedList(pageid ?? 1, pageSize ?? 5));
        }

        // GET: AccountsManage/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            var user = await UserManager.FindByIdAsync(id??0);

            if (user == null)
                return HttpNotFound();


            ViewBag.RoleName = (user.ApplicationRole != null) ? string.Join(";", user.ApplicationRole.Select(s => s.Name).ToArray()) : "訪客";
            return View(user);
        }

        // GET: AccountsManage/Create
        public ActionResult Create()
        {
            ViewBag.RoleId = new SelectList(roleRepo.Where(w => w.Void == false), "Id", "Name");
            return View(new RegisterForAdminsViewModel());
        }

        // POST: AccountsManage/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserName,DisplayName,Password,ConfirmPassword,RoleId,Email")] RegisterForAdminsViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = ApplicationUser.Create();
                user.UserName = registerViewModel.UserName;
                user.DisplayName = registerViewModel.DisplayName;
                user.EMail = registerViewModel.Email;
                user.Password = registerViewModel.Password;
       
                user.PasswordHash = UserManager.PasswordHasher.HashPassword(registerViewModel.Password);

                IdentityResult result = UserManager.Create(user);
                if (result.Succeeded)
                {
                    user = UserManager.FindByName(registerViewModel.UserName);

                    if (user != null)
                    {
                        var role = RoleManager.FindById(registerViewModel.RoleId);
                        //user.ApplicationRole.Add(role);
                        UserManager.AddToRole(user.Id, role.Name);
                        UserManager.Update(user);

                        return RedirectToAction("Index");
                    }
                }
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles.Where(w => w.Void == false), "Id", "Name");
            return View(registerViewModel);
        }

        // GET: AccountsManage/Edit/5
        public ActionResult Edit(int? id,string returnUrl)
        {
            return RedirectToAction("UserProfile", "Manage", new { id });
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

        public ActionResult RemoveUserFromRole(int id,int roleId)
        {
            ApplicationUser applicationUser = db.Get(id);
            if (applicationUser != null)
            {
                ApplicationRole role = roleRepo.Get(roleId);
                applicationUser.ApplicationRole.Remove(role);
                db.UpdateAsync(applicationUser).Wait();
            }
            return RedirectToAction("UserProfile", "Manage", new { id = id });
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
