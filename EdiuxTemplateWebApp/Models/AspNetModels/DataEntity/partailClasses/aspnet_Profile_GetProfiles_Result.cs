using System;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_Profile_GetProfiles_Result
    {
        public string UserName { get; set; }
        public bool IsAnonymous { get; set; }
        public DateTime LastActivityDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public int Column1 { get; set; }
    }
}