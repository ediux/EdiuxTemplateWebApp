namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using EdiuxTemplateWebApp.Models.Identity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using System.Web.Script.Serialization;

    [MetadataType(typeof(aspnet_PersonalizationPerUserMetaData))]
    public partial class aspnet_PersonalizationPerUser
    {
        /// <summary>
        /// 根據參數建立資料執行個體
        /// </summary>
        /// <param name="user"></param>
        /// <param name="sharedPageSetting"></param>
        /// <param name="userPageSettingModel"></param>
        /// <returns></returns>
        internal static aspnet_PersonalizationPerUser Create(aspnet_Users user, aspnet_PersonalizationAllUsers sharedPageSetting, PageSettingByUserViewModel userPageSettingModel = null)
        {
            return new aspnet_PersonalizationPerUser()
            {
                aspnet_Paths = sharedPageSetting.aspnet_Paths,
                LastUpdatedDate = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                UserId = user.Id,
                PageSettings = (userPageSettingModel ?? new PageSettingByUserViewModel(sharedPageSetting, user.Id)).Serialize(),
                PathId = sharedPageSetting.PathId
            };
        }

        /// <summary>
        /// 根據參數建立資料執行個體
        /// </summary>
        /// <param name="user"></param>
        /// <param name="sharedPageSetting"></param>
        /// <param name="userPageSettingModel"></param>
        /// <returns></returns>
        internal static ICollection<aspnet_PersonalizationPerUser> CreateCollection(aspnet_Users user, aspnet_PersonalizationAllUsers sharedPageSetting, PageSettingByUserViewModel userPageSettingModel = null)
        {
            var appPool = new Collection<aspnet_Applications>() as ICollection<aspnet_PersonalizationPerUser>;
            if (user != null && sharedPageSetting != null)
            {
                appPool.Add(Create(user, sharedPageSetting, userPageSettingModel));
            }
            return appPool;
        }


        [ScriptIgnore]
        [IgnoreDataMember]
        public PageSettingByUserViewModel Settings
        {
            get
            {
                return PageSettings.Deserialize<PageSettingByUserViewModel>();
            }

            set
            {
                PageSettings = value.Serialize();
            }
        }
    }

    public partial class aspnet_PersonalizationPerUserMetaData
    {
        [Display(Name = "識別碼")]
        [Required]
        public System.Guid Id { get; set; }
        [Display(Name = "路徑識別碼")]
        public Nullable<System.Guid> PathId { get; set; }
        [Display(Name = "使用者帳號識別碼")]
        public Nullable<System.Guid> UserId { get; set; }
        [Display(Name = "頁面設定")]
        [Required]
        public byte[] PageSettings { get; set; }
        [Display(Name = "最後更新日期")]
        [Required]
        public System.DateTime LastUpdatedDate { get; set; }
        [Display(Name = "路徑清單")]
        public virtual aspnet_Paths aspnet_Paths { get; set; }
        [Display(Name = "使用者清單")]
        public virtual aspnet_Users aspnet_Users { get; set; }
    }
}
