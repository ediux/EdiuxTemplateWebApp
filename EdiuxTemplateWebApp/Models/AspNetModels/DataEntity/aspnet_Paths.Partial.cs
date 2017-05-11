namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using EdiuxTemplateWebApp.Models.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using System.Web.Mvc;

    [MetadataType(typeof(aspnet_PathsMetaData))]
    public partial class aspnet_Paths
    {
        /// <summary>
        /// 根據參數建立資料執行個體
        /// </summary>
        /// <param name="controller">控制器內容物件</param>
        /// <param name="applicationInfo">應用程式資訊物件</param>
        /// <param name="sharedSettings">共用設定資料物件</param>
        /// <param name="perUserSettings">使用者設定集合</param>
        /// <param name="menus">關聯選單集合</param>
        /// <returns>初始化的新的資料實體物件。</returns>
        internal static aspnet_Paths Create(string Url, aspnet_Applications applicationInfo,
            aspnet_PersonalizationAllUsers sharedSettings = null,
            ICollection<aspnet_PersonalizationPerUser> perUserSettings = null,
            ICollection<Menus> menus = null)
        {
            var newinstance = new aspnet_Paths()
            {
                ApplicationId = applicationInfo.ApplicationId,
                aspnet_Applications = applicationInfo,
                Path = Url,
                LoweredPath = Url.ToLowerInvariant(),
                PathId = Guid.NewGuid()
            };

            newinstance.aspnet_PersonalizationAllUsers = sharedSettings ?? aspnet_PersonalizationAllUsers.Create(newinstance);
            newinstance.aspnet_PersonalizationPerUser = perUserSettings ?? new Collection<aspnet_PersonalizationPerUser>();
            newinstance.Menus = menus ?? new Collection<Menus>();

            return newinstance;
        }

        /// <summary>
        /// 根據參數建立資料執行個體
        /// </summary>
        /// <param name="filiterContext">資料篩選器內容物件</param>
        /// <param name="applicationInfo">應用程式資訊物件</param>
        /// <param name="sharedSettings">共用設定資料物件</param>
        /// <param name="perUserSettings">使用者設定集合</param>
        /// <param name="menus">關聯選單集合</param>
        /// <returns>初始化的新的資料實體物件。</returns>
        internal static aspnet_Paths Create(ActionExecutingContext filiterContext, aspnet_Applications applicationInfo = null,
            aspnet_PersonalizationAllUsers sharedSettings = null,
            ICollection<aspnet_PersonalizationPerUser> perUserSettings = null,
            ICollection<Menus> menus = null)
        {
            if (filiterContext == null)
            {
                throw new ArgumentNullException(nameof(filiterContext));
            }

            if (applicationInfo == null)
            {
                if (filiterContext.Controller.ViewBag.ApplicationInfo != null)
                {
                    applicationInfo = (aspnet_Applications)filiterContext.Controller.ViewBag.ApplicationInfo;
                }
                else
                {
                    IApplicationStore<aspnet_Applications, Guid> Store = filiterContext.HttpContext.GetOwinContext().Get<IEdiuxAspNetSqlUserStore>();
                    applicationInfo = Store.GetEntityByQuery(Store.GetApplicationNameFromConfiguratinFile());
                }
            }

            string LoweredPath = filiterContext.HttpContext.Request.Path.ToLowerInvariant();

            var newinstance = new aspnet_Paths()
            {
                ApplicationId = applicationInfo.ApplicationId,
                aspnet_Applications = applicationInfo,
                Path = filiterContext.HttpContext.Request.Path,
                LoweredPath = LoweredPath,
                PathId = Guid.NewGuid()
            };

            newinstance.aspnet_PersonalizationAllUsers = sharedSettings ?? aspnet_PersonalizationAllUsers.Create(newinstance, filiterContext);
            newinstance.aspnet_PersonalizationPerUser = perUserSettings ?? new Collection<aspnet_PersonalizationPerUser>();

            if (filiterContext != null)
            {
                aspnet_PersonalizationPerUser PerUserSetting = new aspnet_PersonalizationPerUser();
                PerUserSetting.Id = Guid.NewGuid();
                PerUserSetting.LastUpdatedDate = DateTime.UtcNow;
                PerUserSetting.PathId = newinstance.PathId;
                PerUserSetting.UserId = filiterContext.Controller.ControllerContext.HttpContext.User.Identity.GetUserId();

                PageSettingByUserViewModel perPageSetting = new PageSettingByUserViewModel(newinstance.aspnet_PersonalizationAllUsers, PerUserSetting.UserId.Value);
                PerUserSetting.PageSettings = perPageSetting.Serialize();
                newinstance.aspnet_PersonalizationPerUser.Add(PerUserSetting);
            }

            newinstance.Menus = menus ?? new Collection<Menus>();

            return newinstance;
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

        [Display(Name = "應用程式資訊")]
        public virtual aspnet_Applications aspnet_Applications { get; set; }
        public virtual aspnet_PersonalizationAllUsers aspnet_PersonalizationAllUsers { get; set; }
        public virtual ICollection<aspnet_PersonalizationPerUser> aspnet_PersonalizationPerUser { get; set; }
        [Display(Name = "功能選單")]
        public virtual ICollection<Menus> Menus { get; set; }
    }
}
