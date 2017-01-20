using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using Owin;
using Microsoft.Owin.Helpers;
using EdiuxTemplateWebApp.Models.AspNetModels;

namespace EdiuxTemplateWebApp.Helpers
{
    public static class OpenCoreWebExtensions
    {
        [DebuggerStepThrough]
        public async static Task<bool> TwoFactorBrowserRememberedAsync(this IAuthenticationManager manager, Guid userId)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            var result = await manager.AuthenticateAsync(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
            return (result != null && result.Identity != null && result.Identity.GetUserId() == userId);
        }

        public static System.Guid GetUserId(this IIdentity identity)
        {
            Models.AspNetModels.Iaspnet_UsersRepository userRepo = Models.AspNetModels.RepositoryHelper.Getaspnet_UsersRepository();

            Task<aspnet_Users> foundUser = userRepo.FindByNameAsync(identity.Name);

            if (foundUser.Result != null)
                return foundUser.Result.Id;

            return Guid.Empty;
        }

        public static System.Guid GetUserGuid(this IIdentity identity)
        {
            return GetUserId(identity);
        }
    }
}