using EdiuxTemplateWebApp.Filters;
using EdiuxTemplateWebApp.Helpers;
using EdiuxTemplateWebApp.Models.AspNetModels;
using EdiuxTemplateWebApp.Models.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq.Expressions;

namespace EdiuxTemplateWebApp.Models
{
    public class EdiuxAspNetSqlUserStore : IEdiuxAspNetSqlUserStore
    {
        #region 變數宣告區
        private static string AppName;

        private aspnet_Applications ApplicationInfo
        {
            get
            {
                IApplicationStore<aspnet_Applications, Guid> store = this;

                if (string.IsNullOrEmpty(AppName))
                {
                    AppName = store.GetApplicationNameFromConfiguratinFile();
                }

                return store.GetEntityByQuery(AppName);
            }
        }

        private IOwinContext pcontext;

        public static IEdiuxAspNetSqlUserStore Create(IdentityFactoryOptions<IEdiuxAspNetSqlUserStore> options, IOwinContext context)
        {
            return new EdiuxAspNetSqlUserStore(context);
        }

        public EdiuxAspNetSqlUserStore(IOwinContext context)
        {
            pcontext = context;

            IApplicationStore<aspnet_Applications, Guid> store = this;

            var asynctask = store.InitializationAsync();

            if (asynctask.Status != TaskStatus.RanToCompletion)
            {
                asynctask.Wait();
            }
        }

        #endregion

        #region Queryable User Store
        public IQueryable<aspnet_Users> Users
        {
            get
            {
                return ApplicationInfo.aspnet_Users.AsQueryable();
            }
        }
        #endregion

        #region Queryable Role Store
        public IQueryable<aspnet_Roles> Roles
        {
            get
            {
                return ApplicationInfo.aspnet_Roles.AsQueryable();
            }
        }

        public IQueryable<aspnet_Applications> Applications
        {
            get
            {
                Iaspnet_ApplicationsRepository appRepo = pcontext.Get<Iaspnet_ApplicationsRepository>();
                return appRepo.All();
            }
        }


        #endregion

        #region User Store(使用者帳號的CRUD)
        public Task CreateAsync(aspnet_Users user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (user.ApplicationId == null || user.ApplicationId == Guid.Empty)
            {
                user.ApplicationId = ApplicationInfo.ApplicationId;
                user.aspnet_Applications = ApplicationInfo;
            }

            Iaspnet_UsersRepository userRepo = pcontext.Get<Iaspnet_UsersRepository>();

            var users = userRepo.GetUserByName(ApplicationInfo.ApplicationName,
                                               user.UserName,
                                               DateTime.UtcNow,
                                               true);

            aspnet_Users newUser = new aspnet_Users();

            if (users == null)
            {
                newUser = aspnet_Users.Create(ApplicationInfo, Guid.Empty, user.UserName, user.aspnet_Membership.Password,
                    isAnonymous: false, passwordSalt: user.aspnet_Membership.PasswordSalt, email: user.aspnet_Membership.Email,
                     mobilePIN: user.aspnet_Membership.MobilePIN, mobileAlias: user.MobileAlias);

                newUser = userRepo.CopyTo<aspnet_Users>(user);
            }
            else
            {
                newUser = userRepo.GetUserByName(user.aspnet_Applications.ApplicationName, user.UserName, DateTime.UtcNow, true);
                newUser = userRepo.CopyTo<aspnet_Users>(user);
                user = userRepo.Update(newUser);
                return Task.CompletedTask;
            }

            if (user.aspnet_Membership != null)
            {
                newUser.aspnet_Membership.UserId = newUser.Id;
            }
            else
            {
                newUser.aspnet_Membership = new aspnet_Membership()
                {
                    AccessFailedCount = 0,
                    ApplicationId = ApplicationInfo.ApplicationId,
                    aspnet_Applications = ApplicationInfo,
                    Comment = "",
                    CreateDate = DateTime.Now.Date,
                    Email = newUser.LoweredUserName + "@localhost.local",
                    EmailConfirmed = true,
                    FailedPasswordAnswerAttemptCount = 0,
                    FailedPasswordAnswerAttemptWindowStart = new DateTime(1754, 1, 1),
                    FailedPasswordAttemptCount = 0,
                    FailedPasswordAttemptWindowStart = new DateTime(1754, 1, 1),
                    IsApproved = true,
                    IsLockedOut = false,
                    LastLockoutDate = new DateTime(1754, 1, 1),
                    LastLoginDate = new DateTime(1754, 1, 1),
                    LastPasswordChangedDate = new DateTime(1754, 1, 1)
                };
                newUser.aspnet_Membership.LoweredEmail = newUser.aspnet_Membership.Email.ToLowerInvariant();
                newUser.aspnet_Membership.MobilePIN = "123456";
                newUser.aspnet_Membership.Password = Membership.GeneratePassword(Membership.MinRequiredPasswordLength, Membership.MinRequiredNonAlphanumericCharacters);
                newUser.aspnet_Membership.PasswordAnswer = "(none)";
                newUser.aspnet_Membership.PasswordFormat = (int)MembershipPasswordFormat.Hashed;
                newUser.aspnet_Membership.PasswordQuestion = "(none)";
                newUser.aspnet_Membership.PasswordSalt = Path.GetRandomFileName();
                newUser.aspnet_Membership.PhoneConfirmed = true;
                newUser.aspnet_Membership.PhoneNumber = "0901123456";
                newUser.aspnet_Membership.ResetPasswordToken = "";
                newUser.aspnet_Membership.UserId = newUser.Id;

                PasswordHasher pwdHasher = new PasswordHasher();

                newUser.aspnet_Membership.Password =
                    pwdHasher.HashPassword(newUser.aspnet_Membership.Password +
                    newUser.aspnet_Membership.PasswordSalt);
            }

            if (newUser.aspnet_Membership == null)
            {

            }

            if (user.aspnet_Profile == null)
            {
                newUser.aspnet_Profile = new aspnet_Profile()
                {
                    LastUpdatedDate = DateTime.UtcNow
                };
                UserProfileViewModel newProfile = new UserProfileViewModel()
                {
                    AvatarFilePath = "/Content/images/user.jpg",
                    CompanyName = "Ediux Workshop",
                    PositionTitle = "PG",
                    CompanyWebSiteURL = "http://www.riaxe.com/"
                };
                newUser.aspnet_Profile.PropertyValuesBinary = newProfile.Serialize();
                newUser.aspnet_Profile.PropertyValuesString = "{}";
                newUser.aspnet_Profile.PropertyNames = string.Join(",", newProfile.GetProperties().Keys.ToArray());
                newUser.aspnet_Profile.LastUpdatedDate = DateTime.UtcNow;

                newUser.aspnet_Profile.UserId = newUser.Id;
            }

            if (!user.aspnet_Roles.Any())
            {
                AddToRoleAsync(newUser, "Users");
            }

            if (!user.aspnet_UserClaims.Any())
            {
                List<aspnet_UserClaims> claims = new List<aspnet_UserClaims>
                {
                    new aspnet_UserClaims()
                    {
                        UserId = newUser.Id,
                        ClaimType = ClaimTypes.Email,
                        ClaimValue = newUser.aspnet_Membership.LoweredEmail,
                        Id = 0
                    },
                    new aspnet_UserClaims()
                    {
                        UserId = newUser.Id,
                        ClaimType = ClaimTypes.Name,
                        ClaimValue = newUser.UserName,
                        Id = 1
                    },
                    new aspnet_UserClaims()
                    {
                        UserId = newUser.Id,
                        ClaimType = ClaimTypes.NameIdentifier,
                        ClaimValue = user.Id.ToString(),
                        Id = 2
                    },
                    new aspnet_UserClaims()
                    {
                        UserId = newUser.Id,
                        ClaimType = ClaimTypes.Email,
                        ClaimValue = "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                        Id = 3
                    }
                };
            }
            else
            {

                foreach (var c in user.aspnet_UserClaims)
                {
                    c.UserId = newUser.Id;
                    newUser.aspnet_UserClaims.Add(c);
                }

            }

            user = userRepo.Add(newUser);
            userRepo.UnitOfWork.Commit();

            return Task.CompletedTask;
        }

