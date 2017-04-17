using EdiuxTemplateWebApp.Models;
using EdiuxTemplateWebApp.Models.AspNetModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartup(typeof(EdiuxTemplateWebApp.Startup))]
namespace EdiuxTemplateWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext(RepositoryHelper.GetUnitOfWork);

            app.CreatePerOwinContext<Iaspnet_ApplicationsRepository>((x, i)
                => RepositoryHelper.Getaspnet_ApplicationsRepository(i.Get<IUnitOfWork>()));
            app.CreatePerOwinContext<Iaspnet_MembershipRepository>((x, i)
                => RepositoryHelper.Getaspnet_MembershipRepository(i.Get<IUnitOfWork>()));
            app.CreatePerOwinContext<Iaspnet_PathsRepository>((x, i)
                => RepositoryHelper.Getaspnet_PathsRepository(i.Get<IUnitOfWork>()));
            app.CreatePerOwinContext<Iaspnet_PersonalizationAllUsersRepository>((x, i)
                => RepositoryHelper.Getaspnet_PersonalizationAllUsersRepository(i.Get<IUnitOfWork>()));
            app.CreatePerOwinContext<Iaspnet_PersonalizationPerUserRepository>((x, i)
                => RepositoryHelper.Getaspnet_PersonalizationPerUserRepository(i.Get<IUnitOfWork>()));
            app.CreatePerOwinContext<Iaspnet_ProfileRepository>((x, i)
                => RepositoryHelper.Getaspnet_ProfileRepository(i.Get<IUnitOfWork>()));
            app.CreatePerOwinContext<Iaspnet_RolesRepository>((x, i)
                => RepositoryHelper.Getaspnet_RolesRepository(i.Get<IUnitOfWork>()));
            app.CreatePerOwinContext<Iaspnet_UserClaimsRepository>((x, i)
                => RepositoryHelper.Getaspnet_UserClaimsRepository(i.Get<IUnitOfWork>()));
            app.CreatePerOwinContext<Iaspnet_UserLoginRepository>((x, i)
                => RepositoryHelper.Getaspnet_UserLoginRepository(i.Get<IUnitOfWork>()));
            app.CreatePerOwinContext<Iaspnet_UsersRepository>((x, i)
                => RepositoryHelper.Getaspnet_UsersRepository(i.Get<IUnitOfWork>()));
            app.CreatePerOwinContext<Iaspnet_VoidUsersRepository>((x, i)
                => RepositoryHelper.Getaspnet_VoidUsersRepository(i.Get<IUnitOfWork>()));
            app.CreatePerOwinContext<Iaspnet_WebEvent_EventsRepository>((x, i)
                => RepositoryHelper.Getaspnet_WebEvent_EventsRepository(i.Get<IUnitOfWork>()));
            app.CreatePerOwinContext<IMenusRepository>((x, i)
                => RepositoryHelper.GetMenusRepository(i.Get<IUnitOfWork>()));

            app.CreatePerOwinContext<IEdiuxAspNetSqlUserStore>(EdiuxAspNetSqlUserStore.Create);

            ConfigureAuth(app);
        }
    }
}
