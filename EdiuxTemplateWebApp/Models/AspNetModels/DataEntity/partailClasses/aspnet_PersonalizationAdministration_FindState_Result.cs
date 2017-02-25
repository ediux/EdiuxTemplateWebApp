using System;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_PersonalizationAdministration_FindState_Result
    {
        public string Path { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public int? SharedDataLength { get; set; }
        public int UserDataLength { get; set; }
        public int UserCount { get; set; }
        public string UserName { get; set; }
        public Nullable<DateTime> LastActivityDate { get; set; }
    }
}