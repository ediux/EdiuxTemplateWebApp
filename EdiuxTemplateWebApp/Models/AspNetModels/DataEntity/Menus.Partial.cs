namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using System;
    using System.Collections.Generic;
    
    using System.ComponentModel.DataAnnotations;
    
    [MetadataType(typeof(MenusMetaData))]
    public partial class Menus
    {
    }
    
    public partial class MenusMetaData
    {
        [Required]
        public int Id { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        public string Name { get; set; }
        
        [StringLength(512, ErrorMessage="欄位長度不得大於 512 個字元")]
        public string DisplayName { get; set; }
        
        [StringLength(2048, ErrorMessage="欄位長度不得大於 2048 個字元")]
        public string Description { get; set; }
        [Required]
        public short Order { get; set; }
        [Required]
        public bool Void { get; set; }
        [Required]
        public bool IsRightAligned { get; set; }
        [Required]
        public bool IsNoActionPage { get; set; }
        [Required]
        public bool AfterBreak { get; set; }
        public Nullable<int> ParentMenuId { get; set; }
        public Nullable<System.Guid> PathId { get; set; }
        
        [StringLength(50, ErrorMessage="欄位長度不得大於 50 個字元")]
        public string IconCSS { get; set; }
        [Required]
        public bool IsExternalLinks { get; set; }
        [Required]
        public System.Guid CreateUserId { get; set; }
        [Required]
        public System.DateTime CreateTime { get; set; }
        public Nullable<System.Guid> LastUpdateUserId { get; set; }
        public Nullable<System.DateTime> LastUpdateTime { get; set; }
        [Required]
        public bool AllowAnonymous { get; set; }
        public Nullable<System.Guid> ApplicationId { get; set; }
        public virtual aspnet_Applications aspnet_Applications { get; set; }
        public virtual aspnet_Paths aspnet_Paths { get; set; }
        public virtual ICollection<Menus> ChildMenus { get; set; }
        public virtual Menus ParentMenu { get; set; }
        public virtual ICollection<aspnet_Roles> aspnet_Roles { get; set; }
    }
}
