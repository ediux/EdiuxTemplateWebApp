using EdiuxTemplateWebApp.Models.AspNetModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models
{
    public interface ICustomUserStore : IUserStore<aspnet_Users, Guid>,
        IUserRoleStore<aspnet_Users, Guid>,
        IUserEmailStore<aspnet_Users, Guid>,
        IUserLockoutStore<aspnet_Users, Guid>,
        IUserLoginStore<aspnet_Users, Guid>,
        IUserPasswordStore<aspnet_Users, Guid>,
        IUserPhoneNumberStore<aspnet_Users, Guid>,
        IUserSecurityStampStore<aspnet_Users, Guid>,
        IUserTwoFactorStore<aspnet_Users, Guid>,
        IUserClaimStore<aspnet_Users, Guid>
    {
        Task<aspnet_Users> GetUserByIdAsync(Guid userId);
    }
}
