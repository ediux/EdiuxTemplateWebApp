namespace EdiuxTemplateWebApp.Models.ApplicationLevelModels
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
        public string Name { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        [Required]
        public string LoweredName { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        public string Description { get; set; }
        
        [StringLength(2048, ErrorMessage="欄位長度不得大於 2048 個字元")]
        [Required]
        public string Namespace { get; set; }
    
        public virtual ICollection<Menus> Menus { get; set; }
        public virtual ICollection<System_Controllers> System_Controllers { get; set; }
        public virtual ICollection<ApplicationRole> ApplicationRoles { get; set; }
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }
}
