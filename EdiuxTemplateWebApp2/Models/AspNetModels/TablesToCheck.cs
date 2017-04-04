using System.Collections.Generic;
using System.Web.Security;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public enum TablesToCheck
    {
        aspnet_Membership=1,
        aspnet_Roles=2,
        aspnet_Profile=4,
        aspnet_PersonalizationPerUser=8,
        aspnet_WebEvent_Events=16,

    }
}