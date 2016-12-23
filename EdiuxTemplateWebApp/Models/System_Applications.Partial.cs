namespace EdiuxTemplateWebApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    [MetadataType(typeof(System_ApplicationsMetaData))]
    public partial class System_Applications
    {
    }
    
    public partial class System_ApplicationsMetaData
    {
        [Required]
        public int Id { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        [Required]
        [Display(Name="應用程式名稱")]
        public string Name { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        [Required]
        [Display(Name = "應用程式名稱")]
        public string LoweredName { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        [Display(Name = "描述說明")]
        public string Description { get; set; }
        
        [StringLength(2048, ErrorMessage="欄位長度不得大於 2048 個字元")]
        [Display(Name = "命名空間")]
        [Required]
        public string Namespace { get; set; }
    
        [Display(Name="選單")]
        public virtual ICollection<Menus> Menus { get; set; }
        [Display(Name ="控制器")]
        public virtual ICollection<System_Controllers> System_Controllers { get; set; }
        [Display(Name = "可用角色")]
        public virtual ICollection<ApplicationRole> ApplicationRoles { get; set; }
        [Display(Name = "可用使用者帳號")]
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }
}
