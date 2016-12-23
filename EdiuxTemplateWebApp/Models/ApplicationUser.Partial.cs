namespace EdiuxTemplateWebApp.Models
{
    using Microsoft.AspNet.Identity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Security.Claims;
    using System.Threading.Tasks;

    [MetadataType(typeof(ApplicationUserMetaData))]
    public partial class ApplicationUser : IUser<int>
    {
        public static ApplicationUser Create()
        {
            return new ApplicationUser()
            {
                Id = -1,
                AccessFailedCount = 0,
                ApplicationUserClaim = new Collection<ApplicationUserClaim>(),
                ApplicationUserLogin = new Collection<ApplicationUserLogin>(),
                CreateTime = DateTime.Now.ToUniversalTime(),
                CreateUserId = -1,
                LastActivityTime = DateTime.Now.ToUniversalTime(),
                LastUpdateTime = DateTime.Now.ToUniversalTime(),
                LastUpdateUserId = -1,
                Password = string.Empty,
                PasswordHash = string.Empty,
                LockoutEnabled = false,
                ResetPasswordToken = string.Empty,
                SecurityStamp = string.Empty,
                TwoFactorEnabled = false,
                UserName = string.Empty,
                Void = false
            };
        }

        public static ApplicationUser CreateKernelUser()
        {
            return new ApplicationUser()
            {
                Id = -1,
                AccessFailedCount = 0,
                ApplicationUserClaim = new Collection<ApplicationUserClaim>(),
                ApplicationUserLogin = new Collection<ApplicationUserLogin>(),
                CreateTime = DateTime.Now.ToUniversalTime(),
                CreateUserId = -1,
                LastActivityTime = DateTime.Now.ToUniversalTime(),
                LastUpdateTime = DateTime.Now.ToUniversalTime(),
                LastUpdateUserId = -1,
                Password = string.Empty,
                PasswordHash = string.Empty,
                LockoutEnabled = false,
                ResetPasswordToken = string.Empty,
                SecurityStamp = string.Empty,
                TwoFactorEnabled = false,
                UserName = "System",
                Void = false
            };
        }

        public virtual void CloneFrom(ApplicationUser source)
        {
            Id = source.Id;
            AccessFailedCount = source.AccessFailedCount;
            CreateTime = source.CreateTime;
            CreateUserId = source.CreateUserId;
            DisplayName = source.DisplayName;
            EMail = source.EMail;
            EMailConfirmed = source.EMailConfirmed;
            LastActivityTime = source.LastActivityTime;
            LastLoginFailTime = source.LastLoginFailTime;
            LastUnlockedTime = source.LastUnlockedTime;
            LastUpdateTime = source.LastUpdateTime;
            LastUpdateUserId = source.LastUpdateUserId;
            LockoutEnabled = source.LockoutEnabled;
            LockoutEndDate = source.LockoutEndDate;
            Password = source.Password;
            PasswordHash = source.PasswordHash;
            PhoneConfirmed = source.PhoneConfirmed;
            PhoneNumber = source.PhoneNumber;
            ResetPasswordToken = source.ResetPasswordToken;
            SecurityStamp = source.SecurityStamp;
            TwoFactorEnabled = source.TwoFactorEnabled;
            UserName = source.UserName;
            Void = source.Void;

            ApplicationRole = source.ApplicationRole;
            ApplicationUserClaim = source.ApplicationUserClaim;
            ApplicationUserLogin = source.ApplicationUserLogin;
        }
    }

    public partial class ApplicationUserMetaData
    {
        [Required]
        public int Id { get; set; }

        [StringLength(50, ErrorMessage = "欄位長度不得大於 50 個字元")]
        [Required]
        [Display(Name = "帳號名稱")]
        public string UserName { get; set; }

        [StringLength(50, ErrorMessage = "欄位長度不得大於 50 個字元")]
        [Display(Name = "密碼")]
        public string Password { get; set; }
        [Display(Name = "雜湊的密碼")]
        public string PasswordHash { get; set; }
        [Display(Name = "安全性戳記")]
        public string SecurityStamp { get; set; }
        [Required]
        [Display(Name = "兩步驟驗證機制")]
        [UIHint("EnabledDisplay")]
        public bool TwoFactorEnabled { get; set; }
        [Required]
        [Display(Name = "狀態")]
        [UIHint("VoidDisplay")]
        public bool Void { get; set; }

        [StringLength(50, ErrorMessage = "欄位長度不得大於 50 個字元")]
        [Required]
        [Display(Name = "顯示名稱")]
        public string DisplayName { get; set; }

        [StringLength(512, ErrorMessage = "欄位長度不得大於 512 個字元")]
        [Display(Name = "電子郵件")]
        public string EMail { get; set; }
        [Required]
        [UIHint("YesNoDisplay")]
        [Display(Name = "電子郵件驗證")]
        public bool EMailConfirmed { get; set; }

        [StringLength(10, ErrorMessage = "欄位長度不得大於 10 個字元")]
        [Display(Name = "電話號碼")]
        public string PhoneNumber { get; set; }
        [Required]
        [UIHint("YesNoDisplay")]
        [Display(Name = "電話號碼驗證")]
        public bool PhoneConfirmed { get; set; }
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
        [Display(Name = "最後活動時間")]
        [UIHint("UTCLocalTimeDisplay")]
        public Nullable<System.DateTime> LastActivityTime { get; set; }
        [Display(Name = "最後解鎖時間")]
        [UIHint("UTCLocalTimeDisplay")]
        public Nullable<System.DateTime> LastUnlockedTime { get; set; }
        [Display(Name = "最後登入失敗時間")]
        [UIHint("UTCLocalTimeDisplay")]
        public Nullable<System.DateTime> LastLoginFailTime { get; set; }
        [Required]
        [Display(Name = "登入失敗次數")]
        public int AccessFailedCount { get; set; }
        [Display(Name = "鎖定狀態")]
        public Nullable<bool> LockoutEnabled { get; set; }
        [Display(Name = "結束鎖定日期")]
        [UIHint("UTCLocalDateDisplay")]
        public Nullable<System.DateTime> LockoutEndDate { get; set; }

        [StringLength(512, ErrorMessage = "欄位長度不得大於 512 個字元")]
        [Display(Name = "重設密碼權杖")]
        public string ResetPasswordToken { get; set; }

        [Display(Name = "宣告式身分資料")]
        public virtual ICollection<ApplicationUserClaim> ApplicationUserClaim { get; set; }
        [Display(Name = "關聯的外部登入")]
        public virtual ICollection<ApplicationUserLogin> ApplicationUserLogin { get; set; }
        [Display(Name = "角色")]
        public virtual ICollection<ApplicationRole> ApplicationRole { get; set; }
        [Display(Name = "所屬應用程式")]
        public virtual ICollection<System_Applications> System_Applications { get; set; }
    }
}
