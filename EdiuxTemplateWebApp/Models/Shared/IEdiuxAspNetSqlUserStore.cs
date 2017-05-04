using EdiuxTemplateWebApp.Models.AspNetModels;
using EdiuxTemplateWebApp.Models.Identity;
using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models
{
    //,
    //    IApplicationStore<aspnet_Applications, Guid>,
    //    IProfileStore<UserProfileViewModel, aspnet_Profile, Guid>,
    //    IPageStore<aspnet_Paths, Guid>

    /// <summary>
    /// 
    /// </summary>
    public interface IEdiuxAspNetSqlUserStore : ICustomUserStore,
        IRoleStore<aspnet_Roles, Guid>,
        IQueryableUserStore<aspnet_Users, Guid>,
        IQueryableRoleStore<aspnet_Roles, Guid>,
        IApplicationStore<aspnet_Applications, Guid>
    {


    }
}