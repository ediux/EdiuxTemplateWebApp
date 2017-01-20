namespace EdiuxTemplateWebApp.Models.ApplicationLevelModels
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
        [Required]
        public string Name { get; set; }
        
        [StringLength(50, ErrorMessage="欄位長度不得大於 50 個字元")]
        public string IconCSS { get; set; }
        [Required]
        public bool IsExternalLinks { get; set; }
        
        [StringLength(2048, ErrorMessage="欄位長度不得大於 2048 個字元")]
        public string ExternalURL { get; set; }
        [Required]
        public bool Void { get; set; }
        public Nullable<int> ParentMenuId { get; set; }
        [Required]
        public int CreateUserId { get; set; }
        [Required]
        public System.DateTime CreateTime { get; set; }
        public Nullable<int> LastUpdateUserId { get; set; }
        public Nullable<System.DateTime> LastUpdateTime { get; set; }
        [Required]
        public bool AllowAnonymous { get; set; }
        public Nullable<int> System_ControllerActionsId { get; set; }
        [Required]
        public int Order { get; set; }
        public Nullable<int> ApplicationId { get; set; }
    
        public virtual ICollection<Menus> ChildMenus { get; set; }
        public virtual Menus ParentMenu { get; set; }
        public virtual ICollection<ApplicationRole> ApplicationRole { get; set; }
        public virtual System_Applications System_Applications { get; set; }
    }
}
