namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using EdiuxTemplateWebApp.Models.Identity;
    using System;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(aspnet_PersonalizationAllUsersMetaData))]
    public partial class aspnet_PersonalizationAllUsers
    {
        /// <summary>
        ///  根據參數建立資料執行個體
        /// </summary>
        /// <param name="path"></param>
        /// <param name="basePageSettingModel"></param>
        /// <returns></returns>
        internal static aspnet_PersonalizationAllUsers Create(aspnet_Paths path, PageSettingsBaseModel basePageSettingModel = null)
        {
            return new aspnet_PersonalizationAllUsers()
            {
                aspnet_Paths = path,
                LastUpdatedDate = DateTime.UtcNow,
                PageSettings = (basePageSettingModel ?? new PageSettingsBaseModel()).Serialize(),
                PathId = path.PathId
            };
        }
    }

    public partial class aspnet_PersonalizationAllUsersMetaData
    {
        [Required]
        public System.Guid PathId { get; set; }
        [Required]
        public byte[] PageSettings { get; set; }
        [Required]
        public System.DateTime LastUpdatedDate { get; set; }
        public virtual aspnet_Paths aspnet_Paths { get; set; }
    }
}
