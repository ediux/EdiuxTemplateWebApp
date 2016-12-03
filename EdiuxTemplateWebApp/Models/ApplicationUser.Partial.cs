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

        public void CloneFrom(ApplicationUser source)
        {           
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
        }
    }
    
    public partial class ApplicationUserMetaData
    {
        [Required]
        public int Id { get; set; }
        
        [StringLength(50, ErrorMessage="欄位長度不得大於 50 個字元")]
        [Required]
        public string UserName { get; set; }
        
        [StringLength(50, ErrorMessage="欄位長度不得大於 50 個字元")]
        public string Password { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        [Required]
        public bool TwoFactorEnabled { get; set; }
        [Required]
        public bool Void { get; set; }
        
        [StringLength(50, ErrorMessage="欄位長度不得大於 50 個字元")]
        [Required]
        public string DisplayName { get; set; }
        
        [StringLength(512, ErrorMessage="欄位長度不得大於 512 個字元")]
        public string EMail { get; set; }
        [Required]
        public bool EMailConfirmed { get; set; }
        
        [StringLength(10, ErrorMessage="欄位長度不得大於 10 個字元")]
        public string PhoneNumber { get; set; }
        [Required]
        public bool PhoneConfirmed { get; set; }
        [Required]
        public int CreateUserId { get; set; }
        [Required]
        public System.DateTime CreateTime { get; set; }
        [Required]
        public int LastUpdateUserId { get; set; }
        [Required]
        public System.DateTime LastUpdateTime { get; set; }
        public Nullable<System.DateTime> LastActivityTime { get; set; }
        public Nullable<System.DateTime> LastUnlockedTime { get; set; }
        public Nullable<System.DateTime> LastLoginFailTime { get; set; }
        [Required]
        public int AccessFailedCount { get; set; }
        public Nullable<bool> LockoutEnabled { get; set; }
        public Nullable<System.DateTime> LockoutEndDate { get; set; }
        
        [StringLength(512, ErrorMessage="欄位長度不得大於 512 個字元")]
        public string ResetPasswordToken { get; set; }
    
        public virtual ICollection<ApplicationUserClaim> ApplicationUserClaim { get; set; }
        public virtual ICollection<ApplicationUserLogin> ApplicationUserLogin { get; set; }
        public virtual ICollection<ApplicationRole> ApplicationRole { get; set; }
    }
}
