using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EdiuxTemplateWebApp.Startup))]
namespace EdiuxTemplateWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            ConfigureDataStore(app);
        }
    }
}
