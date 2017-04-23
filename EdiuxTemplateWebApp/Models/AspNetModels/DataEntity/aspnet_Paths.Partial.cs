namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    [MetadataType(typeof(aspnet_PathsMetaData))]
    public partial class aspnet_Paths
    {
        /// <summary>
        /// 根據參數建立資料執行個體
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="applicationInfo"></param>
        /// <param name="sharedSettings"></param>
        /// <param name="perUserSettings"></param>
        /// <param name="menus"></param>
        /// <returns></returns>
        internal static aspnet_Paths Create(Controller controller, aspnet_Applications applicationInfo,
            aspnet_PersonalizationAllUsers sharedSettings = null,
            ICollection<aspnet_PersonalizationPerUser> perUserSettings = null,
            ICollection<Menus> menus = null)
        {
            return new aspnet_Paths()
            {
                ApplicationId = applicationInfo.ApplicationId,
                aspnet_Applications = applicationInfo,
                Path = controller.Request.Path,
                LoweredPath = controller.Request.Path.ToLowerInvariant(),
                PathId = Guid.NewGuid(),
                aspnet_PersonalizationAllUsers = sharedSettings ?? new aspnet_PersonalizationAllUsers(),
                aspnet_PersonalizationPerUser = perUserSettings ?? new Collection<aspnet_PersonalizationPerUser>(),
                Menus = menus ?? new Collection<Menus>()
            };
        }
    }

    public partial class aspnet_PathsMetaData
    {
        [Display(Name = "應用程式")]
        [Required]
        public System.Guid ApplicationId { get; set; }

        [Display(Name = "識別碼")]
        [Required]
        public System.Guid PathId { get; set; }

        [Display(Name = "路徑名稱")]
        [StringLength(256, ErrorMessage = "欄位長度不得大於 256 個字元")]
        [Required]
        public string Path { get; set; }

        [Display(Name = "路徑名稱(小寫)")]
        [StringLength(256, ErrorMessage = "欄位長度不得大於 256 個字元")]
        [Required]
        public string LoweredPath { get; set; }
        public virtual aspnet_Applications aspnet_Applications { get; set; }
        public virtual aspnet_PersonalizationAllUsers aspnet_PersonalizationAllUsers { get; set; }
        public virtual ICollection<aspnet_PersonalizationPerUser> aspnet_PersonalizationPerUser { get; set; }
        public virtual ICollection<Menus> Menus { get; set; }
    }
}
