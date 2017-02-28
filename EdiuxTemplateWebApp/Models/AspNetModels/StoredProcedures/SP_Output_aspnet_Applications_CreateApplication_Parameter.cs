using System;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_Applications_CreateApplication_OutputParameter
    {
        [OutputParameter(Order = 1)]
        public virtual Guid ApplicationId { get; set; }
    }
}
