namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(aspnet_ApplicationsMetaData))]
    public partial class aspnet_Applications : IApplicationData<Guid>
    {
        /// <summary>
        /// 根據參數建立資料執行個體
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        internal static aspnet_Applications Create(string applicationName, string description = "")
        {
            return new aspnet_Applications()
            {
                ApplicationId = Guid.NewGuid(),
                ApplicationName = applicationName,
                LoweredApplicationName = applicationName.ToLowerInvariant(),
                Description = description
            };
        }
    }

    public partial class aspnet_ApplicationsMetaData
    {
        [Display(Name = "應用程式名稱")]
        [StringLength(256, ErrorMessage = "欄位長度不得大於 256 個字元")]
        [Required]
        public string ApplicationName { get; set; }

        [Display(Name = "應用程式名稱")]
        [StringLength(256, ErrorMessage = "欄位長度不得大於 256 個字元")]
        [Required]
        public string LoweredApplicationName { get; set; }

        [Display(Name = "識別碼")]
        [Required]
        public System.Guid ApplicationId { get; set; }

        [StringLength(256, ErrorMessage = "欄位長度不得大於 256 個字元")]
        public string Description { get; set; }
        public virtual ICollection<aspnet_Membership> aspnet_Membership { get; set; }
        public virtual ICollection<aspnet_Paths> aspnet_Paths { get; set; }
        public virtual ICollection<aspnet_Roles> aspnet_Roles { get; set; }
        public virtual ICollection<aspnet_Users> aspnet_Users { get; set; }
        public virtual ICollection<aspnet_VoidUsers> aspnet_VoidUsers { get; set; }
        public virtual ICollection<Menus> Menus { get; set; }
    }
}
