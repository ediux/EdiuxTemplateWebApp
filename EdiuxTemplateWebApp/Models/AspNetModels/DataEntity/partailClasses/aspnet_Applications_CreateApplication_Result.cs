using System;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_Applications_CreateApplication_Result
    {
        [OutputParameter(Order = 1)]
        public Guid ApplicationId { get; set; }
    }
}