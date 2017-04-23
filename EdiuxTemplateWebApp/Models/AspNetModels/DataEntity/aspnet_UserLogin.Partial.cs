namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using System;
    using System.Collections.Generic;
    
    using System.ComponentModel.DataAnnotations;
    
    [MetadataType(typeof(aspnet_UserLoginMetaData))]
    public partial class aspnet_UserLogin
    {
        /// <summary>
        /// 根據參數建立資料執行個體
        /// </summary>
        /// <param name="user">對應的使用者資訊物件</param>
        /// <param name="ProviderName">OpenAuth登入提供者名稱，如: Google 等。</param>
        /// <param name="ProviderKey">登入提供者系統的帳戶識別碼</param>
        /// <returns></returns>
        internal static aspnet_UserLogin Create(aspnet_Users user,string ProviderName,string ProviderKey)
        {
            return new aspnet_UserLogin() {
                aspnet_Users = user,
                UserId = user.Id,
                LoginProvider = ProviderName,
                ProviderKey = ProviderKey
            };
        }

    }
    
    public partial class aspnet_UserLoginMetaData
    {
        [Display(Name = "")]
        [Required]
        public System.Guid UserId { get; set; }
        
        [StringLength(512, ErrorMessage="欄位長度不得大於 512 個字元")]
        [Required]
        public string LoginProvider { get; set; }
        
        [StringLength(512, ErrorMessage="欄位長度不得大於 512 個字元")]
        [Required]
        public string ProviderKey { get; set; }
        public virtual aspnet_Users aspnet_Users { get; set; }
    }
}
