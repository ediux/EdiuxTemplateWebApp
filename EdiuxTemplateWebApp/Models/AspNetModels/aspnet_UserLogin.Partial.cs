namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    [MetadataType(typeof(aspnet_UserLoginMetaData))]
    public partial class aspnet_UserLogin
    {
    }
    
    public partial class aspnet_UserLoginMetaData
    {
        [Required]
        public System.Guid UserId { get; set; }
        
        [StringLength(512, ErrorMessage="欄位長度不得大於 512 個字元")]
        [Required]
        public string LoginProvider { get; set; }
        
        [StringLength(512, ErrorMessage="欄位長度不得大於 512 個字元")]
        [Required]
        public string ProviderKey { get; set; }
    
        public virtual aspnet_Users aspnet_Users { get; set; }
    }
}
