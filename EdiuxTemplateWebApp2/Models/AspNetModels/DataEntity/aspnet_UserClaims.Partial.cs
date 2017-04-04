namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using System;
    using System.Collections.Generic;
    
    using System.ComponentModel.DataAnnotations;
    
    [MetadataType(typeof(aspnet_UserClaimsMetaData))]
    public partial class aspnet_UserClaims
    {
    }
    
    public partial class aspnet_UserClaimsMetaData
    {
        [Required]
        public System.Guid UserId { get; set; }
        [Required]
        public int Id { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public virtual aspnet_Users aspnet_Users { get; set; }
    }
}
