using EdiuxTemplateWebApp.Helpers;
using EdiuxTemplateWebApp.Models.AspNetModels;
using EdiuxTemplateWebApp.Models.Shared;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace EdiuxTemplateWebApp.Models
{
    public class EdiuxAspNetSqlUserStore : IEdiuxAspNetSqlUserStore
    {
        #region 變數宣告區
        //protected IUnitOfWork UnitOfWork;
        //private Iaspnet_ApplicationsRepository appRepo;
        //private Iaspnet_UsersRepository userRepo;
        //private Iaspnet_RolesRepository roleRepo;
        //private Iaspnet_UserLoginRepository userloginRepo;
        //private Iaspnet_MembershipRepository membershipRepo;
        private aspnet_Applications applicationInfo;

        private IOwinContext pcontext;

        public static IEdiuxAspNetSqlUserStore Create(IdentityFactoryOptions<IEdiuxAspNetSqlUserStore> options, IOwinContext context)
        {
            return new EdiuxAspNetSqlUserStore(context);
        }

        public EdiuxAspNetSqlUserStore(IOwinContext context)
        {
            pcontext = context;

            //UnitOfWork = pcontext.Get<IUnitOfWork>();

            //appRepo = context.Get<Iaspnet_ApplicationsRepository>();
            //userRepo = context.Get<Iaspnet_UsersRepository>();
            //membershipRepo = context.Get<Iaspnet_MembershipRepository>();
            //roleRepo = context.Get<Iaspnet_RolesRepository>();
            //userloginRepo = context.Get<Iaspnet_UserLoginRepository>();

            //appRepo.UnitOfWork = UnitOfWork;
            //userRepo.UnitOfWork = UnitOfWork;
            //membershipRepo.UnitOfWork = UnitOfWork;
            //roleRepo.UnitOfWork = UnitOfWork;
            //userloginRepo.UnitOfWork = UnitOfWork;

            var asynctask = createApplicationIfNotExisted();

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
                return applicationInfo.aspnet_Users.AsQueryable();
            }
        }
        #endregion

        #region Queryable Role Store
        public IQueryable<aspnet_Roles> Roles
        {
            get
            {
                return applicationInfo.aspnet_Roles.AsQueryable();
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
                user.ApplicationId = applicationInfo.ApplicationId;
                user.aspnet_Applications = applicationInfo;
            }

            Iaspnet_UsersRepository userRepo = pcontext.Get<Iaspnet_UsersRepository>();

            var users = userRepo.GetUserByName(applicationInfo.ApplicationName,
                                               user.UserName,
                                               DateTime.UtcNow,
                                               true);

            aspnet_Users newUser = new aspnet_Users();

            if (users == null)
            {
                newUser.Id = Guid.NewGuid();
                newUser.ApplicationId = user.ApplicationId;
                newUser.aspnet_Applications = user.aspnet_Applications;
                newUser.IsAnonymous = false;
                newUser.LastActivityDate = DateTime.UtcNow;
                if (string.IsNullOrEmpty(user.LoweredUserName))
                {
                    if (string.IsNullOrEmpty(user.UserName))
                    {
                        throw new ArgumentException("user.UserName 不能為空或Null.");
                    }
                    else
                    {
                        newUser.UserName = user.UserName;
                        newUser.LoweredUserName = user.UserName.ToLowerInvariant();
                    }
                }
                else
                {
                    newUser.LoweredUserName = user.LoweredUserName;
                }
                if (string.IsNullOrEmpty(user.UserName))
                {
                    throw new ArgumentException("user.UserName 不能為空或Null.");
                }
                else
                {
                    newUser.UserName = user.UserName;
                }
                if (!string.IsNullOrEmpty(user.MobileAlias))
                {
                    newUser.MobileAlias = user.MobileAlias;
                }

                newUser = userRepo.CopyTo<aspnet_Users>(user);
            }
            else
            {
                newUser = userRepo.GetUserByName(user.aspnet_Applications.ApplicationName, user.UserName, DateTime.UtcNow, true);
                newUser = userRepo.CopyTo<aspnet_Users>(user);
                user = userRepo.Update(newUser);
                return Task.CompletedTask;
            }

            if (newUser.aspnet_Membership == null)
            {
                if (user.aspnet_Membership != null)
                {
                    newUser.aspnet_Membership.UserId = newUser.Id;
                }
                else
                {
                    newUser.aspnet_Membership = new aspnet_Membership();

                    newUser.aspnet_Membership.AccessFailedCount = 0;
                    newUser.aspnet_Membership.ApplicationId = applicationInfo.ApplicationId;
                    newUser.aspnet_Membership.aspnet_Applications = applicationInfo;
                    newUser.aspnet_Membership.Comment = "";
                    newUser.aspnet_Membership.CreateDate = DateTime.Now.Date;
                    newUser.aspnet_Membership.Email = newUser.LoweredUserName + "@localhost.local";
                    newUser.aspnet_Membership.EmailConfirmed = true;
                    newUser.aspnet_Membership.FailedPasswordAnswerAttemptCount = 0;
                    newUser.aspnet_Membership.FailedPasswordAnswerAttemptWindowStart = new DateTime(1754, 1, 1);
                    newUser.aspnet_Membership.FailedPasswordAttemptCount = 0;
                    newUser.aspnet_Membership.FailedPasswordAttemptWindowStart = new DateTime(1754, 1, 1);
                    newUser.aspnet_Membership.IsApproved = true;
                    newUser.aspnet_Membership.IsLockedOut = false;
                    newUser.aspnet_Membership.LastLockoutDate = new DateTime(1754, 1, 1);
                    newUser.aspnet_Membership.LastLoginDate = new DateTime(1754, 1, 1);
                    newUser.aspnet_Membership.LastPasswordChangedDate = new DateTime(1754, 1, 1);
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
            }

            if (user.aspnet_Profile == null)
            {
                newUser.aspnet_Profile = new aspnet_Profile();
                newUser.aspnet_Profile.LastUpdatedDate = DateTime.UtcNow;

                UserProfileViewModel newProfile = new UserProfileViewModel();

                newProfile.AvatarFilePath = "/Content/images/user.jpg";
                newProfile.CompanyName = "Ediux Workshop";
                newProfile.PositionTitle = "PG";
                newProfile.CompanyWebSiteURL = "http://www.riaxe.com/";

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
                List<aspnet_UserClaims> claims = new List<aspnet_UserClaims>();
                claims.Add(new aspnet_UserClaims()
                {
                    UserId = newUser.Id,
                    ClaimType = ClaimTypes.Email,
                    ClaimValue = newUser.aspnet_Membership.LoweredEmail,
                    Id = 0
                });
                claims.Add(new aspnet_UserClaims()
                {
                    UserId = newUser.Id,
                    ClaimType = ClaimTypes.Name,
                    ClaimValue = newUser.UserName,
                    Id = 1
                });
                claims.Add(new aspnet_UserClaims()
                {
                    UserId = newUser.Id,
                    ClaimType = ClaimTypes.NameIdentifier,
                    ClaimValue = user.Id.ToString(),
                    Id = 2
                });
                claims.Add(new aspnet_UserClaims()
                {
                    UserId = newUser.Id,
                    ClaimType = ClaimTypes.Email,
                    ClaimValue = "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                    Id = 3
                });
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
                                           w.ApplicationId == applicationInfo.ApplicationId)
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
                                   && w.ApplicationId == applicationInfo
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
                    .Where(x => x.ApplicationId == applicationInfo.ApplicationId
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
                    user.ApplicationId = applicationInfo.ApplicationId;
                    user.aspnet_Applications = applicationInfo;
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
                user.ApplicationId = applicationInfo.ApplicationId;
                user.aspnet_Applications = applicationInfo;

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
                    applicationInfo.ApplicationId, roleName)
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
                        applicationInfo.ApplicationName,
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
                        founduser.LockoutEndDate.HasValue ?
                        founduser.LockoutEndDate.Value : new DateTime(1754, 1, 1)));
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
        #endregion

        #region Helper Functions

        private bool checkCurrentAppIsRegistered()
        {
            try
            {
                var getAppNameTask = GetApplicationNameFromConfiguratinFileAsync();

                string appName = getAppNameTask.Result;

                Iaspnet_ApplicationsRepository appRepo = pcontext.Get<Iaspnet_ApplicationsRepository>();

                if (!appRepo.FindByName(appName).Any())
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }

        }

        private void setToMemoryCache()
        {
            this.setApplicationGlobalVariable(ApplicationInfoKey, applicationInfo);
        }

        protected virtual void WriteErrorLog(Exception ex)
        {
            if (System.Web.HttpContext.Current == null)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
            else
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }

        private bool checkRootUserHasAdminsRole()
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

        private void addRootUserToAdminsRole()
        {
            if (applicationInfo.aspnet_Roles.Any(s => s.Name.Equals("Admins", StringComparison.InvariantCultureIgnoreCase)) == false)
                throw new NullReferenceException(string.Format("The role of name, '{0}', is not found.", "Admins"));

            Iaspnet_UsersRepository userRepo = pcontext.Get<Iaspnet_UsersRepository>();

            aspnet_Users rootUser = userRepo.GetUserByName(applicationInfo.ApplicationName, "root", DateTime.UtcNow, false);

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
                if (applicationInfo != null)
                {
                    return applicationInfo.aspnet_Users.Any(s => s.UserName.Equals("root", StringComparison.InvariantCultureIgnoreCase));
                }

                return false;
            }
        }

        private void CreateRootUser()
        {
            aspnet_Users rootUser = new aspnet_Users();
            rootUser.Id = Guid.NewGuid();
            rootUser.ApplicationId = applicationInfo.ApplicationId;
            rootUser.UserName = "root";
            rootUser.LoweredUserName = "root";
            rootUser.IsAnonymous = false;
            rootUser.LastActivityDate = DateTime.Now;
            rootUser.MobileAlias = "";

            rootUser.aspnet_Membership = new aspnet_Membership();
            rootUser.aspnet_Membership.AccessFailedCount = 0;
            rootUser.aspnet_Membership.ApplicationId = applicationInfo.ApplicationId;
            rootUser.aspnet_Membership.aspnet_Applications = applicationInfo;
            rootUser.aspnet_Membership.Comment = "";
            rootUser.aspnet_Membership.CreateDate = DateTime.Now.Date;
            rootUser.aspnet_Membership.Email = "root@localhost.local";
            rootUser.aspnet_Membership.EmailConfirmed = true;
            rootUser.aspnet_Membership.FailedPasswordAnswerAttemptCount = 0;
            rootUser.aspnet_Membership.FailedPasswordAnswerAttemptWindowStart = new DateTime(1754, 1, 1);
            rootUser.aspnet_Membership.FailedPasswordAttemptCount = 0;
            rootUser.aspnet_Membership.FailedPasswordAttemptWindowStart = new DateTime(1754, 1, 1);
            rootUser.aspnet_Membership.IsApproved = true;
            rootUser.aspnet_Membership.IsLockedOut = false;
            rootUser.aspnet_Membership.LastLockoutDate = new DateTime(1754, 1, 1);
            rootUser.aspnet_Membership.LastLoginDate = new DateTime(1754, 1, 1);
            rootUser.aspnet_Membership.LastPasswordChangedDate = new DateTime(1754, 1, 1);
            rootUser.aspnet_Membership.LoweredEmail = rootUser.aspnet_Membership.Email.ToLowerInvariant();
            rootUser.aspnet_Membership.MobilePIN = "123456";
            rootUser.aspnet_Membership.Password = "!QAZ2wsx";
            rootUser.aspnet_Membership.PasswordSalt = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            rootUser.aspnet_Membership.PasswordAnswer = "";
            rootUser.aspnet_Membership.PasswordFormat = (int)MembershipPasswordFormat.Hashed;
            rootUser.aspnet_Membership.PasswordQuestion = "";
            rootUser.aspnet_Membership.PhoneConfirmed = true;
            rootUser.aspnet_Membership.PhoneNumber = "0901123456";
            rootUser.aspnet_Membership.ResetPasswordToken = "";

            var asynctask = CreateAsync(rootUser);
            asynctask.ContinueWith((x) =>
            {
                var asynctaskb = AddToRoleAsync(rootUser, "Admins");
                asynctaskb.Start();
            });
        }

        private void CreateAnonymousUser()
        {
            aspnet_Users guestUser = new aspnet_Users();
            guestUser.Id = Guid.NewGuid();
            guestUser.ApplicationId = applicationInfo.ApplicationId;
            guestUser.UserName = "guest";
            guestUser.LoweredUserName = "guest";
            guestUser.IsAnonymous = true;
            guestUser.LastActivityDate = DateTime.Now;
            guestUser.MobileAlias = "";


            guestUser.aspnet_Membership = new aspnet_Membership();
            guestUser.aspnet_Membership.AccessFailedCount = 0;
            guestUser.aspnet_Membership.ApplicationId = applicationInfo.ApplicationId;
            guestUser.aspnet_Membership.aspnet_Applications = applicationInfo;
            guestUser.aspnet_Membership.Comment = "";
            guestUser.aspnet_Membership.CreateDate = DateTime.Now.Date;
            guestUser.aspnet_Membership.Email = "anonymous@localhost.local";
            guestUser.aspnet_Membership.EmailConfirmed = true;
            guestUser.aspnet_Membership.FailedPasswordAnswerAttemptCount = 0;
            guestUser.aspnet_Membership.FailedPasswordAnswerAttemptWindowStart = new DateTime(1754, 1, 1);
            guestUser.aspnet_Membership.FailedPasswordAttemptCount = 0;
            guestUser.aspnet_Membership.FailedPasswordAttemptWindowStart = new DateTime(1754, 1, 1);
            guestUser.aspnet_Membership.IsApproved = true;
            guestUser.aspnet_Membership.IsLockedOut = false;
            guestUser.aspnet_Membership.LastLockoutDate = new DateTime(1754, 1, 1);
            guestUser.aspnet_Membership.LastLoginDate = new DateTime(1754, 1, 1);
            guestUser.aspnet_Membership.LastPasswordChangedDate = new DateTime(1754, 1, 1);
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
            Iaspnet_ApplicationsRepository appRepo = pcontext.Get<Iaspnet_ApplicationsRepository>();

            if (appRepo == null)
                appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();

            if (applicationInfo != null)
            {
                if (applicationInfo.aspnet_Roles.Count == 0)
                {
                    return false;
                }

                aspnet_Roles[] defaultRoles = new aspnet_Roles[] {
                    new aspnet_Roles() {
                        ApplicationId = applicationInfo.ApplicationId,
                         aspnet_Applications = applicationInfo,
                        Description = "系統管理員",
                        LoweredRoleName = "admins",
                        Name = "Admins",
                        Id = Guid.NewGuid()
                    },
                    new aspnet_Roles() {
                        ApplicationId = applicationInfo.ApplicationId,
                        aspnet_Applications = applicationInfo,
                        Description = "次要管理員",
                        LoweredRoleName = "coadmins",
                        Name = "CoAdmins",
                        Id = Guid.NewGuid()
                    },
                    new aspnet_Roles() {
                        ApplicationId = applicationInfo.ApplicationId,
                        aspnet_Applications = applicationInfo,
                        Description = "使用者",
                        LoweredRoleName = "users",
                        Name = "Users",
                        Id = Guid.NewGuid()
                    },
                    new aspnet_Roles() {
                        ApplicationId = applicationInfo.ApplicationId,
                        aspnet_Applications = applicationInfo,
                        Description = "訪客",
                        LoweredRoleName = "guests",
                        Name = "Guests",
                        Id = Guid.NewGuid()
                    }
                };

                Iaspnet_RolesRepository roleRepo = pcontext.Get<Iaspnet_RolesRepository>();

                bool roleexisted = true;

                foreach (var checkRole in defaultRoles)
                {
                    if (!roleRepo.IsExists(checkRole))
                    {
                        roleexisted = false;
                        break;
                    }
                }
                return roleexisted;
            }

            return false;
        }

        private void createDefaultRoles()
        {
            aspnet_Applications appInfo = GetCurrentApplicationInfoAsync().Result;

            aspnet_Roles[] defaultRoles = new aspnet_Roles[] {
                    new aspnet_Roles() {
                        ApplicationId = appInfo.ApplicationId,
                        Description = "系統管理員",
                        LoweredRoleName = "admins",
                        Name = "Admins",
                        Id = Guid.NewGuid()
                    },
                    new aspnet_Roles() {
                        ApplicationId = appInfo.ApplicationId,
                        Description = "次要管理員",
                        LoweredRoleName = "coadmins",
                        Name = "CoAdmins",
                        Id = Guid.NewGuid()
                    },
                    new aspnet_Roles() {
                        ApplicationId = appInfo.ApplicationId,
                        Description = "使用者",
                        LoweredRoleName = "users",
                        Name = "Users",
                        Id = Guid.NewGuid()
                    },
                    new aspnet_Roles() {
                        ApplicationId = applicationInfo.ApplicationId,
                        aspnet_Applications = applicationInfo,
                        Description = "訪客",
                        LoweredRoleName = "guests",
                        Name = "Guests",
                        Id = Guid.NewGuid()
                    }
                };

            Iaspnet_RolesRepository roleRepo = pcontext.Get<Iaspnet_RolesRepository>();

            roleRepo.UnitOfWork.TranscationMode = true;

            foreach (var checkRole in defaultRoles)
            {
                if (!roleRepo.IsExists(checkRole))
                {
                    roleRepo.Add(checkRole);
                }
            }

            roleRepo.UnitOfWork.TranscationMode = false;
            roleRepo.UnitOfWork.Commit();

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

        public Task createApplicationIfNotExisted()
        {
            //檢查Application是否已經註冊?

            if (checkCurrentAppIsRegistered() == false)
            {
                aspnet_Applications newApplication = new aspnet_Applications();

                var getAppNameTask = GetApplicationNameFromConfiguratinFileAsync();

                string applicationName = getAppNameTask.Result;

                newApplication.ApplicationName = applicationName;
                newApplication.Description = applicationName;
                newApplication.LoweredApplicationName = applicationName.ToLowerInvariant();

                CreateAsync(newApplication);

                Iaspnet_ApplicationsRepository appRepo = pcontext.Get<Iaspnet_ApplicationsRepository>();

                applicationInfo =
                    appRepo.CopyTo<aspnet_Applications>(GetCurrentApplicationInfoAsync().Result);

                setToMemoryCache();

            }
            else
            {
                Iaspnet_ApplicationsRepository appRepo = pcontext.Get<Iaspnet_ApplicationsRepository>();

                applicationInfo = appRepo.CopyTo<aspnet_Applications>(GetCurrentApplicationInfoAsync().Result);
            }

            if (CheckCurrentAppHasRoles() == false)
            {
                createDefaultRoles();
            }

            if (IsHasRootUser== false)
            {
                CreateRootUser();
            }

            if (applicationInfo.aspnet_Users.Any(s => s.LoweredUserName == "guest") == false)
            {
                CreateAnonymousUser();
            }

            if (checkRootUserHasAdminsRole() == false)
            {
                addRootUserToAdminsRole();
            }

            return Task.CompletedTask;
        }

        public Task CreateAsync(aspnet_Applications app)
        {
            aspnet_Applications newApplication = new aspnet_Applications();

            newApplication.ApplicationId = app.ApplicationId;
            newApplication.ApplicationName = app.ApplicationName;
            newApplication.Description = app.Description;
            newApplication.LoweredApplicationName = app.LoweredApplicationName;

            Iaspnet_ApplicationsRepository appRepo = pcontext.Get<Iaspnet_ApplicationsRepository>();
            appRepo.Add(newApplication);
            appRepo.UnitOfWork.Commit();

            newApplication = appRepo.Reload(newApplication);
            app = appRepo.CopyTo<aspnet_Applications>(newApplication);

            return Task.CompletedTask;
        }

        public Task DeleteAsync(aspnet_Applications app)
        {
            Iaspnet_ApplicationsRepository appRepo = pcontext.Get<Iaspnet_ApplicationsRepository>();
            appRepo.Delete(app);
            return appRepo.UnitOfWork.CommitAsync();
        }

        public Task UpdateAsync(aspnet_Applications app)
        {
            Iaspnet_ApplicationsRepository appRepo = pcontext.Get<Iaspnet_ApplicationsRepository>();
            var existapp = appRepo.Get(app.ApplicationId);
            existapp = appRepo.CopyTo<aspnet_Applications>(app);
            return appRepo.UnitOfWork.CommitAsync();
        }

        public Task<string> GetApplicationNameFromConfiguratinFileAsync()
        {
            string appName = ConfigHelper.GetConfig(ApplicationName);
            return Task.FromResult(appName);
        }

        Task<IEnumerable<aspnet_Applications>> IApplicationStore<aspnet_Applications, Guid>.FindByIdAsync(Guid appId)
        {
            Iaspnet_ApplicationsRepository appRepo = pcontext.Get<Iaspnet_ApplicationsRepository>();
            return Task.FromResult(appRepo.FindById(appId));
        }

        Task<IEnumerable<aspnet_Applications>> IApplicationStore<aspnet_Applications, Guid>.FindByNameAsync(string appName)
        {
            Iaspnet_ApplicationsRepository appRepo = pcontext.Get<Iaspnet_ApplicationsRepository>();
            return Task.FromResult(appRepo.FindByName(appName));
        }

        public Task<aspnet_Applications> GetByIdAsync(Guid appId)
        {
            Iaspnet_ApplicationsRepository appRepo = pcontext.Get<Iaspnet_ApplicationsRepository>();
            return appRepo.GetAsync(appId);
        }

        public Task<aspnet_Applications> GetByNameAsync(string appName)
        {
            return Task.FromResult((from app in Applications
                                    where app.ApplicationName == appName ||
                                    app.LoweredApplicationName == appName
                                    select app).ToList().SingleOrDefault());
        }

        public async Task<aspnet_Applications> GetCurrentApplicationInfoAsync()
        {
            string appName = await GetApplicationNameFromConfiguratinFileAsync();
            return await GetByNameAsync(appName);
        }

        public const string ApplicationInfoKey = "ApplicationInfo";
        internal const string ApplicationName = "ApplicationName";
    }
}