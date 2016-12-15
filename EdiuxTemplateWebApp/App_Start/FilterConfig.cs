using EdiuxTemplateWebApp.Filters;
using System.Web;
using System.Web.Mvc;

namespace EdiuxTemplateWebApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ComponentRegistrationAttribute(),0);
            filters.Add(new DbAuthorizeAttribute(),1);
            filters.Add(new HandleErrorAttribute(),2);
        }
    }
}
