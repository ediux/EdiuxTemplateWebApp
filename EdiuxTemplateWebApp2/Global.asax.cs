using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using System.Web.Optimization;

namespace EdiuxTemplateWebApp
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles); 
		}
	}
}
