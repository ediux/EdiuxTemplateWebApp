using EdiuxTemplateWebApp.Models.AspNetModels;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp
{
    public static class IdentityFrameworkExtensions
    {
        public async static Task<bool> TwoFactorBrowserRememberedAsync(this IAuthenticationManager manager, Guid userId)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            var result = await manager.AuthenticateAsync(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
            return (result != null && result.Identity != null && result.Identity.GetUserId() == userId);
        }

        public static Guid GetUserId(this IIdentity identity)
        {
            Iaspnet_UsersRepository userRepo = RepositoryHelper.Getaspnet_UsersRepository();

            aspnet_Users foundUser = userRepo.All().SingleOrDefault(w => w.UserName == identity.Name);

            if (foundUser != null)
                return foundUser.Id;

            return Guid.Empty;
        }

        public static Guid GetUserId2(this IIdentity identity)
        {
            return GetUserId(identity);
        }
        public static AspNetDbEntities GetAspNetMembershipDbContext(this IUnitOfWork db)
        {
            return db.Context as AspNetDbEntities;
        }
    }
}