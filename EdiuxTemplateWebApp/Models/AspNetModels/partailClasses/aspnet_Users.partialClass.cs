using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_Users : IUser<Guid>, ICloneable
    {
        private Iaspnet_UsersRepository userRepository;
        public Iaspnet_UsersRepository UserRepository
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

            Update();
        }

        public IList<string> GetRoles()
        {
            return aspnet_Roles.Select(w => w.Name).ToList() as IList<string>;
        }

        public bool IsInRole(string roleName)
        {
            return UserRepository.IsInRole(aspnet_Applications.ApplicationName, UserName, roleName);
            //return aspnet_Roles.Any(r => r.Name == roleName || r.LoweredRoleName == roleName);
        }

        public void Update()
        {
            UserRepository.UnitOfWork.Context.Entry(this).State = System.Data.Entity.EntityState.Modified;            
            UserRepository.UnitOfWork.Commit();         
        }

        public void AddToRole(string roleName)
        {
            UserRepository.AddToRole(aspnet_Applications.ApplicationName, UserName, roleName);
            aspnet_Roles = UserRepository.Get(Id).aspnet_Roles;
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

        public object Clone()
        {
            Type sourceType = this.GetType();

            var copy = Activator.CreateInstance(sourceType);

            Type tatgetType = copy.GetType();

            var props_src = sourceType.GetProperties();

            foreach (var srcProp in props_src)
            {
                if (srcProp.PropertyType.IsGenericType)
                {
                    if (srcProp.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                    {
                        var targetProp = tatgetType.GetProperty(srcProp.Name);
                        targetProp.SetValue(copy, Activator.CreateInstance(typeof(Collection<>).MakeGenericType(srcProp.PropertyType.GenericTypeArguments)));
                    }
                }
                else
                {
                    var targetProp = tatgetType.GetProperty(srcProp.Name);
                    targetProp.SetValue(copy, srcProp.GetValue(this));
                }
            }

            foreach (var clone_item in aspnet_PersonalizationPerUser)
            {
                try
                {
                    aspnet_PersonalizationPerUser cloneObject = clone_item.Clone() as aspnet_PersonalizationPerUser;
                    ((aspnet_Users)copy).aspnet_PersonalizationPerUser.Add(cloneObject);

                }
                catch (Exception ex)
                {
                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                    continue;
                }
            }

            foreach (var clone_item in aspnet_Roles)
            {
                try
                {
                    aspnet_Roles cloneObject = clone_item.Clone() as aspnet_Roles;
                    ((aspnet_Users)copy).aspnet_Roles.Add(cloneObject);
                }
                catch (Exception ex)
                {
                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                    continue;
                }
            }

            return copy;
        }
    }
}