namespace EdiuxTemplateWebApp.Models.ApplicationLevelModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    [MetadataType(typeof(ApplicationUserClaimMetaData))]
    public partial class ApplicationUserClaim
    {
    }
    
    public partial class ApplicationUserClaimMetaData
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int Id { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
