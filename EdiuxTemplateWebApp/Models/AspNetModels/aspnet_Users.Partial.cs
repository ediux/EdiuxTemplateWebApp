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
        private static Iaspnet_UsersRepository userRepository;
        public static Iaspnet_UsersRepository UserRepository
        {
            get
            {
                if (userRepository == null)
                    userRepository = RepositoryHelper.Getaspnet_UsersRepository();
                return userRepository;
            }
            set
            {
                userRepository = value;
            }
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

        public void Update()
        {
            UserRepository.UnitOfWork.Context.Entry(this).State = System.Data.Entity.EntityState.Modified;
            UserRepository.UnitOfWork.Context.Entry(aspnet_Membership).State = System.Data.Entity.EntityState.Modified;
            UserRepository.UnitOfWork.Context.Entry(aspnet_PersonalizationPerUser).State = System.Data.Entity.EntityState.Modified;
            UserRepository.UnitOfWork.Context.Entry(aspnet_Profile).State = System.Data.Entity.EntityState.Modified;
            UserRepository.UnitOfWork.Context.Entry(aspnet_Roles).State = System.Data.Entity.EntityState.Modified;
            UserRepository.UnitOfWork.Context.Entry(aspnet_UserLogin).State = System.Data.Entity.EntityState.Modified;
            UserRepository.UnitOfWork.Commit();

            aspnet_Users existedUser = UserRepository.Get(Id);
                

            ApplicationId = existedUser.ApplicationId;
            this.aspnet_Applications = existedUser.aspnet_Applications;
            this.aspnet_Membership = existedUser.aspnet_Membership;
            this.aspnet_PersonalizationPerUser = existedUser.aspnet_PersonalizationPerUser;
            this.aspnet_Profile = existedUser.aspnet_Profile;
            this.aspnet_Roles = existedUser.aspnet_Roles;
            this.aspnet_UserLogin = existedUser.aspnet_UserLogin;
            this.IsAnonymous = existedUser.IsAnonymous;
            this.LastActivityDate = existedUser.LastActivityDate;
            this.LoweredUserName = existedUser.LoweredUserName;
            this.MobileAlias = existedUser.MobileAlias;
            this.UserName = existedUser.UserName;

        }

        public void AddToRole(string roleName)
        {
            if (UserRepository == null)
                UserRepository = RepositoryHelper.Getaspnet_UsersRepository();

            aspnet_Roles role = aspnet_Applications.GetRoleByName(roleName);
            aspnet_Roles.Add(role);
          
            Update();
        }

        public void RemoveFromRole(string roleName)
        {
            string loweredRoleName = roleName.ToLowerInvariant();
            aspnet_Roles foundRole = aspnet_Roles.SingleOrDefault(w => w.LoweredRoleName == loweredRoleName || w.Name == roleName);
            aspnet_Roles.Remove(foundRole);
            UserRepository.UnitOfWork.Context.Entry(aspnet_Roles).State = System.Data.Entity.EntityState.Modified;
            Update();
        }

        public void AddLogin(UserLoginInfo login)
        {
            aspnet_UserLogin.Add(new aspnet_UserLogin() { aspnet_Users = this, LoginProvider = login.LoginProvider, ProviderKey = login.ProviderKey, UserId = Id });
            Update();
        }

        public void RemoveLogin(UserLoginInfo login)
        {
            aspnet_UserLogin dblogin = this.FindLogin(login);
            aspnet_UserLogin.Remove(dblogin);
            Update();
        }

        public IList<UserLoginInfo> GetLogins()
        {
            return aspnet_UserLogin.ToList().ConvertAll(s => new UserLoginInfo(s.LoginProvider, s.ProviderKey));
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
