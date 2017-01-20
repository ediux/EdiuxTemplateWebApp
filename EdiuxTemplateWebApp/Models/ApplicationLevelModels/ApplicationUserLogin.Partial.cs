namespace EdiuxTemplateWebApp.Models.ApplicationLevelModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    [MetadataType(typeof(ApplicationUserLoginMetaData))]
    public partial class ApplicationUserLogin
    {
    }
    
    public partial class ApplicationUserLoginMetaData
    {
        [Required]
        public int UserId { get; set; }
        
        [StringLength(512, ErrorMessage="欄位長度不得大於 512 個字元")]
        [Required]
        public string LoginProvider { get; set; }
        
        [StringLength(512, ErrorMessage="欄位長度不得大於 512 個字元")]
        [Required]
        public string ProviderKey { get; set; }
    
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
