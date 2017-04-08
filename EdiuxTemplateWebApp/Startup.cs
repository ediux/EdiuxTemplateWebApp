using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(EdiuxTemplateWebApp.Startup))]
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
