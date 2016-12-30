namespace EdiuxTemplateWebApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    [MetadataType(typeof(System_ControllersMetaData))]
    public partial class System_Controllers
    {
        public bool isUserAuthorizend(ApplicationUser user)
        {
            return AllowAnonymous;
        }
    }
    
    public partial class System_ControllersMetaData
    {
        [Required]
        public int Id { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        [Required]
        public string Name { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        [Required]
        public string ClassName { get; set; }
        
        [StringLength(512, ErrorMessage="欄位長度不得大於 512 個字元")]
        [Required]
        public string Namespace { get; set; }
        [Required]
        public bool Void { get; set; }
        [Required]
        public int CreateUserId { get; set; }
        [Required]
        public System.DateTime CreateTime { get; set; }
        public Nullable<int> LastUpdateUserId { get; set; }
        public Nullable<System.DateTime> LastUpdateTime { get; set; }
        [Required]
        public bool AllowAnonymous { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        public virtual ICollection<System_ControllerActions> System_ControllerActions { get; set; }
        public virtual System_Applications System_Applications { get; set; }
    }
}
