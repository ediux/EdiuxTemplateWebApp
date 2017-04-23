using EdiuxTemplateWebApp.Models.AspNetModels;
using EdiuxTemplateWebApp.Models.Identity;
using EdiuxTemplateWebApp.Models.Shared;
using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models
{
    public interface IEdiuxAspNetSqlUserStore : IUserStore<aspnet_Users, Guid>,
        IUserRoleStore<aspnet_Users, Guid>,
        IRoleStore<aspnet_Roles, Guid>,
        IUserEmailStore<aspnet_Users, Guid>, 
        IUserLockoutStore<aspnet_Users, Guid>, 
        IUserLoginStore<aspnet_Users, Guid>, 
        IUserPasswordStore<aspnet_Users, Guid>, 
        IUserPhoneNumberStore<aspnet_Users, Guid>, 
        IUserSecurityStampStore<aspnet_Users, Guid>, 
        IUserTwoFactorStore<aspnet_Users, Guid>, 
        IUserClaimStore<aspnet_Users, Guid>, 
        IQueryableUserStore<aspnet_Users, Guid>, 
        IQueryableRoleStore<aspnet_Roles, Guid>, 
        IApplicationStore<aspnet_Applications, Guid>,
        IProfileStore<PageSettingByUserViewModel,Guid>,
        IPageStore<aspnet_Paths,Guid>
    {
        
        Task<aspnet_Users> GetUserByIdAsync(Guid userId);
    }
}