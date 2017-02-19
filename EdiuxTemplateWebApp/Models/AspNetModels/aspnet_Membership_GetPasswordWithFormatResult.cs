using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public class aspnet_Membership_GetPasswordWithFormatResult: aspnet_Membership_GetPasswordResult
    {
        public string PasswordSalt { get; set; }
        public int FailedPasswordAttemptCount { get; set; }
        public int FailedPasswordAnswerAttemptCount { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
    }
}