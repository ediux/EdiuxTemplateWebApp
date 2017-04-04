namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    
    [MetadataType(typeof(aspnet_Membership_GetUserByUserId_ResultMetaData))]
    public partial class aspnet_Membership_GetUserByUserId_Result
    {
    }
    
    public partial class aspnet_Membership_GetUserByUserId_ResultMetaData
    {
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        public string Email { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        public string PasswordQuestion { get; set; }
        public string Comment { get; set; }
        [Required]
        public bool IsApproved { get; set; }
        [Required]
        public System.DateTime CreateDate { get; set; }
        [Required]
        public System.DateTime LastLoginDate { get; set; }
        [Required]
        public System.DateTime LastActivityDate { get; set; }
        [Required]
        public System.DateTime LastPasswordChangedDate { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        [Required]
        public string UserName { get; set; }
        [Required]
        public bool IsLockedOut { get; set; }
        [Required]
        public System.DateTime LastLockoutDate { get; set; }
    }
}
