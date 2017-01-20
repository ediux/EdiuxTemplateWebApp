namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using Microsoft.AspNet.Identity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(aspnet_RolesMetaData))]
    public partial class aspnet_Roles : IRole<Guid>
    {
    }
    
    public partial class aspnet_RolesMetaData
    {
        [Required]
        public System.Guid ApplicationId { get; set; }
        [Required]
        public System.Guid Id { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        [Required]
        public string Name { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        [Required]
        public string LoweredRoleName { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        public string Description { get; set; }
    
        public virtual aspnet_Applications aspnet_Applications { get; set; }
        public virtual ICollection<aspnet_Users> aspnet_Users { get; set; }
    }
}
