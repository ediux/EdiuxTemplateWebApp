namespace EdiuxTemplateWebApp.Models
{
    using Microsoft.AspNet.Identity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(ApplicationRoleMetaData))]
    public partial class ApplicationRole : IRole<int>
    {
        public static ApplicationRole Create()
        {
            return new ApplicationRole()
            {
                Id = -1,
                CreateTime = DateTime.Now.ToUniversalTime(),
                CreateUserId = -1,
                LastUpdateTime = DateTime.Now.ToUniversalTime(),
                LastUpdateUserId = -1,
                Name = string.Empty,
                ApplicationUser = new Collection<ApplicationUser>(),
                System_ControllerActions = new Collection<System_ControllerActions>(),
                Menus = new Collection<Menus>(),
                Void = false
            };
        }
    }

    public partial class ApplicationRoleMetaData
    {
        [Required]
        public int Id { get; set; }

        [StringLength(50, ErrorMessage = "欄位長度不得大於 50 個字元")]
        [Required]
        [Display(Name ="角色名稱")]
        public string Name { get; set; }
        [Required]
        public bool Void { get; set; }
        [Required]
        public int CreateUserId { get; set; }
        [Required]
        public System.DateTime CreateTime { get; set; }
        [Required]
        public int LastUpdateUserId { get; set; }
        [Required]
        public System.DateTime LastUpdateTime { get; set; }

        public virtual ICollection<System_ControllerActions> System_ControllerActions { get; set; }
        public virtual ICollection<ApplicationUser> ApplicationUser { get; set; }
        public virtual ICollection<Menus> Menus { get; set; }
    }
}
