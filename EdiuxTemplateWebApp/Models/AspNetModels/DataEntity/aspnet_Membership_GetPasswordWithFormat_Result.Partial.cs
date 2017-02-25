namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    
    [MetadataType(typeof(aspnet_Membership_GetPasswordWithFormat_ResultMetaData))]
    public partial class aspnet_Membership_GetPasswordWithFormat_Result
    {
    }
    
    public partial class aspnet_Membership_GetPasswordWithFormat_ResultMetaData
    {
        
        [StringLength(128, ErrorMessage="欄位長度不得大於 128 個字元")]
        public string Password { get; set; }
        public Nullable<int> PasswordFormat { get; set; }
        
        [StringLength(128, ErrorMessage="欄位長度不得大於 128 個字元")]
        public string PasswordSalt { get; set; }
        public Nullable<int> FailedPasswordAttemptCount { get; set; }
        public Nullable<int> FailedPasswordAnswerAttemptCount { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public Nullable<System.DateTime> LastLoginDate { get; set; }
        public Nullable<System.DateTime> C_LastActivityDate { get; set; }
    }
}
