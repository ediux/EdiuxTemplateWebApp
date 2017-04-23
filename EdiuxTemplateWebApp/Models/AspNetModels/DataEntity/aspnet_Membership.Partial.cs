namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Security;

    [MetadataType(typeof(aspnet_MembershipMetaData))]
    public partial class aspnet_Membership
    {
        /// <summary>
        /// 根據參數建立資料執行個體
        /// </summary>
        /// <param name="applicationInfo">對應的應用程式</param>
        /// <param name="userId">對應的帳號</param>
        /// <param name="currentTime">目前時間</param>
        /// <param name="password">密碼</param>
        /// <param name="passwordAnswer">安全性問題答案</param>
        /// <param name="passwordQuestion">安全性問題</param>
        /// <param name="isLockedOut">是否鎖定</param>
        /// <param name="twoFactorEnabled">啟用兩步驟驗證</param>
        /// <param name="passwordSalt">密碼鹽</param>
        /// <param name="mobileAlias">行動裝置別名</param>
        /// <param name="comment">註解說明</param>
        /// <param name="email">電子郵件地址</param>
        /// <param name="mobilePIN">行動裝置個人識別碼</param>
        /// <param name="phoneNumber">行動電話號碼</param>
        /// <param name="passwordFormat">密碼儲存格式</param>
        /// <returns></returns>
        internal static aspnet_Membership Create(aspnet_Applications applicationInfo,
            Guid userId,
            DateTime currentTime,
            string password,
            string passwordAnswer = "",
            string passwordQuestion = "",
            bool isLockedOut = false,
            bool twoFactorEnabled = false,
            string passwordSalt = "EdiuxTemplateWebSite",
            string mobileAlias = "",
            string comment = "",
            string email = "system@localhost.local",
            string mobilePIN = "",
            string phoneNumber = "0910123456",
            MembershipPasswordFormat passwordFormat = MembershipPasswordFormat.Hashed)
        {
            DateTime nullTime = new DateTime(1754, 1, 1);

            return new aspnet_Membership()
            {
                AccessFailedCount = 0,
                ApplicationId = applicationInfo.ApplicationId,
                aspnet_Applications = applicationInfo,
                Comment = comment,
                CreateDate = currentTime,
                Email = email,
                LoweredEmail = email.ToLowerInvariant(),
                EmailConfirmed = !twoFactorEnabled,
                FailedPasswordAnswerAttemptCount = 0,
                FailedPasswordAnswerAttemptWindowStart = nullTime,
                FailedPasswordAttemptCount = 0,
                FailedPasswordAttemptWindowStart = nullTime,
                IsApproved = !twoFactorEnabled,
                IsLockedOut = isLockedOut,
                LastLockoutDate = nullTime,
                LastLoginDate = nullTime,
                LastPasswordChangedDate = currentTime,
                MobilePIN = mobilePIN,
                Password = password,
                PasswordSalt = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(passwordSalt)),
                PasswordAnswer = passwordAnswer,
                PasswordFormat = (int)passwordFormat,
                PasswordQuestion = passwordQuestion,
                PhoneConfirmed = !twoFactorEnabled,
                PhoneNumber = phoneNumber,
                ResetPasswordToken = string.Empty,
                UserId = userId
            };
        }
    }

    public partial class aspnet_MembershipMetaData
    {
        [Display(Name = "應用程式")]
        [Required]
        public System.Guid ApplicationId { get; set; }

        [Display(Name = "帳號")]
        [Required]
        public System.Guid UserId { get; set; }

        [Display(Name = "密碼")]
        [StringLength(128, ErrorMessage = "欄位長度不得大於 128 個字元")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "密碼儲存格式")]
        [Required]
        public int PasswordFormat { get; set; }

        [Display(Name = "密碼鹽")]
        [StringLength(128, ErrorMessage = "欄位長度不得大於 128 個字元")]
        [Required]
        public string PasswordSalt { get; set; }

        [Display(Name = "行動裝置PIN")]
        [StringLength(16, ErrorMessage = "欄位長度不得大於 16 個字元")]
        public string MobilePIN { get; set; }

        [Display(Name = "電子郵件")]
        [StringLength(256, ErrorMessage = "欄位長度不得大於 256 個字元")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "電子郵件")]
        [StringLength(256, ErrorMessage = "欄位長度不得大於 256 個字元")]
        [DataType(DataType.EmailAddress)]
        public string LoweredEmail { get; set; }

        [Display(Name = "安全性問題")]
        [StringLength(256, ErrorMessage = "欄位長度不得大於 256 個字元")]
        public string PasswordQuestion { get; set; }

        [Display(Name = "安全性問題答案")]
        [StringLength(128, ErrorMessage = "欄位長度不得大於 128 個字元")]
        public string PasswordAnswer { get; set; }

        [Display(Name = "已驗證為本人")]
        [Required]
        public bool IsApproved { get; set; }

        [Display(Name = "鎖定")]
        [Required]
        public bool IsLockedOut { get; set; }

        [Display(Name = "建立時間")]
        [Required]
        public System.DateTime CreateDate { get; set; }

        [Display(Name = "最後登入日期")]
        [Required]
        public System.DateTime LastLoginDate { get; set; }

        [Display(Name = "最後密碼變更日期")]
        [Required]
        public System.DateTime LastPasswordChangedDate { get; set; }

        [Display(Name = "最後鎖定日期")]
        [Required]
        public System.DateTime LastLockoutDate { get; set; }

        [Display(Name = "密碼錯誤嘗試次數")]
        [Required]
        public int FailedPasswordAttemptCount { get; set; }

        [Display(Name = "最後密碼錯誤嘗試時間")]
        [Required]
        public System.DateTime FailedPasswordAttemptWindowStart { get; set; }

        [Required]
        [Display(Name = "最後安全性問題回答錯誤次數")]
        public int FailedPasswordAnswerAttemptCount { get; set; }

        [Display(Name = "最後安全性問題回答錯誤時間")]
        [Required]
        public System.DateTime FailedPasswordAnswerAttemptWindowStart { get; set; }

        [Display(Name = "說明註解")]
        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }

        [Display(Name = "電子郵件確認")]
        [Required]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "行動電話")]
        [StringLength(10, ErrorMessage = "欄位長度不得大於 10 個字元")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Display(Name ="行動電話裝置確認")]
        [Required]
        public bool PhoneConfirmed { get; set; }

        [Display(Name = "登入驗證失敗次數")]
        [Required]
        public int AccessFailedCount { get; set; }

        [Display(Name = "最後變更者")]
        public Guid? LastUpdateUserId { get; set; }

        [Display(Name = "最後變更時間")]
        public DateTime? LastUpdateTime { get; set; }

        [Display(Name = "最後活動時間")]
        public DateTime? LastActivityTime { get; set; }

        [Display(Name = "最後解除鎖定時間")]
        public DateTime? LockoutEndDate { get; set; }

        [Display(Name = "密碼重置權杖")]
        [StringLength(512, ErrorMessage = "欄位長度不得大於 512 個字元")]
        public string ResetPasswordToken { get; set; }

        [Display(Name = "應用程式")]
        public virtual aspnet_Applications aspnet_Applications { get; set; }
    }
}
