namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    [MetadataType(typeof(aspnet_PersonalizationPerUserMetaData))]
    public partial class aspnet_PersonalizationPerUser
    {
    }
    
    public partial class aspnet_PersonalizationPerUserMetaData
    {
        [Required]
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> PathId { get; set; }
        public Nullable<System.Guid> UserId { get; set; }
        [Required]
        public byte[] PageSettings { get; set; }
        [Required]
        public System.DateTime LastUpdatedDate { get; set; }
    
        public virtual aspnet_Paths aspnet_Paths { get; set; }
        public virtual aspnet_Users aspnet_Users { get; set; }
    }
}
