using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using EdiuxTemplateWebApp.Models.AspNetModels;
using EdiuxTemplateWebApp.Models;

namespace EdiuxTemplateWebApp
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // 將您的電子郵件服務外掛到這裡以傳送電子郵件。
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // 將您的 SMS 服務外掛到這裡以傳送簡訊。
            return Task.FromResult(0);
        }
    }
    public class ApplicationUserRole : IdentityUserRole<int>
    {

    }
    // 設定此應用程式中使用的應用程式使用者管理員。UserManager 在 ASP.NET Identity 中定義且由應用程式中使用。
    public class ApplicationUserManager : UserManager<aspnet_Users, Guid>
    {
        public ApplicationUserManager(IUserStore<aspnet_Users, Guid> store)
            : base(store)
        {

        }

        public Models.AspNetModels.IUnitOfWork UnitOfWork
        {
            get { return ((Models.EdiuxAspNetSqlUserStore)Store).UnitOfWork; }
        }
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new EdiuxAspNetSqlUserStore(context.Get<Models.AspNetModels.IUnitOfWork>()));

            // 設定使用者名稱的驗證邏輯
            manager.UserValidator = new UserValidator<aspnet_Users, Guid>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };

            // 設定密碼的驗證邏輯
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // 設定使用者鎖定詳細資料
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // 註冊雙因素驗證提供者。此應用程式使用手機和電子郵件接收驗證碼以驗證使用者
            // 您可以撰寫專屬提供者，並將它外掛到這裡。
            manager.RegisterTwoFactorProvider("電話代碼", new PhoneNumberTokenProvider<aspnet_Users, Guid>
            {
                MessageFormat = "您的安全碼為 {0}"
            });
            manager.RegisterTwoFactorProvider("電子郵件代碼", new EmailTokenProvider<aspnet_Users, Guid>
            {
                Subject = "安全碼",
                BodyFormat = "您的安全碼為 {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<aspnet_Users, Guid>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    public class ApplicationRoleManager : RoleManager<aspnet_Roles, Guid>
    {
        public ApplicationRoleManager(IRoleStore<aspnet_Roles, Guid> store) :
            base(store)
        {

        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            var roleManager = new ApplicationRoleManager(new EdiuxAspNetSqlUserStore(context.Get<Models.AspNetModels.IUnitOfWork>()));
            roleManager.RoleValidator = new RoleValidator<aspnet_Roles, Guid>(roleManager);

            return roleManager;
        }

        public async override Task<IdentityResult> UpdateAsync(aspnet_Roles role)
        {
            try
            {
                await Store.UpdateAsync(role);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                throw;
            }

        }
    }

    // 設定在此應用程式中使用的應用程式登入管理員。
    public class ApplicationSignInManager : SignInManager<aspnet_Users, Guid>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(aspnet_Users user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }

        public override Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            return base.PasswordSignInAsync(userName, password, isPersistent, shouldLockout);
        }
    }
}
