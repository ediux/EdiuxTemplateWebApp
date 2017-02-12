using System;
using System.Web.Security;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public class aspnet_Membership_GetAllUsersResult
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public string PasswordQuestion { get; set; }
        public string Comment { get; set; }

        public bool IsApproved { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime LastLoginDate { get; set; }

        public DateTime? LastActivityDate { get; set; }

        public DateTime LastPasswordChangedDate { get; set; }

        public bool IsLockedOut { get; set; }

        public DateTime LastLockoutDate { get; set; }

        public static MembershipUser ConvertToMembershipUser(aspnet_Membership_GetAllUsersResult source)
        {
            return new MembershipUser("Microsoft ASP.NET Identify Framework 3.5",
                source.UserName,
                source.UserId,
                source.Email,
                source.PasswordQuestion,
                source.Comment,
                source.IsApproved,
                source.IsLockedOut,
                source.CreateDate,
                source.LastLoginDate,
                source.LastActivityDate.HasValue ? source.LastActivityDate.Value : default(DateTime),
                source.LastPasswordChangedDate,
                source.LastLockoutDate
                );
        }

        public static aspnet_Membership ConverToaspnet_Membership(aspnet_Membership_GetAllUsersResult source)
        {
            return new aspnet_Membership()
            {
                Comment = source.Comment,
                CreateDate = source.CreateDate,
                Email = source.Email,
                IsApproved = source.IsApproved,
                PasswordQuestion = source.PasswordQuestion,
                IsLockedOut = source.IsLockedOut,
                LastActivityTime = source.LastActivityDate,
                LastLoginDate = source.LastLoginDate,
                LastLockoutDate = source.LastLockoutDate,
                LastPasswordChangedDate = source.LastPasswordChangedDate

            };
        }
    }
}