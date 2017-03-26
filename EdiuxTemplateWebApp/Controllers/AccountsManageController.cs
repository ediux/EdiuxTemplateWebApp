using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using EdiuxTemplateWebApp.Models.AspNetModels;
using EdiuxTemplateWebApp.Models;

namespace EdiuxTemplateWebApp.Controllers
{
    public class AccountsManageController : Controller
    {
        private Iaspnet_ApplicationsRepository db;
        // private IApplicationRoleRepository roleRepo;

        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private aspnet_Applications appInfo;
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
                return _roleManager ?? new ApplicationRoleManager(new Models.EdiuxAspNetSqlUserStore(HttpContext.GetOwinContext().Get<Models.AspNetModels.IUnitOfWork>()));
            }
            private set
            {
                _roleManager = value;
            }
        }

        public AccountsManageController()
        {
            db = RepositoryHelper.Getaspnet_ApplicationsRepository();
            appInfo = this.getApplicationInfo();
            //roleRepo = RepositoryHelper.GetApplicationRoleRepository(db.UnitOfWork);
        }

        // GET: AccountsManage
        public ActionResult Index(int? pageid, int? pageSize)
        {
            if (ViewBag.ApplicationInfo == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            aspnet_Applications appInfo = ViewBag.ApplicationInfo as aspnet_Applications;

            int iSize = pageSize ?? 5;
            if (iSize == -1)
            {

                var result = UserManager.Users.ToList();
                return View(result.ToPagedList(1, result.Count()));
            }
            return View(UserManager.Users.OrderBy(o => o.Id).ToPagedList(pageid ?? 1, pageSize ?? 5));
        }

        // GET: AccountsManage/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            var user = await UserManager.FindByIdAsync(id ?? Guid.Empty);

            if (user == null)
                return HttpNotFound();

            ViewBag.RoleName = (user.aspnet_Roles != null) ? string.Join(";", user.aspnet_Roles.Select(s => s.Name).ToArray()) : "訪客";
            return View(user);
        }

        // GET: AccountsManage/Create
        public ActionResult Create()
        {
            if (ViewBag.ApplicationInfo == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            aspnet_Applications appInfo = ViewBag.ApplicationInfo as aspnet_Applications;

            ViewBag.RoleId = new SelectList(appInfo.aspnet_Roles, "Id", "Name");
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
                if (ViewBag.ApplicationInfo == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                aspnet_Applications appInfo = ViewBag.ApplicationInfo as aspnet_Applications;

                aspnet_Users user = new aspnet_Users();


                user.UserName = registerViewModel.UserName;
                user.aspnet_Membership = new aspnet_Membership();
                user.aspnet_Membership.Password = registerViewModel.Password;
                user.aspnet_Membership.PasswordSalt = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                user.aspnet_Membership.Email = registerViewModel.Email;

                //user.DisplayName = registerViewModel.DisplayName;
                //user.EMail = registerViewModel.Email;
                //user.Password = registerViewModel.Password;

                //user.PasswordHash = UserManager.PasswordHasher.HashPassword(registerViewModel.Password);

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
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Id", "Name");
            return View(registerViewModel);
        }

        // GET: AccountsManage/Edit/5
        public ActionResult Edit(Guid? id, string returnUrl)
        {
            return RedirectToAction("UserProfile", "Manage", new { id });
        }

        // POST: AccountsManage/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserName,Password,PasswordHash,SecurityStamp,TwoFactorEnabled,Void,DisplayName,EMail,EMailConfirmed,PhoneNumber,PhoneConfirmed,CreateUserId,CreateTime,LastUpdateUserId,LastUpdateTime,LastActivityTime,LastUnlockedTime,LastLoginFailTime,AccessFailedCount,LockoutEnabled,LockoutEndDate,ResetPasswordToken")] aspnet_Users applicationUser)
        {
            if (ModelState.IsValid)
            {
                UserManager.UpdateAsync(applicationUser);

                return RedirectToAction("Index");
            }
            return View(applicationUser);
        }

        // GET: AccountsManage/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            aspnet_Users applicationUser = UserManager.FindById(id ?? Guid.Empty);// this.getApplicationInfo().FindUserById(id ?? Guid.Empty);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: AccountsManage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            aspnet_Users applicationUser = UserManager.FindById(id);
            UserManager.Delete(applicationUser);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AddUserToRole(Guid id, Guid RoleId)
        {
            aspnet_Users applicationUser = UserManager.FindById(id);
            if (applicationUser != null)
            {
                aspnet_Roles role = RoleManager.FindById(RoleId);
                applicationUser.aspnet_Roles.Add(role);
                UserManager.Update(applicationUser);
            }
            return RedirectToAction("UserProfile", "Manage", new { id = id });
        }

        public ActionResult RemoveUserFromRole(Guid id, Guid roleId)
        {
            aspnet_Users applicationUser = UserManager.FindById(id);
            if (applicationUser != null)
            {
                aspnet_Roles role = RoleManager.FindById(roleId);
                applicationUser.aspnet_Roles.Remove(role);
                UserManager.Update(applicationUser);
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
