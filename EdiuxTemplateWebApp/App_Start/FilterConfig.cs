using EdiuxTemplateWebApp.Filters;
using System.Web;
using System.Web.Mvc;

namespace EdiuxTemplateWebApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ApplicationIdentifyAttribute(), 1);
            filters.Add(new ComponentRegistrationAttribute(), 2);
            filters.Add(new DbAuthorizeAttribute(), 3);
            filters.Add(new HandleErrorAttribute(), 4);
        }
    }
}