        public Task DeleteAsync(aspnet_Users user)
        {
            try
            {
                Iaspnet_UsersRepository userRepo = pcontext.Get<Iaspnet_UsersRepository>();
                userRepo.Delete(user);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
        }

        public Task<aspnet_Users> FindByIdAsync(Guid userId)
        {
            try
            {
                Iaspnet_UsersRepository userRepo = pcontext.Get<Iaspnet_UsersRepository>();
                var _user = userRepo.Where(w => w.Id == userId &&
                                           w.ApplicationId == ApplicationInfo.ApplicationId)
                                    .SingleAsync();

                return _user;

            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
        }

        public Task<aspnet_Users> FindByNameAsync(string userName)
        {
            try
            {
                Iaspnet_UsersRepository userRepo = pcontext.Get<Iaspnet_UsersRepository>();
                var _user =
                    userRepo.Where(w => (w.UserName == userName
                                         || w.LoweredUserName == userName)
                                   && w.ApplicationId == ApplicationInfo
                                   .ApplicationId)
                            .SingleOrDefaultAsync();
                return _user;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
        }

        public Task UpdateAsync(aspnet_Users user)
        {
            try
            {
                Iaspnet_UsersRepository userRepo = pcontext.Get<Iaspnet_UsersRepository>();
                Iaspnet_MembershipRepository membershipRepo = pcontext.Get<Iaspnet_MembershipRepository>();

                membershipRepo.UnitOfWork = userRepo.UnitOfWork;

                aspnet_Users _existedUser = userRepo
                    .Where(x => x.ApplicationId == ApplicationInfo.ApplicationId
                           && (x.UserName == user.UserName
                               || x.LoweredUserName == user.LoweredUserName))
                    .SingleOrDefault();

                _existedUser = userRepo.CopyTo<aspnet_Users>(user);
                _existedUser = userRepo.Update(_existedUser);

                _existedUser.aspnet_Membership =
                                membershipRepo.CopyTo<aspnet_Membership>(
                                    user.aspnet_Membership);

                membershipRepo.Update(_existedUser.aspnet_Membership);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
        }

        public Task<aspnet_Users> GetUserByIdAsync(Guid userId)
        {
            return Task.FromResult((from app in Applications
                                    from user in app.aspnet_Users
                                    where user.Id == userId
                                    select user).ToList().SingleOrDefault());
        }
        #endregion

        #region User Role Store
        public Task AddToRoleAsync(aspnet_Users user, string roleName)
        {
            try
            {
                Iaspnet_UsersRepository userRepo = pcontext.Get<Iaspnet_UsersRepository>();
                userRepo.AddToRole(user, roleName);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }
        }

        public Task<IList<string>> GetRolesAsync(aspnet_Users user)
        {
            try
            {
                if (user.ApplicationId == null || user.ApplicationId == Guid.Empty)
                {
                    user.ApplicationId = ApplicationInfo.ApplicationId;
                    user.aspnet_Applications = ApplicationInfo;
                }
                Iaspnet_UsersRepository userRepo = pcontext.Get<Iaspnet_UsersRepository>();

                var foundUser = userRepo
                    .Where(w => w.Id == user.Id
                           || (w.UserName == user.UserName ||
                                 w.LoweredUserName == user.LoweredUserName)
                           && w.ApplicationId == user.ApplicationId)
                    .SingleOrDefault();

                if (foundUser != null)
                {
                    var rolesforUser = foundUser.aspnet_Roles.Select(s => s.Name).ToList();

                    return Task.FromResult(rolesforUser as IList<string>);
                }

                return Task.FromResult<IList<string>>(new List<string>());
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<IList<string>>(ex);
            }
        }

        public Task<bool> IsInRoleAsync(aspnet_Users user, string roleName)
        {
            try
            {
                user.ApplicationId = ApplicationInfo.ApplicationId;
                user.aspnet_Applications = ApplicationInfo;

                Iaspnet_UsersRepository userRepo = pcontext.Get<Iaspnet_UsersRepository>();
                var isinrole = userRepo.IsInRole(user, roleName);
                return Task.FromResult(isinrole);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<bool>(ex);
            }
        }

        public Task RemoveFromRoleAsync(aspnet_Users user, string roleName)
        {
            try
            {
                Iaspnet_UsersRepository userRepo = pcontext.Get<Iaspnet_UsersRepository>();
                userRepo.RemoveFromRole(user, roleName);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }
        }
        #endregion

        #region Role Store
        public Task CreateAsync(aspnet_Roles role)
        {
            try
            {
                Iaspnet_RolesRepository roleRepo = pcontext.Get<Iaspnet_RolesRepository>();
                roleRepo.Add(role);
                return roleRepo.UnitOfWork.CommitAsync();

            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }
        }

        public Task UpdateAsync(aspnet_Roles role)
        {
            try
            {
                Iaspnet_RolesRepository roleRepo = pcontext.Get<Iaspnet_RolesRepository>();
                roleRepo.Update(role);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }
        }

        public Task DeleteAsync(aspnet_Roles role)
        {
            try
            {
                Iaspnet_RolesRepository roleRepo = pcontext.Get<Iaspnet_RolesRepository>();
                roleRepo.Delete(role);

                return roleRepo.UnitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }
        }

        Task<aspnet_Roles> IRoleStore<aspnet_Roles, Guid>.FindByIdAsync(Guid roleId)
        {
            try
            {
                Iaspnet_RolesRepository roleRepo = pcontext.Get<Iaspnet_RolesRepository>();
                return Task.FromResult(roleRepo.Get(roleId));
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<aspnet_Roles>(ex);
            }
        }

        Task<aspnet_Roles> IRoleStore<aspnet_Roles, Guid>.FindByNameAsync(string roleName)
        {
            try
            {
                Iaspnet_RolesRepository roleRepo = pcontext.Get<Iaspnet_RolesRepository>();
                return Task.FromResult(roleRepo.FindByName(
                    ApplicationInfo.ApplicationId, roleName)
                                       .SingleOrDefault());
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<aspnet_Roles>(ex);
            }
        }
        #endregion

        #region Email Store
        public Task SetEmailAsync(aspnet_Users user, string email)
        {
            try
            {
                Iaspnet_MembershipRepository membershipRepo = pcontext.Get<Iaspnet_MembershipRepository>();

                if (user.aspnet_Membership == null)
                {
                    throw new Exception(string.Format("The user '{0}' has missed membership information! ", user.UserName));
                }

                var membership = membershipRepo.Get(user.aspnet_Membership.UserId);

                membership.Email = email;
                membership.LoweredEmail = email.ToLowerInvariant();

                membershipRepo.Update(membership);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }
        }

        public Task<string> GetEmailAsync(aspnet_Users user)
        {
            try
            {
                if (user.aspnet_Membership == null)
                {
                    throw new Exception(string.Format("The user '{0}' has missed membership information! ", user.UserName));
                }
                Iaspnet_MembershipRepository membershipRepo = pcontext.Get<Iaspnet_MembershipRepository>();
                var membership = membershipRepo.Get(user.aspnet_Membership.UserId);

                if (membership != null)
                {
                    return Task.FromResult(membership.Email);
                }
                return Task.FromResult(string.Empty);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<string>(ex);
            }
        }

        public Task<bool> GetEmailConfirmedAsync(aspnet_Users user)
        {
            try
            {
                if (user.aspnet_Membership == null)
                {
                    throw new Exception(string.Format("The user '{0}' has missed membership information! ", user.UserName));
                }
                Iaspnet_MembershipRepository membershipRepo = pcontext.Get<Iaspnet_MembershipRepository>();
                var membership = membershipRepo.Get(user.aspnet_Membership.UserId);
                if (membership != null)
                {
                    return Task.FromResult(membership.EmailConfirmed);
                }
                return Task.FromException<bool>(new Exception("User not found."));
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<bool>(ex);
            }
        }

        public Task SetEmailConfirmedAsync(aspnet_Users user, bool confirmed)
        {
            try
            {
                if (user.aspnet_Membership == null)
                {
                    throw new Exception(string.Format("The user '{0}' has missed membership information! ", user.UserName));
                }

                Iaspnet_UsersRepository userRepo = pcontext.Get<Iaspnet_UsersRepository>();
                Iaspnet_MembershipRepository membershipRepo = pcontext.Get<Iaspnet_MembershipRepository>();
                membershipRepo.UnitOfWork = userRepo.UnitOfWork;

                var membership = membershipRepo.Get(user.aspnet_Membership.UserId);
                if (membership != null)
                {
                    membership.EmailConfirmed = confirmed;
                    membershipRepo.Update(membership);
                    user = userRepo.Reload(user);
                }
                return Task.FromException(new Exception("User not found."));
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }
        }

        public Task<aspnet_Users> FindByEmailAsync(string email)
        {
            try
            {
                Iaspnet_UsersRepository userRepo = pcontext.Get<Iaspnet_UsersRepository>();

                return Task.FromResult(
                    userRepo.GetUserByEmail(
                        ApplicationInfo.ApplicationName,
                        email,
                        DateTime.UtcNow,
                    false));

            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<aspnet_Users>(ex);
            }
        }

        #endregion

        #region User Lockout Store
        public Task<DateTimeOffset> GetLockoutEndDateAsync(aspnet_Users user)
        {
            try
            {
                if (user.aspnet_Membership == null)
                {
                    throw new Exception(string.Format("The user '{0}' has missed membership information! ", user.UserName));
                }
                Iaspnet_MembershipRepository membershipRepo = pcontext.Get<Iaspnet_MembershipRepository>();
                var founduser = membershipRepo.Get(user.aspnet_Membership.UserId);
                return Task.FromResult(
                    new DateTimeOffset(
                        founduser.LockoutEndDate ?? new DateTime(1754, 1, 1)));
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<DateTimeOffset>(ex);
            }
        }

        public Task SetLockoutEndDateAsync(aspnet_Users user, DateTimeOffset lockoutEnd)
        {
            try
            {
                if (user.aspnet_Membership == null)
                {
                    throw new Exception(string.Format("The user '{0}' has missed membership information! ", user.UserName));
                }
                Iaspnet_MembershipRepository membershipRepo = pcontext.Get<Iaspnet_MembershipRepository>();
                var membership = membershipRepo.Get(user.aspnet_Membership.UserId);

                if (membership != null)
                {
                    membership.LockoutEndDate = new DateTime(lockoutEnd.Ticks);
                    membershipRepo.Update(membership);
                    user.aspnet_Membership = membershipRepo.Reload(membership);
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }
        }

        public Task<int> IncrementAccessFailedCountAsync(aspnet_Users user)
        {
            try
            {
                if (user.aspnet_Membership == null)
                {
                    throw new Exception(string.Format("The user '{0}' has missed membership information! ", user.UserName));
                }
                Iaspnet_MembershipRepository membershipRepo = pcontext.Get<Iaspnet_MembershipRepository>();
                var membership = membershipRepo.Get(user.aspnet_Membership.UserId);

                membership.AccessFailedCount += 1;

                membershipRepo.Update(membership);
                user.aspnet_Membership = membershipRepo.Reload(membership);

                return Task.FromResult(membership.FailedPasswordAttemptCount);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<int>(ex);
            }

        }

        public Task ResetAccessFailedCountAsync(aspnet_Users user)
        {
            try
            {
                if (user.aspnet_Membership == null)
                {
                    throw new Exception(string.Format("The user '{0}' has missed membership information! ", user.UserName));
                }

                Iaspnet_UsersRepository userRepo = pcontext.Get<Iaspnet_UsersRepository>();

                var founduser = userRepo.Get(user.Id);
                var membership = founduser?.aspnet_Membership;

                if (membership != null)
                {
                    membership.IsLockedOut = true;
                    membership.LockoutEndDate = DateTime.UtcNow;
                    membership.AccessFailedCount = 0;

                    userRepo.Update(founduser);
                    user = userRepo.Reload(founduser);
                }


                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }
        }

        public Task<int> GetAccessFailedCountAsync(aspnet_Users user)
        {
            try
            {
                if (user.aspnet_Membership == null)
                {
                    throw new Exception(string.Format("The user '{0}' has missed membership information! ", user.UserName));
                }
                Iaspnet_MembershipRepository membershipRepo = pcontext.Get<Iaspnet_MembershipRepository>();
                var membership = membershipRepo.Get(user.aspnet_Membership.UserId);

                return Task.FromResult(membership.AccessFailedCount);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<int>(ex);
            }
        }

        public Task<bool> GetLockoutEnabledAsync(aspnet_Users user)
        {
            try
            {
                if (user.aspnet_Membership == null)
                {
                    throw new Exception(string.Format("The user '{0}' has missed membership information! ", user.UserName));
                }
                Iaspnet_MembershipRepository membershipRepo = pcontext.Get<Iaspnet_MembershipRepository>();
                var membership = membershipRepo.Get(user.aspnet_Membership.UserId);

                return Task.FromResult(membership.IsLockedOut);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<bool>(ex);
            }
        }

        public Task SetLockoutEnabledAsync(aspnet_Users user, bool enabled)
        {
            try
            {
                if (user.aspnet_Membership == null)
                {
                    throw new Exception(string.Format("The user '{0}' has missed membership information! ", user.UserName));
                }
                Iaspnet_MembershipRepository membershipRepo = pcontext.Get<Iaspnet_MembershipRepository>();
                var membership = membershipRepo.Get(user.aspnet_Membership.UserId);

                membership.IsLockedOut = enabled;

                if (enabled)
                {
                    membership.LastLockoutDate = DateTime.Now;
                }
                else
                {
                    membership.LockoutEndDate = DateTime.Now;
                }

                membershipRepo.Update(membership);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }
        }
        #endregion

        #region User Login Store
        public Task AddLoginAsync(aspnet_Users user, UserLoginInfo login)
        {
            try
            {
                Iaspnet_UsersRepository userRepo = pcontext.Get<Iaspnet_UsersRepository>();
                Iaspnet_UserLoginRepository userloginRepo = pcontext.Get<Iaspnet_UserLoginRepository>();
                userloginRepo.UnitOfWork = userRepo.UnitOfWork;

                userloginRepo.Add(new aspnet_UserLogin() { LoginProvider = login.LoginProvider, ProviderKey = login.ProviderKey, UserId = user.Id });
                userloginRepo.UnitOfWork.Commit();
                user = userRepo.Reload(user);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }
        }

        public Task RemoveLoginAsync(aspnet_Users user, UserLoginInfo login)
        {
            try
            {
                Iaspnet_UserLoginRepository userloginRepo = pcontext.Get<Iaspnet_UserLoginRepository>();
                var found = userloginRepo.Get(login.LoginProvider, login.ProviderKey);
                userloginRepo.Delete(found);
                userloginRepo.UnitOfWork.Commit();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(aspnet_Users user)
        {
            try
            {
                Iaspnet_UserLoginRepository userloginRepo = pcontext.Get<Iaspnet_UserLoginRepository>();
                var found = userloginRepo.Where(w => w.UserId == user.Id);
                return Task.FromResult(found.ToList().ConvertAll(c => new UserLoginInfo(c.LoginProvider, c.ProviderKey)) as IList<UserLoginInfo>);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<IList<UserLoginInfo>>(ex);
            }
        }

        public Task<aspnet_Users> FindAsync(UserLoginInfo login)
        {
            try
            {
                Iaspnet_UsersRepository userRepo = pcontext.Get<Iaspnet_UsersRepository>();
                Iaspnet_UserLoginRepository userloginRepo = pcontext.Get<Iaspnet_UserLoginRepository>();
                userloginRepo.UnitOfWork = userRepo.UnitOfWork;

                aspnet_UserLogin _login = userloginRepo.Get(login.LoginProvider, login.ProviderKey);
                if (_login != null)
                {
                    return Task.FromResult(userRepo.Get(_login.UserId));
                }
                return Task.FromResult<aspnet_Users>(null);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<aspnet_Users>(ex);
            }
        }
        #endregion

        #region User Password Store
        public Task SetPasswordHashAsync(aspnet_Users user, string passwordHash)
        {
            try
            {
                Iaspnet_UsersRepository userRepo = pcontext.Get<Iaspnet_UsersRepository>();
                Iaspnet_MembershipRepository membershipRepo = pcontext.Get<Iaspnet_MembershipRepository>();
                membershipRepo.UnitOfWork = userRepo.UnitOfWork;

                if (user.aspnet_Membership == null)
                {
                    throw new Exception(string.Format("The user '{0}' has missed membership information! ", user.UserName));
                }

                var membership = membershipRepo.Get(user.aspnet_Membership.UserId);

                if (membership != null)
                {
                    membership.Password = passwordHash;
                    membership.PasswordFormat = (int)MembershipPasswordFormat.Hashed;
                    membership.LastPasswordChangedDate = DateTime.UtcNow;
                    membership.LastUpdateTime = DateTime.UtcNow;
                    membership.LastUpdateUserId = HttpContext.Current.User.Identity.GetUserId();

                    membershipRepo.Update(membership);
                    membershipRepo.UnitOfWork.Commit();

                    user = userRepo.Get(user.Id);
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }
        }

        public Task<string> GetPasswordHashAsync(aspnet_Users user)
        {
            try
            {

                if (user.aspnet_Membership == null)
                {
                    throw new Exception(string.Format("The user '{0}' has missed membership information! ", user.UserName));
                }
                Iaspnet_MembershipRepository membershipRepo = pcontext.Get<Iaspnet_MembershipRepository>();
                var membership = membershipRepo.Get(user.aspnet_Membership.UserId);

                if (membership.PasswordFormat == (int)MembershipPasswordFormat.Hashed ||
                    membership.PasswordFormat == (int)MembershipPasswordFormat.Encrypted)
                {
                    return Task.FromResult(membership.Password);
                }
                else
                {
                    return Task.FromResult(string.Empty);
                }

            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<string>(ex);
            }
        }

        public Task<bool> HasPasswordAsync(aspnet_Users user)
        {
            try
            {
                if (user.aspnet_Membership == null)
                {
                    throw new Exception(string.Format("The user '{0}' has missed membership information! ", user.UserName));
                }
                Iaspnet_MembershipRepository membershipRepo = pcontext.Get<Iaspnet_MembershipRepository>();
                var membership = membershipRepo.Get(user.aspnet_Membership.UserId);

                bool hasPasswordTask = !string.IsNullOrEmpty(membership.Password);

                return Task.FromResult(hasPasswordTask);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<bool>(ex);
            }
        }
        #endregion

        #region User Phone Number Store
        public Task SetPhoneNumberAsync(aspnet_Users user, string phoneNumber)
        {
            try
            {
                if (user.aspnet_Membership == null)
                {
                    throw new Exception(string.Format("The user '{0}' has missed membership information! ", user.UserName));
                }
                Iaspnet_MembershipRepository membershipRepo = pcontext.Get<Iaspnet_MembershipRepository>();
                var membership = membershipRepo.Get(user.aspnet_Membership.UserId);

                if (phoneNumber != membership.PhoneNumber)
                {


                    membership.PhoneNumber = phoneNumber;
                    if (GetTwoFactorEnabledAsync(user).Result)
                    {
                        membership.PhoneConfirmed = false;
                    }
                    else
                    {
                        membership.PhoneConfirmed = true;
                    }

                    membershipRepo.Update(membership);
                    user.aspnet_Membership = membershipRepo.Reload(membership);
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }
        }

        public Task<string> GetPhoneNumberAsync(aspnet_Users user)
        {
            try
            {
                if (user.aspnet_Membership == null)
                {
                    throw new Exception(string.Format("The user '{0}' has missed membership information! ", user.UserName));
                }
                Iaspnet_MembershipRepository membershipRepo = pcontext.Get<Iaspnet_MembershipRepository>();
                var membership = membershipRepo.Get(user.aspnet_Membership.UserId);

                return Task.FromResult(membership.PhoneNumber);

            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<string>(ex);
            }
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(aspnet_Users user)
        {
            try
            {
                if (user.aspnet_Membership == null)
                {
                    throw new Exception(string.Format("The user '{0}' has missed membership information! ", user.UserName));
                }
                Iaspnet_MembershipRepository membershipRepo = pcontext.Get<Iaspnet_MembershipRepository>();
                var membership = membershipRepo.Get(user.aspnet_Membership.UserId);
                return Task.FromResult(user.aspnet_Membership.PhoneConfirmed);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<bool>(ex);
            }
        }

        public Task SetPhoneNumberConfirmedAsync(aspnet_Users user, bool confirmed)
        {
            try
            {
                if (user.aspnet_Membership == null)
                {
                    throw new Exception(string.Format("The user '{0}' has missed membership information! ", user.UserName));
                }
                Iaspnet_MembershipRepository membershipRepo = pcontext.Get<Iaspnet_MembershipRepository>();
                var membership = membershipRepo.Get(user.aspnet_Membership.UserId);

                membership.PhoneConfirmed = confirmed;
                membershipRepo.Update(membership);
                user.aspnet_Membership = membershipRepo.Reload(membership);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }
        }
        #endregion

        #region User Security Stamp Store
        public Task SetSecurityStampAsync(aspnet_Users user, string stamp)
        {
            try
            {
                var profile = user.GetProfile();
                profile.SecurityStamp = stamp;
                user.SetProfile(profile);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }
        }

        public Task<string> GetSecurityStampAsync(aspnet_Users user)
        {
            try
            {
                var profile = user.GetProfile();
                return Task.FromResult(profile.SecurityStamp);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<string>(ex);
            }
        }
        #endregion

        #region User Two Factor Store
        public Task SetTwoFactorEnabledAsync(aspnet_Users user, bool enabled)
        {
            try
            {
                if (user.aspnet_Profile == null)
                {
                    throw new Exception(string.Format("The user '{0}' has missed membership information! ", user.UserName));
                }

                var userprofile = user.GetProfile();
                userprofile.TwoFactorEnabled = enabled;
                user.SetProfile(userprofile);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }
        }

        public Task<bool> GetTwoFactorEnabledAsync(aspnet_Users user)
        {
            try
            {
                if (user.aspnet_Profile == null)
                {
                    throw new Exception(string.Format("The user '{0}' has missed membership information! ", user.UserName));
                }
                Iaspnet_MembershipRepository membershipRepo = pcontext.Get<Iaspnet_MembershipRepository>();
                var membership = membershipRepo.Get(user.aspnet_Membership.UserId);

                UserProfileViewModel profile = user.GetProfile();

                if (profile == null)
                    return Task.FromException<bool>(new Exception("Profile is not existed."));

                return Task.FromResult(profile.TwoFactorEnabled);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<bool>(ex);
            }
        }
        #endregion

        #region User Claim Store
        public Task<IList<Claim>> GetClaimsAsync(aspnet_Users user)
        {
            try
            {
                return Task.FromResult(new List<Claim>() as IList<Claim>);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<IList<Claim>>(ex);
            }
        }

        public Task AddClaimAsync(aspnet_Users user, Claim claim)
        {
            try
            {
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }
        }

        public Task RemoveClaimAsync(aspnet_Users user, Claim claim)
        {
            try
            {
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }
        }


        private bool CheckCurrentAppIsRegistered
        {
            get
            {
                try
                {
                    IApplicationStore<aspnet_Applications, Guid> appStore = this;
                    var getAppNameTask = appStore.GetApplicationNameFromConfiguratinFileAsync();

                    if (getAppNameTask.Status != TaskStatus.RanToCompletion)
                    {
                        getAppNameTask.Wait();
                    }

                    string appName = getAppNameTask.Result.ToLowerInvariant();

                    return appStore.IsExisted(w => w.LoweredApplicationName == appName);
                }
                catch (Exception ex)
                {
                    WriteErrorLog(ex);
                    throw;
                }

            }
        }

        private void SetToMemoryCache()
        {
            this.SetApplicationGlobalVariable(ApplicationInfoKey, ApplicationInfo);
        }

        protected virtual void WriteErrorLog(Exception ex)
        {
            if (HttpContext.Current == null)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
            else
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }

        private bool GetCheckRootUserHasAdminsRole()
        {
            Iaspnet_ApplicationsRepository appRepo = pcontext.Get<Iaspnet_ApplicationsRepository>();

            if (appRepo == null)
                appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();
            string appName = GetApplicationNameFromConfiguratinFileAsync().Result;
            aspnet_Applications appInfo = appRepo.FindByName(appName).SingleOrDefault();

            Iaspnet_UsersRepository usersRepo = RepositoryHelper.Getaspnet_UsersRepository(appRepo.UnitOfWork);

            aspnet_Users rootUser = usersRepo.GetUserByName(appInfo.ApplicationName, "root", DateTime.UtcNow, false);

            if (rootUser != null)
            {
                return rootUser.aspnet_Roles.Any(s => s.Name.Equals("Admins", StringComparison.InvariantCultureIgnoreCase));
            }
            throw new NullReferenceException(string.Format("The object of '{0}' is not found.", nameof(rootUser)));
        }

        private void AddRootUserToAdminsRole()
        {
            if (ApplicationInfo.aspnet_Roles.Any(s => s.Name.Equals("Admins", StringComparison.InvariantCultureIgnoreCase)) == false)
                throw new NullReferenceException(string.Format("The role of name, '{0}', is not found.", "Admins"));

            Iaspnet_UsersRepository userRepo = pcontext.Get<Iaspnet_UsersRepository>();

            aspnet_Users rootUser = userRepo.GetUserByName(ApplicationInfo.ApplicationName, "root", DateTime.UtcNow, false);

            if (rootUser != null)
            {
                if (userRepo.IsInRole(rootUser, "Admins") == false)
                    userRepo.AddToRole(rootUser, "Admins");
                return;
            }
            throw new NullReferenceException(string.Format("The username , '{0}', is not found.", "root"));
        }

        private bool IsHasRootUser
        {
            get
            {
                if (ApplicationInfo != null)
                {
                    return ApplicationInfo.aspnet_Users.Any(s => s.UserName.Equals("root", StringComparison.InvariantCultureIgnoreCase));
                }

                return false;
            }
        }

        private void CreateRootUser()
        {
            UserProfileViewModel defaultProfile = new UserProfileViewModel();
            Guid userId = Guid.NewGuid();

            aspnet_Users rootUser = new aspnet_Users()
            {
                Id = userId,
                ApplicationId = ApplicationInfo.ApplicationId,
                UserName = "root",
                LoweredUserName = "root",
                IsAnonymous = false,
                LastActivityDate = DateTime.Now,
                MobileAlias = string.Empty,
                aspnet_Membership = new aspnet_Membership()
                {
                    AccessFailedCount = 0,
                    ApplicationId = ApplicationInfo.ApplicationId,
                    aspnet_Applications = ApplicationInfo,
                    Comment = string.Empty,
                    CreateDate = DateTime.UtcNow,
                    Email = "root@localhost.local",
                    EmailConfirmed = true,
                    FailedPasswordAnswerAttemptCount = 0,
                    FailedPasswordAnswerAttemptWindowStart = new DateTime(1754, 1, 1),
                    FailedPasswordAttemptCount = 0,
                    FailedPasswordAttemptWindowStart = new DateTime(1754, 1, 1),
                    IsApproved = true,
                    IsLockedOut = false,
                    LastLockoutDate = new DateTime(1754, 1, 1),
                    LastLoginDate = new DateTime(1754, 1, 1),
                    LastPasswordChangedDate = new DateTime(1754, 1, 1),
                    LoweredEmail = "root@localhost.local",
                    MobilePIN = string.Empty,
                    Password = "!QAZ2wsx",
                    PasswordSalt = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                    PasswordAnswer = string.Empty,
                    PasswordFormat = (int)MembershipPasswordFormat.Hashed,
                    PasswordQuestion = string.Empty,
                    PhoneConfirmed = true,
                    PhoneNumber = "0901123456",
                    ResetPasswordToken = string.Empty,
                    UserId = userId
                },
                aspnet_Profile = new aspnet_Profile()
                {
                    LastUpdatedDate = DateTime.UtcNow,
                    PropertyNames = string.Join(",", defaultProfile.GetProperties().Select(s => s.Key).ToArray()),
                    PropertyValuesString = JsonConvert.SerializeObject(defaultProfile),
                    PropertyValuesBinary = defaultProfile.Serialize(),
                    UserId = userId
                }
            };

            var asynctask = CreateAsync(rootUser);

            if (!asynctask.IsCompleted)
            {
                asynctask.Wait();
            }

            var asynctask_addtoroleuser = AddToRoleAsync(rootUser, "Admins");

            if (!asynctask_addtoroleuser.IsCompleted)
            {
                asynctask_addtoroleuser.Wait();
            }

        }

        private void CreateAnonymousUser()
        {
            aspnet_Users guestUser = new aspnet_Users()
            {
                Id = Guid.NewGuid(),
                ApplicationId = ApplicationInfo.ApplicationId,
                UserName = "guest",
                LoweredUserName = "guest",
                IsAnonymous = true,
                LastActivityDate = DateTime.Now,
                MobileAlias = "",


                aspnet_Membership = new aspnet_Membership()
                {
                    AccessFailedCount = 0,
                    ApplicationId = ApplicationInfo.ApplicationId,
                    aspnet_Applications = ApplicationInfo,
                    Comment = "",
                    CreateDate = DateTime.Now.Date,
                    Email = "anonymous@localhost.local",
                    EmailConfirmed = true,
                    FailedPasswordAnswerAttemptCount = 0,
                    FailedPasswordAnswerAttemptWindowStart = new DateTime(1754, 1, 1),
                    FailedPasswordAttemptCount = 0,
                    FailedPasswordAttemptWindowStart = new DateTime(1754, 1, 1),
                    IsApproved = true,
                    IsLockedOut = false,
                    LastLockoutDate = new DateTime(1754, 1, 1),
                    LastLoginDate = new DateTime(1754, 1, 1),
                    LastPasswordChangedDate = new DateTime(1754, 1, 1)
                }
            };
            guestUser.aspnet_Membership.LoweredEmail = guestUser.aspnet_Membership.Email.ToLowerInvariant();
            guestUser.aspnet_Membership.MobilePIN = "123456";
            guestUser.aspnet_Membership.Password =
                Membership.GeneratePassword(Membership.MinRequiredPasswordLength,
                Membership.MinRequiredNonAlphanumericCharacters);

            guestUser.aspnet_Membership.PasswordAnswer = "(none)";
            guestUser.aspnet_Membership.PasswordFormat = (int)MembershipPasswordFormat.Hashed;
            guestUser.aspnet_Membership.PasswordQuestion = "(none)";
            guestUser.aspnet_Membership.PasswordSalt = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            PasswordHasher pwdhasher = new PasswordHasher();

            pwdhasher.HashPassword(guestUser.aspnet_Membership.Password
                + guestUser.aspnet_Membership.PasswordSalt);

            guestUser.aspnet_Membership.PhoneConfirmed = true;
            guestUser.aspnet_Membership.PhoneNumber = "0901123456";
            guestUser.aspnet_Membership.ResetPasswordToken = "";
            guestUser.aspnet_Membership.UserId = guestUser.Id;

            CreateAsync(guestUser);

            var asynctaskb = AddToRoleAsync(guestUser, "Guests");

        }

        private bool CheckCurrentAppHasRoles()
        {
            IRoleStore<aspnet_Roles, Guid> roleStore = this;

            if (ApplicationInfo != null)
            {
                if (ApplicationInfo.aspnet_Roles.Count == 0)
                {
                    return false;
                }

                aspnet_Roles[] defaultRoles =
                    JsonConvert.DeserializeObject<aspnet_Roles[]>(
                        this.ReadTextFile(HttpContext.Current.Server.MapPath("~/Files/Sys_Init/DefaultRoles.json")));

                bool roleexisted = true;

                foreach (var checkRole in defaultRoles)
                {
                    var checkTask = roleStore.FindByNameAsync(checkRole.Name);

                    if (checkTask.Result == null)
                    {
                        roleexisted = false;
                        break;
                    }
                }
                return roleexisted;
            }

            return false;
        }

        private void CreateDefaultRoles()
        {
            aspnet_Applications appInfo = ApplicationInfo;
            string jsonInitContext = this.ReadTextFile(HttpContext.Current.Server.MapPath("~/Files/Sys_Init/DefaultRoles.json"));

            aspnet_Roles[] defaultRoles = JsonConvert.DeserializeObject<aspnet_Roles[]>(jsonInitContext);

            if (defaultRoles.Any())
            {
                foreach (var role in defaultRoles)
                {
                    role.ApplicationId = ApplicationInfo.ApplicationId;
                    role.Id = Guid.NewGuid();
                }
            }

            IRoleStore<aspnet_Roles, Guid> RoleStore = this;

            foreach (var checkRole in defaultRoles)
            {
                if (!Roles.Where(w => w.LoweredRoleName == checkRole.LoweredRoleName).Any())
                {
                    var task = RoleStore.CreateAsync(checkRole);

                    if (task.Status != TaskStatus.RanToCompletion)
                    {
                        task.Wait();
                    }
                }
            }

        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 處置 Managed 狀態 (Managed 物件)。

                }

                // TODO: 釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下方的完成項。
                // TODO: 將大型欄位設為 null。

                disposedValue = true;
            }
        }

        // TODO: 僅當上方的 Dispose(bool disposing) 具有會釋放 Unmanaged 資源的程式碼時，才覆寫完成項。
        // ~EdiuxAspNetSqlUserStore() {
        //   // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 加入這個程式碼的目的在正確實作可處置的模式。
        public void Dispose()
        {
            // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Application Store
        public bool IsExisted(Expression<Func<aspnet_Applications, bool>> filiter)
        {
            //檢查Application是否已經註冊?
            IApplicationStore<aspnet_Applications, Guid> appStore = this;

            var getAppNameTask = appStore.GetApplicationNameFromConfiguratinFileAsync();

            if (getAppNameTask.Status != TaskStatus.RanToCompletion)
            {
                getAppNameTask.Wait();
            }

            string appName = getAppNameTask.Result.ToLowerInvariant();

            return appStore.Applications.Where(filiter).Any();

        }

        public Task<bool> IsExistedAsync(Expression<Func<aspnet_Applications, bool>> filiter)
        {
            try
            {
                return Task.FromResult(IsExisted(filiter));
            }
            catch (Exception ex)
            {
                return Task.FromException<bool>(ex);
            }
        }

        public aspnet_Applications CreateDataEntityInstance()
        {
            return aspnet_Applications.Create(GetApplicationNameFromConfiguratinFile());
        }

        public Task<aspnet_Applications> CreateDataEntityInstanceAsync()
        {
            try
            {
                return Task.FromResult(CreateDataEntityInstance());
            }
            catch (Exception ex)
            {
                return Task.FromException<aspnet_Applications>(ex);
            }
        }

        public aspnet_Applications GetDataEntityInstanceByQuery(Guid key)
        {
            Iaspnet_ApplicationsRepository appRepo = pcontext.Get<Iaspnet_ApplicationsRepository>();
            return appRepo.Get(key);
        }

        public Task<aspnet_Applications> GetDataEntityInstanceByQueryAsync(Guid key)
        {
            try
            {
                return Task.FromResult(GetDataEntityInstanceByQuery(key));
            }
            catch (Exception ex)
            {
                return Task.FromException<aspnet_Applications>(ex);
            }
        }

        public IEnumerable<aspnet_Applications> FindDataEntitiesByQuery(Guid key)
        {
            Iaspnet_ApplicationsRepository appRepo = pcontext.Get<Iaspnet_ApplicationsRepository>();
            return appRepo.Where(w => w.ApplicationId == key).ToList();
        }

        public void Add(aspnet_Applications entity)
        {
            Iaspnet_ApplicationsRepository appRepo = pcontext.Get<Iaspnet_ApplicationsRepository>();

            appRepo.Add(entity);
            appRepo.UnitOfWork.Commit();

            entity = appRepo.Reload(entity);

            if (CheckCurrentAppHasRoles() == false)
            {
                CreateDefaultRoles();
            }

            if (IsHasRootUser == false)
            {
                CreateRootUser();
            }

            if (ApplicationInfo.aspnet_Users.Any(s => s.LoweredUserName == "guest") == false)
            {
                CreateAnonymousUser();
            }

            if (GetCheckRootUserHasAdminsRole() == false)
            {
                AddRootUserToAdminsRole();
            }
        }

        public Task AddAsync(aspnet_Applications entity)
        {
            try
            {
                Add(entity);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }

        public aspnet_Applications GetEntityByQuery(Guid key)
        {
            Iaspnet_ApplicationsRepository appRepo = pcontext.Get<Iaspnet_ApplicationsRepository>();
            return appRepo.Get(key);
        }

        public IEnumerable<TResult> FindEntitiesByQuery<TResult>(Guid key, Expression<Func<aspnet_Applications, TResult>> selector = null)
        {
            Iaspnet_ApplicationsRepository appRepo = pcontext.Get<Iaspnet_ApplicationsRepository>();
            if (selector != null)
            {
                List<TResult> result = appRepo.All().Where(w => w.ApplicationId == key).Select(selector).ToList();
                return result;
            }
            else
            {
                List<aspnet_Applications> result = appRepo.All().Where(w => w.ApplicationId == key).ToList();
                return result.Cast<TResult>();
            }
        }

        public bool Update(aspnet_Applications entity)
        {
            throw new NotImplementedException();
        }

        public bool Update(IEnumerable<aspnet_Applications> entities)
        {
            throw new NotImplementedException();
        }

        public bool Delete(aspnet_Applications entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(IEnumerable<aspnet_Applications> entities)
        {
            throw new NotImplementedException();
        }

        public Task<aspnet_Applications> GetEntityByQueryAsync(Guid key)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TResult>> FindEntitiesByQueryAsync<TResult>(Guid key, Expression<Func<aspnet_Applications, TResult>> selector = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(aspnet_Applications entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(IEnumerable<aspnet_Applications> entities)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(aspnet_Applications entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(IEnumerable<aspnet_Applications> entities)
        {
            throw new NotImplementedException();
        }

        public string GetApplicationNameFromConfiguratinFile()
        {
            string appName = ConfigHelper.GetConfig(ApplicationName);
            return appName;
        }

        public Task<string> GetApplicationNameFromConfiguratinFileAsync()
        {
            try
            {
                return Task.FromResult(GetApplicationNameFromConfiguratinFile());
            }
            catch (Exception ex)
            {
                return Task.FromException<string>(ex);
            }
        }

        public aspnet_Applications GetEntityByQuery(string ApplicationName)
        {
            throw new NotImplementedException();
        }

        public Task<aspnet_Applications> GetEntityByQueryAsync(string ApplicationName)
        {
            throw new NotImplementedException();
        }

        public void Initialization()
        {
            IApplicationStore<aspnet_Applications, Guid> applicationStore = this;

            if (CheckCurrentAppIsRegistered == false)
            {
                aspnet_Applications newApplication = applicationStore.CreateDataEntityInstance();

                var getAppNameTask = GetApplicationNameFromConfiguratinFileAsync();

                if (getAppNameTask.Status == TaskStatus.Running)
                {
                    getAppNameTask.Wait();
                }

                string applicationName = getAppNameTask.Result;

                newApplication.ApplicationName = applicationName;
                newApplication.Description = applicationName;
                newApplication.LoweredApplicationName = applicationName.ToLowerInvariant();

                var AddApplicationTask = applicationStore.AddAsync(newApplication);

                if (AddApplicationTask.Status == TaskStatus.Running)
                {
                    AddApplicationTask.Wait();
                }

                SetToMemoryCache();

            }



        }

        public Task InitializationAsync()
        {
            try
            {
                Initialization();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }

        }


        //public Task CreateApplicationIfNotExisted()
        //{
        //    //檢查Application是否已經註冊?

        //    if (CheckCurrentAppIsRegistered == false)
        //    {
        //        aspnet_Applications newApplication = new aspnet_Applications();

        //        var getAppNameTask = GetApplicationNameFromConfiguratinFileAsync();

        //        string applicationName = getAppNameTask.Result;

        //        newApplication.ApplicationName = applicationName;
        //        newApplication.Description = applicationName;
        //        newApplication.LoweredApplicationName = applicationName.ToLowerInvariant();

        //        CreateAsync(newApplication);

        //        Iaspnet_ApplicationsRepository appRepo = pcontext.Get<Iaspnet_ApplicationsRepository>();

        //        applicationInfo =
        //            appRepo.CopyTo<aspnet_Applications>(GetCurrentApplicationInfoAsync().Result);

        //        SetToMemoryCache();

        //    }
        //    else
        //    {
        //        Iaspnet_ApplicationsRepository appRepo = pcontext.Get<Iaspnet_ApplicationsRepository>();

        //        applicationInfo = appRepo.CopyTo<aspnet_Applications>(GetCurrentApplicationInfoAsync().Result);
        //    }

        //    if (CheckCurrentAppHasRoles() == false)
        //    {
        //        CreateDefaultRoles();
        //    }

        //    if (IsHasRootUser == false)
        //    {
        //        CreateRootUser();
        //    }

        //    if (applicationInfo.aspnet_Users.Any(s => s.LoweredUserName == "guest") == false)
        //    {
        //        CreateAnonymousUser();
        //    }

        //    if (GetCheckRootUserHasAdminsRole() == false)
        //    {
        //        AddRootUserToAdminsRole();
        //    }

        //    return Task.CompletedTask;
        //}

        //public Task CreateAsync(aspnet_Applications app)
        //{
        //    aspnet_Applications newApplication = new aspnet_Applications()
        //    {
        //        ApplicationId = app.ApplicationId,
        //        ApplicationName = app.ApplicationName,
        //        Description = app.Description,
        //        LoweredApplicationName = app.LoweredApplicationName
        //    };

        //    Iaspnet_ApplicationsRepository appRepo = pcontext.Get<Iaspnet_ApplicationsRepository>();
        //    appRepo.Add(newApplication);
        //    appRepo.UnitOfWork.Commit();

        //    newApplication = appRepo.Reload(newApplication);
        //    app = appRepo.CopyTo<aspnet_Applications>(newApplication);

        //    return Task.CompletedTask;
        //}

        //public Task DeleteAsync(aspnet_Applications app)
        //{
        //    Iaspnet_ApplicationsRepository appRepo = pcontext.Get<Iaspnet_ApplicationsRepository>();
        //    appRepo.Delete(app);
        //    return appRepo.UnitOfWork.CommitAsync();
        //}

        //public Task UpdateAsync(aspnet_Applications app)
        //{
        //    Iaspnet_ApplicationsRepository appRepo = pcontext.Get<Iaspnet_ApplicationsRepository>();
        //    var existapp = appRepo.Get(app.ApplicationId);
        //    existapp = appRepo.CopyTo<aspnet_Applications>(app);
        //    return appRepo.UnitOfWork.CommitAsync();
        //}

        //public Task<string> GetApplicationNameFromConfiguratinFileAsync()
        //{
        //    string appName = ConfigHelper.GetConfig(ApplicationName);
        //    return Task.FromResult(appName);
        //}

        //Task<IEnumerable<aspnet_Applications>> IApplicationStore<aspnet_Applications, Guid>.FindByIdAsync(Guid appId)
        //{
        //    Iaspnet_ApplicationsRepository appRepo = pcontext.Get<Iaspnet_ApplicationsRepository>();
        //    return Task.FromResult(appRepo.FindById(appId));
        //}

        //Task<IEnumerable<aspnet_Applications>> IApplicationStore<aspnet_Applications, Guid>.FindByNameAsync(string appName)
        //{
        //    Iaspnet_ApplicationsRepository appRepo = pcontext.Get<Iaspnet_ApplicationsRepository>();
        //    return Task.FromResult(appRepo.FindByName(appName));
        //}

        //public Task<aspnet_Applications> GetByIdAsync(Guid appId)
        //{
        //    Iaspnet_ApplicationsRepository appRepo = pcontext.Get<Iaspnet_ApplicationsRepository>();
        //    return appRepo.GetAsync(appId);
        //}

        //public Task<aspnet_Applications> GetByNameAsync(string appName)
        //{
        //    return Task.FromResult((from app in Applications
        //                            where app.ApplicationName == appName ||
        //                            app.LoweredApplicationName == appName
        //                            select app).ToList().SingleOrDefault());
        //}

        //public async Task<aspnet_Applications> GetCurrentApplicationInfoAsync()
        //{
        //    string appName = await GetApplicationNameFromConfiguratinFileAsync();
        //    return await GetByNameAsync(appName);
        //}
        #endregion

        //#region Page Setting
        //public Task<PageSettingByUserViewModel> GetAsync(IController controller)
        //{
        //    Task<aspnet_Paths> getPathTask = ((IPageSettingsBaseStore<aspnet_Paths, Guid>)this).GetAsync(controller);

        //    if (!getPathTask.IsCompleted)
        //    {
        //        getPathTask.Wait();
        //    }

        //    aspnet_Paths PathData = getPathTask.Result;

        //    if (PathData != null)
        //    {
        //        var userId = (controller as Controller).User.Identity.GetUserId();

        //        var PageSettingDatas = (from d in PathData.aspnet_PersonalizationPerUser
        //                                where d.UserId == userId
        //                                select d).ToList();

        //        if (PageSettingDatas.Any())
        //        {
        //            return Task.FromResult(PageSettingDatas.Single().PageSettings.Deserialize<PageSettingByUserViewModel>());
        //        }

        //    }

        //    return Task.FromResult(default(PageSettingByUserViewModel));
        //}

        //public Task<bool> IsHasProfileAsync(IController controller)
        //{
        //    IProfileStore<UserProfileViewModel, aspnet_Profile, Guid> profilestore = (IProfileStore<UserProfileViewModel, aspnet_Profile, Guid>)this;
        //    IPageSettingsBaseStore<aspnet_PersonalizationPerUser, Guid> PageSettingsViaUser = (IPageSettingsBaseStore<aspnet_PersonalizationPerUser, Guid>)this;

        //    var GetProfileTask = profilestore.GetAsync(controller);

        //    var GetPathTask = ((IPageSettingsBaseStore<aspnet_Paths, Guid>)this).GetAsync(controller);

        //    if (!GetPathTask.IsCompleted)
        //    {
        //        GetPathTask.Wait();
        //    }

        //    var pathInfo = GetPathTask.Result;

        //    if (pathInfo != null)
        //    {

        //    }
        //}

        //public Task<bool> IsHasBasePageSetting(IController controller)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> IsHasUserPageSetting(IController controller)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task InitializationProfileAsync(IController controller)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<PageSettingByUserViewModel> UpdateAsync(PageSettingByUserViewModel model)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task RemoveAsync(IController controller, PageSettingByUserViewModel model)
        //{
        //    throw new NotImplementedException();
        //}

        //Task<aspnet_Paths> IPageSettingsBaseStore<aspnet_Paths, Guid>.GetAsync(IController controller)
        //{
        //    Controller ctr = controller as Controller;
        //    string cPath = ctr.Request.Path.ToLowerInvariant();

        //    var pathDataTask = (from app in Applications
        //                        from paths in app.aspnet_Paths
        //                        where paths.LoweredPath == cPath
        //                        select paths).ToListAsync();

        //    if (!pathDataTask.IsCompleted)
        //    {
        //        pathDataTask.Wait();
        //    }

        //    var pathDatas = pathDataTask.Result;

        //    if (pathDatas.Any())
        //    {
        //        return Task.FromResult(pathDatas.Single());
        //    }

        //    return Task.FromResult(default(aspnet_Paths));
        //}

        //public Task<bool> CheckPathHasRegisteredAsync(IController controller, ActionDescriptor ActionDescriptor)
        //{
        //    Controller ctr = controller as Controller;

        //    aspnet_Paths pathInfo;
        //    string url = ctr.Request.Path;
        //    string loweredUrl = url.ToLowerInvariant();
        //    var pathRepo = pcontext.Get<Iaspnet_PathsRepository>();

        //    pathInfo = pathRepo.Where(w => (w.Path == url || w.LoweredPath == loweredUrl)
        //     && w.ApplicationId == applicationInfo.ApplicationId).SingleOrDefault();

        //    if (pathInfo != null)
        //    {
        //        if (pathInfo.aspnet_PersonalizationAllUsers == null)
        //        {
        //            pathInfo.aspnet_PersonalizationAllUsers = aspnet_PersonalizationAllUsers.Create(pathInfo);
        //            //    new aspnet_PersonalizationAllUsers()
        //            //{
        //            //    PathId = pathInfo.PathId,
        //            //    LastUpdatedDate = DateTime.UtcNow,
        //            //};

        //            PageSettingsBaseModel BaseSetting = PageSettingsBaseModel.Create(
        //                ctr.RouteData.DataTokens.ContainsKey("area") ? ctr.RouteData.DataTokens["area"].ToString() : string.Empty,
        //                ActionDescriptor.ActionName,
        //                ActionDescriptor.ControllerDescriptor.ControllerName,
        //                ActionDescriptor.GetParameters().ToDictionary(s => s.ParameterName, v => v.DefaultValue));

        //            #region 尋找原始的授權屬性
        //            var auth = ActionDescriptor.GetCustomAttributes(typeof(AuthorizeAttribute), true)
        //                .Select(s => (AuthorizeAttribute)s).ToList().SingleOrDefault();

        //            if (auth != null)
        //            {
        //                var allowRoles = auth.Roles.ToLowerInvariant().Split(',');

        //                foreach (var r in allowRoles)
        //                {
        //                    BaseSetting.AllowExcpetionRoles.Add(r, true);
        //                }

        //                var allowUsers = auth.Users.ToLowerInvariant().Split(',');

        //                foreach (var u in allowUsers)
        //                {
        //                    BaseSetting.AllowExcpetionUsers.Add(u, true);
        //                }
        //            }

        //            #endregion

        //            #region 尋找自訂的授權屬性
        //            var adbuth = ActionDescriptor.GetCustomAttributes(typeof(DbAuthorizeAttribute), true)
        //                .Select(s => (DbAuthorizeAttribute)s).ToList().SingleOrDefault();

        //            if (auth != null)
        //            {
        //                var allowRoles = auth.Roles.ToLowerInvariant().Split(',');

        //                foreach (var r in allowRoles)
        //                {
        //                    BaseSetting.AllowExcpetionRoles.Add(r, true);
        //                }

        //                var allowUsers = auth.Users.ToLowerInvariant().Split(',');

        //                foreach (var u in allowUsers)
        //                {
        //                    BaseSetting.AllowExcpetionUsers.Add(u, true);
        //                }
        //            }
        //            #endregion

        //            if (ctr.ViewBag.Title != null)
        //            {
        //                BaseSetting.Title = ctr.ViewBag.Title as string;
        //            }

        //            pathInfo.aspnet_PersonalizationAllUsers.PageSettings = BaseSetting.Serialize();


        //            pathRepo.Update(pathInfo);
        //            pathInfo = pathRepo.Reload(pathInfo);
        //        }

        //        Guid userId =
        //            ctr.HttpContext.User.Identity.GetUserId();

        //        if (!pathInfo.aspnet_PersonalizationPerUser.Any(w => w.UserId == userId))
        //        {
        //            if (pathInfo.aspnet_PersonalizationAllUsers.PageSettings != null)
        //            {
        //                PageSettingsBaseModel BaseSetting = pathInfo.aspnet_PersonalizationAllUsers.PageSettings.Deserialize<PageSettingsBaseModel>();

        //                if (BaseSetting != null)
        //                {
        //                    aspnet_PersonalizationPerUser perUserSetting = new aspnet_PersonalizationPerUser()
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        LastUpdatedDate = DateTime.UtcNow,
        //                        PathId = pathInfo.PathId,
        //                        UserId = userId
        //                    };

        //                    PageSettingByUserViewModel perUserModel = new PageSettingByUserViewModel(pathInfo.aspnet_PersonalizationAllUsers, userId);

        //                    perUserSetting.PageSettings = perUserModel.Serialize();

        //                    pathInfo.aspnet_PersonalizationPerUser.Add(perUserSetting);

        //                    pathRepo.Update(pathInfo);
        //                    pathInfo = pathRepo.Reload(pathInfo);
        //                }
        //            }


        //        }
        //    }
        //    return pathInfo;

        //    Controller ctr = controller as Controller;
        //    string cPath = ctr.Request.Path.ToLowerInvariant();

        //    var pathDataTask = (from app in Applications
        //                        from paths in app.aspnet_Paths
        //                        where paths.LoweredPath == cPath
        //                        select paths).ToListAsync();

        //    if (!pathDataTask.IsCompleted)
        //    {
        //        pathDataTask.Wait();
        //    }

        //    var pathDatas = pathDataTask.Result;

        //    return Task.FromResult(pathDatas.Any());
        //}

        //public Task RegisterControllerAsync(IController controller)
        //{
        //    Controller ctr = controller as Controller;

        //    if (ctr == null)
        //    {
        //        throw new ArgumentException("{0} casted failed.", nameof(controller));
        //    }
        //    aspnet_Applications appInfo = ctr.ViewBag.ApplicationInfo;

        //    if (appInfo == null)
        //    {
        //        throw new Exception("System not found.");
        //    }

        //    aspnet_Users loginedUser = ctr.ViewBag.LoginedUser as aspnet_Users;
        //    aspnet_Paths newPath = aspnet_Paths.Create(ctr, appInfo);
        //    newPath.aspnet_PersonalizationAllUsers = aspnet_PersonalizationAllUsers.Create(newPath);
        //    newPath.aspnet_PersonalizationPerUser = aspnet_PersonalizationPerUser.CreateCollection(loginedUser,
        //        newPath.aspnet_PersonalizationAllUsers);
        //    Iaspnet_PathsRepository PathRepo = pcontext.Get<Iaspnet_PathsRepository>();
        //    PathRepo.Add(newPath);

        //    return PathRepo.UnitOfWork.CommitAsync();
        //}

        //public Task UnRegisterControllerAsync(IController controller)
        //{
        //    Iaspnet_PathsRepository PathRepo = pcontext.Get<Iaspnet_PathsRepository>();
        //    var TaskGetPath = ((IPageSettingsBaseStore<aspnet_Paths, Guid>)this).GetAsync(controller);

        //    if (!TaskGetPath.IsCompleted)
        //    {
        //        TaskGetPath.Wait();
        //    }

        //    PathRepo.Delete(TaskGetPath.Result);

        //    return PathRepo.UnitOfWork.CommitAsync();
        //}

        //public Task ReRegisterControllerAsync(IController controller, aspnet_Paths entity)
        //{
        //    UnRegisterControllerAsync(controller);
        //    RegisterControllerAsync(controller);
        //    return Task.CompletedTask;
        //}

        //public Task<bool> CheckPathHasRegisteredAsync(IController controller)
        //{
        //    throw new NotImplementedException();
        //}
        //#endregion

        //#region Profile Store

        //#endregion
        public const string ApplicationInfoKey = "ApplicationInfo";
        internal const string ApplicationName = "ApplicationName";


    }
}