namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using System;
    using System.Collections.Generic;
    
    using System.ComponentModel.DataAnnotations;
    
    [MetadataType(typeof(aspnet_MembershipMetaData))]
    public partial class aspnet_Membership
    {
    }
    
    public partial class aspnet_MembershipMetaData
    {
        [Required]
        public System.Guid ApplicationId { get; set; }
        [Required]
        public System.Guid UserId { get; set; }
        
        [StringLength(128, ErrorMessage="欄位長度不得大於 128 個字元")]
        [Required]
        public string Password { get; set; }
        [Required]
        public int PasswordFormat { get; set; }
        
        [StringLength(128, ErrorMessage="欄位長度不得大於 128 個字元")]
        [Required]
        public string PasswordSalt { get; set; }
        
        [StringLength(16, ErrorMessage="欄位長度不得大於 16 個字元")]
        public string MobilePIN { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        public string Email { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        public string LoweredEmail { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        public string PasswordQuestion { get; set; }
        
        [StringLength(128, ErrorMessage="欄位長度不得大於 128 個字元")]
        public string PasswordAnswer { get; set; }
        [Required]
        public bool IsApproved { get; set; }
        [Required]
        public bool IsLockedOut { get; set; }
        [Required]
        public System.DateTime CreateDate { get; set; }
        [Required]
        public System.DateTime LastLoginDate { get; set; }
        [Required]
        public System.DateTime LastPasswordChangedDate { get; set; }
        [Required]
        public System.DateTime LastLockoutDate { get; set; }
        [Required]
        public int FailedPasswordAttemptCount { get; set; }
        [Required]
        public System.DateTime FailedPasswordAttemptWindowStart { get; set; }
        [Required]
        public int FailedPasswordAnswerAttemptCount { get; set; }
        [Required]
        public System.DateTime FailedPasswordAnswerAttemptWindowStart { get; set; }
        public string Comment { get; set; }
        [Required]
        public bool EmailConfirmed { get; set; }
        
        [StringLength(10, ErrorMessage="欄位長度不得大於 10 個字元")]
        public string PhoneNumber { get; set; }
        [Required]
        public bool PhoneConfirmed { get; set; }
        [Required]
        public int AccessFailedCount { get; set; }
        public Nullable<System.Guid> LastUpdateUserId { get; set; }
        public Nullable<System.DateTime> LastUpdateTime { get; set; }
        public Nullable<System.DateTime> LastActivityTime { get; set; }
        public Nullable<System.DateTime> LockoutEndDate { get; set; }
        
        [StringLength(512, ErrorMessage="欄位長度不得大於 512 個字元")]
        public string ResetPasswordToken { get; set; }
        public virtual aspnet_Applications aspnet_Applications { get; set; }
    }
}
