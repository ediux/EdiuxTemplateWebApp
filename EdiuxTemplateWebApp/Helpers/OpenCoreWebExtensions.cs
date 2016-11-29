using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EdiuxTemplateWebApp.Helpers
{
    public static class OpenCoreWebExtensions
    {
        [DebuggerStepThrough]
        public async static Task<bool> TwoFactorBrowserRememberedAsync(this IAuthenticationManager manager, int userId)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            var result = await manager.AuthenticateAsync(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
            return (result != null && result.Identity != null && result.Identity.GetUserId<int>() == userId);
        }
    }
}