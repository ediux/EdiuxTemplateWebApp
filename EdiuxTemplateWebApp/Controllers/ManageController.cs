﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using EdiuxTemplateWebApp.Models;
using EdiuxTemplateWebApp.Helpers;
namespace EdiuxTemplateWebApp.Controllers
{
    [Authorize]
    public class ManageController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
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

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "已變更您的密碼。"
                : message == ManageMessageId.SetPasswordSuccess ? "已設定您的密碼。"
                : message == ManageMessageId.SetTwoFactorSuccess ? "已設定您的雙因素驗證。"
                : message == ManageMessageId.Error ? "發生錯誤。"
                : message == ManageMessageId.AddPhoneSuccess ? "已新增您的電話號碼。"
                : message == ManageMessageId.RemovePhoneSuccess ? "已移除您的電話號碼。"
                : "";
            ViewBag.AccountTotalCount = UserManager.Users.Count();
            ViewBag.NewRegisterCount = UserManager.Users.Where(w => w.CreateTime >= DateTime.Now.AddMinutes(-30)).Count();
            var userId = User.Identity.GetUserId<int>();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            };
            return View(model);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId<int>(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // 產生並傳送 Token
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId<int>(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "您的安全碼為: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId<int>(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId<int>(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId<int>(), phoneNumber);
            // 透過 SMS 提供者傳送 SMS，以驗證電話號碼
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId<int>(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            ModelState.AddModelError("", "無法驗證號碼");
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId<int>(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId<int>(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId<int>(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "已移除外部登入。"
                : message == ManageMessageId.Error ? "發生錯誤。"
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId<int>());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // 要求重新導向至外部登入提供者，以連結目前使用者的登入
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId<int>(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }
        #region User Profile Manage
        public async Task<ActionResult> UserProfile(int? id)
        {

            UserProfileViewModel model = new UserProfileViewModel();

            var userId = id ?? User.Identity.GetUserId<int>();
            var user = await UserManager.FindByIdAsync(userId);
            if (user != null)
            {

                model.CloneFrom(user);
                if (user.ApplicationUserClaim.Any())
                {
                    foreach(var claim in user.ApplicationUserClaim)
                    {
                        switch (claim.ClaimType)
                        {
                            case "FirstName":
                                model.FirstName = claim.ClaimValue;
                                break;
                            case "MiddleName":
                                model.MiddleName = claim.ClaimValue;
                                break;
                            case "LastName":
                                model.LastName = claim.ClaimValue;
                                break;
                            case "Gender":
                                bool isMale = false;
                                if(bool.TryParse(claim.ClaimValue,out isMale))
                                model.Gender = isMale;
                                break;
                            case "PositionTitle":
                                model.PositionTitle = claim.ClaimValue;
                                break;
                        }
                     
                    }
                }
            }
            model.UserAccountManage = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            };
            IApplicationRoleRepository roleRepo = RepositoryHelper.GetApplicationRoleRepository(UserManager.UnitOfWork);
            var roleList = user.ApplicationRole.AsEnumerable();
            var options = roleRepo.All().AsEnumerable();
            options = options.Except(roleList);
            //options.Insert(0, new ApplicationRole() { Id = 0, Name = "請選擇" });
            ViewBag.RoleId = new SelectList(options, "Id", "Name");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserProfile(int id, UserProfileViewModel userProfile)
        {
            IApplicationUserClaimRepository claimRepo = RepositoryHelper.GetApplicationUserClaimRepository(UserManager.UnitOfWork);
            var userId = id;
            var user = await UserManager.FindByIdAsync(userId);
            if (user != null)
            {
                user = userProfile;

                if (!string.IsNullOrEmpty(userProfile.FirstName))
                {
                   if(user.ApplicationUserClaim
                        .Any(s => s.ClaimType.Equals("FirstName", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        ApplicationUserClaim data = user.ApplicationUserClaim
                            .Single(s => s.ClaimType.Equals("FirstName", StringComparison.InvariantCultureIgnoreCase));

                        data.ClaimValue = userProfile.FirstName;

                    }else
                    {
                        ApplicationUserClaim data = new ApplicationUserClaim();
                        data.ClaimType = "FirstName";
                        data.ClaimValue = userProfile.FirstName;
                        data.UserId = userProfile.Id;

                        claimRepo.Add(data);
                        
                        user.ApplicationUserClaim.Add(data);
                    }
                }

                if (!string.IsNullOrEmpty(userProfile.MiddleName))
                {
                    if (user.ApplicationUserClaim
                        .Any(s => s.ClaimType.Equals("MiddleName", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        ApplicationUserClaim data = user.ApplicationUserClaim
                            .Single(s => s.ClaimType.Equals("MiddleName", StringComparison.InvariantCultureIgnoreCase));

                        data.ClaimValue = userProfile.MiddleName;

                    }
                    else
                    {
                        ApplicationUserClaim data = new ApplicationUserClaim();
                        data.ClaimType = "MiddleName";
                        data.ClaimValue = userProfile.MiddleName;
                        data.UserId = userProfile.Id;

                        claimRepo.Add(data);
                        user.ApplicationUserClaim.Add(data);
                    }
                }

                if (!string.IsNullOrEmpty(userProfile.LastName))
                {
                    if (user.ApplicationUserClaim
    .Any(s => s.ClaimType.Equals("LastName", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        ApplicationUserClaim data = user.ApplicationUserClaim
                            .Single(s => s.ClaimType.Equals("LastName", StringComparison.InvariantCultureIgnoreCase));

                        data.ClaimValue = userProfile.LastName;

                    }
                    else
                    {
                        ApplicationUserClaim data = new ApplicationUserClaim();
                        data.ClaimType = "LastName";
                        data.ClaimValue = userProfile.LastName;
                        data.UserId = userProfile.Id;


                        claimRepo.Add(data);
                        user.ApplicationUserClaim.Add(data);
                    }
                }

                if (user.ApplicationUserClaim
    .Any(s => s.ClaimType.Equals("Gender", StringComparison.InvariantCultureIgnoreCase)))
                {
                    ApplicationUserClaim data = user.ApplicationUserClaim
                        .Single(s => s.ClaimType.Equals("Gender", StringComparison.InvariantCultureIgnoreCase));

                    data.ClaimValue = userProfile.Gender.ToString();

                }
                else
                {
                    ApplicationUserClaim data = new ApplicationUserClaim();
                    data.ClaimType = "Gender";
                    data.ClaimValue = userProfile.Gender.ToString();
                    data.UserId = userProfile.Id;

                    claimRepo.Add(data);
                    user.ApplicationUserClaim.Add(data);
                }

                if (!string.IsNullOrEmpty(userProfile.PositionTitle))
                {
                    if (user.ApplicationUserClaim
      .Any(s => s.ClaimType.Equals("PositionTitle", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        ApplicationUserClaim data = user.ApplicationUserClaim
                            .Single(s => s.ClaimType.Equals("PositionTitle", StringComparison.InvariantCultureIgnoreCase));

                        data.ClaimValue = userProfile.PositionTitle;

                    }
                    else
                    {
                        ApplicationUserClaim data = new ApplicationUserClaim();
                        data.ClaimType = "PositionTitle";
                        data.ClaimValue = userProfile.PositionTitle;
                        data.UserId = userProfile.Id;
                        
                        claimRepo.Add(data);
                        user.ApplicationUserClaim.Add(data);
                       
                    }
                }
                
                await UserManager.UpdateAsync(user);
            }
         
            return RedirectToAction("UserProfile", new { Id = userProfile.Id });
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helper
        // 新增外部登入時用來當做 XSRF 保護
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId<int>());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId<int>());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion
    }
}