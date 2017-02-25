namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using System;
    using System.Collections.Generic;
    
    using System.ComponentModel.DataAnnotations;
    
    [MetadataType(typeof(aspnet_PersonalizationAllUsersMetaData))]
    public partial class aspnet_PersonalizationAllUsers
    {
    }
    
    public partial class aspnet_PersonalizationAllUsersMetaData
    {
        [Required]
        public System.Guid PathId { get; set; }
        [Required]
        public byte[] PageSettings { get; set; }
        [Required]
        public System.DateTime LastUpdatedDate { get; set; }
        public virtual aspnet_Paths aspnet_Paths { get; set; }
    }
}
