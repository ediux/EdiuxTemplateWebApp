namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;

    using System.ComponentModel.DataAnnotations;
    using System.Collections.ObjectModel;
    using System.Web.Security;

    [MetadataType(typeof(aspnet_UserClaimsMetaData))]
    public partial class aspnet_UserClaims
    {
        /// <summary>
        /// 根據參數建立資料執行個體
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claimtype"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static aspnet_UserClaims Create(aspnet_Users user, string claimtype, object value)
        {
            return new aspnet_UserClaims()
            {
                aspnet_Users = user,
                UserId = user.Id,
                ClaimType = claimtype,
                ClaimValue = value.ToString()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        internal static ICollection<aspnet_UserClaims> CreateDefaultCollection(aspnet_Users user)
        {
            return new Collection<aspnet_UserClaims>(
                new aspnet_UserClaims[] {
                    Create(user,ClaimTypes.Authentication,(user != null) ? bool.TrueString : bool.FalseString),
                    Create(user,ClaimTypes.Email, user.aspnet_Membership.Email),
                    Create(user,ClaimTypes.Name, user.UserName),
                    Create(user,ClaimTypes.NameIdentifier, user.Id.ToString()),
                    Create(user,"http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", Membership.Provider.Name) });
        }
    }

    public partial class aspnet_UserClaimsMetaData
    {
        [Required]
        public System.Guid UserId { get; set; }
        [Required]
        public int Id { get; set; }

        [StringLength(256, ErrorMessage = "欄位長度不得大於 256 個字元")]
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public virtual aspnet_Users aspnet_Users { get; set; }
    }
}
