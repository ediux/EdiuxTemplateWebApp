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
        [Display(Name = "狀態")]
        [UIHint("VoidDisplay")]
        public bool Void { get; set; }
        [Required]
        [UIHint("UserIDMappingDisplay")]
        [Display(Name = "建立者")]
        public int CreateUserId { get; set; }
        [Required]
        [Display(Name = "建立時間")]
        [UIHint("UTCLocalTimeDisplay")]
        public System.DateTime CreateTime { get; set; }
        [Required]
        [UIHint("UserIDMappingDisplay")]
        [Display(Name = "更新者")]
        public int LastUpdateUserId { get; set; }
        [Required]
        [Display(Name = "最後修改時間")]
        [UIHint("UTCLocalTimeDisplay")]
        public System.DateTime LastUpdateTime { get; set; }

        [Display(Name="可用控制器動作")]
        public virtual ICollection<System_ControllerActions> System_ControllerActions { get; set; }
        [Display(Name = "成員")]
        public virtual ICollection<ApplicationUser> ApplicationUser { get; set; }
        [Display(Name = "可用選單")]
        public virtual ICollection<Menus> Menus { get; set; }
        [Display(Name = "所屬應用程式")]
        public virtual ICollection<System_Applications> System_Applications { get; set; }
    }
}
