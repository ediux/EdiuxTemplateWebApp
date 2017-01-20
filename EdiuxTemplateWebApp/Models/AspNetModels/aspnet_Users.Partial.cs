namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using Microsoft.AspNet.Identity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    [MetadataType(typeof(aspnet_UsersMetaData))]
    public partial class aspnet_Users : IUser<Guid>
    {
        public Iaspnet_UsersRepository UserRepository
        {
            get; set;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<aspnet_Users, System.Guid> manager)
        {
            // 注意 authenticationType 必須符合 CookieAuthenticationOptions.AuthenticationType 中定義的項目
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在這裡新增自訂使用者宣告

            return userIdentity;
        }

        public string GetEmail()
        {
            try
            {
                return aspnet_Membership.Email;
            }
            catch (Exception)
            {                                
                throw;
            }            
        }
        public bool GetEmailConfirmed()
        {
            return false;
        }

        public void SetEmail(string email)
        {
            aspnet_Membership.Email = email;

            if (UserRepository == null)
                UserRepository = RepositoryHelper.Getaspnet_UsersRepository();

            UserRepository.UnitOfWork.Context.Entry(aspnet_Membership).State = System.Data.Entity.EntityState.Modified;
            UserRepository.UnitOfWork.Commit();
        }
  
        public IList<string> GetRoles()
        {
            return aspnet_Roles.Select(w => w.Name).ToList() as IList<string>;
        }

        public bool IsInRole(string roleName)
        {
            return aspnet_Roles.Any(r => r.Name == roleName || r.LoweredRoleName == roleName);
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
        public virtual aspnet_Membership aspnet_Membership { get; set; }
        public virtual ICollection<aspnet_PersonalizationPerUser> aspnet_PersonalizationPerUser { get; set; }
        public virtual aspnet_Profile aspnet_Profile { get; set; }
        public virtual ICollection<aspnet_Roles> aspnet_Roles { get; set; }
    }
}
