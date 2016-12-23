using EdiuxTemplateWebApp.Filters;
using System.Web;
using System.Web.Mvc;

namespace EdiuxTemplateWebApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ApplicationIdentifyAttribute(typeof(MvcApplication)),0);
            filters.Add(new ComponentRegistrationAttribute(),1);
            filters.Add(new DbAuthorizeAttribute(),2);
            filters.Add(new HandleErrorAttribute(),3);
        }
    }
}
