namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using EdiuxTemplateWebApp.Models.Identity;
    using Microsoft.AspNet.Identity;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Security;
    using aspnet_PersonalizationPerUserHelper = AspNetModels.aspnet_PersonalizationPerUser;
    [MetadataType(typeof(aspnet_UsersMetaData))]
    public partial class aspnet_Users : IUser<Guid>
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<aspnet_Users, Guid> manager)
        {
            // 注意 authenticationType 必須符合 CookieAuthenticationOptions.AuthenticationType 中定義的項目
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在這裡新增自訂使用者宣告

            return userIdentity;
        }

        /// <summary>
        /// 初始化新使用者的預設帳號資料
        /// </summary>
        /// <param name="applicationInfo">要新增的應用程式物件</param>
        /// <param name="userName">帳號名稱</param>
        /// <param name="password">帳號密碼</param>
        /// <param name="passwordAnswer">安全性問題(選填)</param>
        /// <param name="passwordQuestion">安全性問題的答案</param>
        /// <param name="isAnonymous">指出是否為匿名帳號</param>
        /// <param name="twoFactorEnabled">是否啟用兩步驟驗證</param>
        /// <param name="isLockedOut">指出是否初始鎖定</param>
        /// <param name="passwordSalt">加鹽字串</param>
        /// <param name="mobileAlias">行動裝置別名</param>
        /// <param name="comment">帳號說明</param>
        /// <param name="email">電子郵件地址</param>
        /// <param name="mobilePIN">行動裝置PIN碼</param>
        /// <param name="phoneNumber">行動電話號碼</param>
        /// <param name="passwordFormat">密碼格式</param>
        /// <returns></returns>
        internal static aspnet_Users Create(
            aspnet_Applications applicationInfo,
            Guid pathId,
            string userName,
            string password,
            string passwordAnswer = "",
            string passwordQuestion = "",
            bool isAnonymous = true,
            bool twoFactorEnabled = false,
            bool isLockedOut = false,
            string passwordSalt = "EdiuxTemplateWebSite",
            string mobileAlias = "",
            string comment = "",
            string email = "system@localhost.local",
            string mobilePIN = "",
            string phoneNumber = "0910123456",
            MembershipPasswordFormat passwordFormat = MembershipPasswordFormat.Hashed
            )
        {
            UserProfileViewModel defaultProfile = new UserProfileViewModel();
            Guid userId = Guid.NewGuid();
            DateTime currentTime = DateTime.UtcNow;
            DateTime nullTime = new DateTime(1754, 1, 1);

            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("user.UserName 不能為空或Null.");
            }

            aspnet_Users newUser = new aspnet_Users()
            {
                Id = userId,
                ApplicationId = applicationInfo.ApplicationId,
                UserName = userName,
                LoweredUserName = userName.ToLowerInvariant(),
                IsAnonymous = isAnonymous,
                LastActivityDate = currentTime,
                MobileAlias = mobileAlias,
                aspnet_Membership = aspnet_Membership.Create(applicationInfo, userId, currentTime, password,
                passwordAnswer, passwordQuestion, isLockedOut, twoFactorEnabled, passwordSalt, mobileAlias, comment, email, mobilePIN, phoneNumber, passwordFormat),
                aspnet_Profile = aspnet_Profile.Create(),
                aspnet_PersonalizationPerUser = aspnet_PersonalizationPerUserHelper.CreateCollection(null, null),
                aspnet_Applications = applicationInfo,
                aspnet_Roles = new Collection<aspnet_Roles>(),
                aspnet_UserClaims = new Collection<aspnet_UserClaims>(),
                aspnet_UserLogin = new Collection<aspnet_UserLogin>()
            };

            return newUser;
        }

    }

    public partial class aspnet_UsersMetaData
    {
        [Required]
        public System.Guid ApplicationId { get; set; }
        [Required]
        public System.Guid Id { get; set; }

        [StringLength(256, ErrorMessage = "欄位長度不得大於 256 個字元")]
        [Required]
        public string UserName { get; set; }

        [StringLength(256, ErrorMessage = "欄位長度不得大於 256 個字元")]
        [Required]
        public string LoweredUserName { get; set; }

        [StringLength(16, ErrorMessage = "欄位長度不得大於 16 個字元")]
        public string MobileAlias { get; set; }
        [Required]
        public bool IsAnonymous { get; set; }
        [Required]
        public System.DateTime LastActivityDate { get; set; }
        public virtual aspnet_Applications aspnet_Applications { get; set; }
        public virtual ICollection<aspnet_PersonalizationPerUser> aspnet_PersonalizationPerUser { get; set; }
        public virtual aspnet_Profile aspnet_Profile { get; set; }
        public virtual ICollection<aspnet_Roles> aspnet_Roles { get; set; }
        public virtual ICollection<aspnet_UserLogin> aspnet_UserLogin { get; set; }
        public virtual ICollection<aspnet_UserClaims> aspnet_UserClaims { get; set; }
        public virtual aspnet_Membership aspnet_Membership { get; set; }
    }
}
