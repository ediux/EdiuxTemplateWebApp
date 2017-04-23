namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using EdiuxTemplateWebApp.Models.Identity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using aspnet_RolesHelper = aspnet_Roles;

    [MetadataType(typeof(MenusMetaData))]
    public partial class Menus
    {
        internal static Menus Create(aspnet_Applications applicationInfo, aspnet_Paths pathInfo, string Name,
            string Description = "",
            string DisplayName = "",
            short Order = 0,
            string CssClass = "",
            bool isAfterBreak = false,
            bool isExternalLinks = false,
            bool isNoActionPage = false,
            bool isRightAligned = false,
            ICollection<aspnet_RolesHelper> ApplyWithRoles = null,
            ICollection<Menus> ChildMenus = null,
            Menus ParentMenu = null,
            aspnet_Users CreateUser = null)
        {
            if(applicationInfo==null)
            {
                throw new ArgumentNullException(nameof(applicationInfo));
            }

            if(pathInfo == null)
            {
                throw new ArgumentNullException(nameof(pathInfo));
            }

            if (string.IsNullOrEmpty(DisplayName))
            {
                DisplayName = Name;
            }

            var SharedPageSetting = pathInfo.aspnet_PersonalizationAllUsers.PageSettings.Deserialize<PageSettingsBaseModel>();
   
            var currentTime = DateTime.UtcNow;

            return new Menus()
            {
                AfterBreak = isAfterBreak,
                AllowAnonymous = SharedPageSetting.AllowAnonymous,
                ApplicationId = applicationInfo.ApplicationId,
                aspnet_Applications = applicationInfo,
                aspnet_Paths = pathInfo,
                aspnet_Roles = ApplyWithRoles ?? aspnet_RolesHelper.CreateCollection(applicationInfo, new string[] { }),
                ChildMenus = ChildMenus ?? new Collection<Menus>(),
                CreateTime = currentTime,
                CreateUserId = CreateUser.Id,
                Description = Description,
                IconCSS = CssClass,
                IsExternalLinks = isExternalLinks,
                IsNoActionPage = isNoActionPage,
                IsRightAligned = isRightAligned,
                LastUpdateTime = currentTime,
                LastUpdateUserId = CreateUser.Id,
                Order = Order,
                ParentMenu = ParentMenu,
                Id = -1,
                ParentMenuId = (ParentMenu != null) ? ParentMenu.Id : -1,
                Void = false,
                PathId = pathInfo.PathId,
                Name = Name,
                DisplayName = ""
            };
        }
    }

    public partial class MenusMetaData
    {
        [Display(Name = "識別碼")]
        [Required]
        public int Id { get; set; }

        [StringLength(256, ErrorMessage = "欄位長度不得大於 256 個字元")]
        public string Name { get; set; }

        [StringLength(512, ErrorMessage = "欄位長度不得大於 512 個字元")]
        public string DisplayName { get; set; }

        [StringLength(2048, ErrorMessage = "欄位長度不得大於 2048 個字元")]
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

        [StringLength(50, ErrorMessage = "欄位長度不得大於 50 個字元")]
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
