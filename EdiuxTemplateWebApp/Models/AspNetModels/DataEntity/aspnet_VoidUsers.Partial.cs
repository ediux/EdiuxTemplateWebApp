namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using System;
    using System.Collections.Generic;
    
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(aspnet_VoidUsersMetaData))]
    public partial class aspnet_VoidUsers : ICloneable
    {
        public object Clone()
        {
            throw new NotImplementedException();
        }
    }

    public partial class aspnet_VoidUsersMetaData
    {
        [Required]
        public System.Guid ApplicationId { get; set; }
        [Required]
        public System.Guid UserId { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        [Required]
        public string UserName { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        [Required]
        public string LoweredUserName { get; set; }
        
        [StringLength(16, ErrorMessage="欄位長度不得大於 16 個字元")]
        public string MobileAlias { get; set; }
        [Required]
        public bool IsAnonymous { get; set; }
        [Required]
        public System.DateTime LastActivityDate { get; set; }
        public virtual aspnet_Applications aspnet_Applications { get; set; }
    }
}
