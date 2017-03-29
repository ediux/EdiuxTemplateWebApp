using EdiuxTemplateWebApp.Models.AspNetModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace EdiuxTemplateWebApp.Models
{
    public class EdiuxAspNetSqlUserStore : IUserStore<aspnet_Users, Guid>
        , IUserRoleStore<aspnet_Users, Guid>, IRoleStore<aspnet_Roles, Guid>
        , IUserEmailStore<aspnet_Users, Guid>, IUserLockoutStore<aspnet_Users, Guid>
        , IUserLoginStore<aspnet_Users, Guid>, IUserPasswordStore<aspnet_Users, Guid>
        , IUserPhoneNumberStore<aspnet_Users, Guid>, IUserSecurityStampStore<aspnet_Users, Guid>
        , IUserTwoFactorStore<aspnet_Users, Guid>, IUserClaimStore<aspnet_Users, Guid>
        , IQueryableUserStore<aspnet_Users, Guid>, IQueryableRoleStore<aspnet_Roles, Guid>
    {
        #region 變數宣告區
        protected IUnitOfWork UnitOfWork;
        private Iaspnet_ApplicationsRepository appRepo;
        private Iaspnet_UsersRepository userRepo;
        private Iaspnet_RolesRepository roleRepo;
        private Iaspnet_UserLoginRepository userloginRepo;
        private Iaspnet_MembershipRepository membershipRepo;
        private aspnet_Applications applicationInfo;

        public EdiuxAspNetSqlUserStore(IUnitOfWork dbUnitOfWork)
        {
            UnitOfWork = dbUnitOfWork;

            appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository(UnitOfWork);

            applicationInfo = this.getApplicationInfo();

            if (applicationInfo == null)
            {
                throw new Exception("Application isn't existed.");
            }
            userRepo = RepositoryHelper.Getaspnet_UsersRepository(UnitOfWork);
            membershipRepo = RepositoryHelper.Getaspnet_MembershipRepository(UnitOfWork);
            roleRepo = RepositoryHelper.Getaspnet_RolesRepository(UnitOfWork);
            userloginRepo = RepositoryHelper.Getaspnet_UserLoginRepository(UnitOfWork);
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


        #endregion

        #region User Store(使用者帳號的CRUD)
        public Task CreateAsync(aspnet_Users user)
        {
            try
            {

                var appInfos = appRepo.FindByName(user.aspnet_Applications.ApplicationName);

                aspnet_Applications appInfo = null;

                if (!appInfos.Any())
                {
                    appInfo = appRepo.Add(user.aspnet_Applications);
                    UnitOfWork.Commit();
                    appInfo = appRepo.Reload(appInfo);
                    user.ApplicationId = appInfo.ApplicationId;
                    user.aspnet_Applications = appInfo;
                }
                else
                {
                    appInfo = appInfos.Single();
                    user.ApplicationId = appInfo.ApplicationId;
                    user.aspnet_Applications = appInfo;
                }

                var users = userRepo.GetUserByName(user.aspnet_Applications.ApplicationName,
                                                   user.UserName,
                                                   DateTime.UtcNow,
                                                   true);

                if (users == null)
                {
                    aspnet_Users newUser = userRepo.Add(user);
                    UnitOfWork.TranscationMode = false;
                    UnitOfWork.Commit();
                    user = newUser;

                }

                if (user?.aspnet_Membership == null)
                {
                    user.aspnet_Membership = membershipRepo.Add(user.aspnet_Membership);
                    UnitOfWork.TranscationMode = false;
                    UnitOfWork.Commit();
                    user.aspnet_Membership = membershipRepo.Reload(user.aspnet_Membership);
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
        }

        public Task DeleteAsync(aspnet_Users user)
        {
            try
            {
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
                roleRepo.Add(role);
                UnitOfWork.Commit();
                return Task.CompletedTask;
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
                roleRepo.Delete(role);
                UnitOfWork.Commit();
                return Task.CompletedTask;
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

                var membership = membershipRepo.Get(user.aspnet_Membership.UserId);



                UnitOfWork.TranscationMode = true;

                SetLockoutEnabledAsync(user, false);
                SetLockoutEndDateAsync(user, new DateTimeOffset(DateTime.Now));

                UnitOfWork.TranscationMode = false;
                membership.AccessFailedCount = 0;
                membershipRepo.UnitOfWork.Commit();

                user.aspnet_Membership = membershipRepo.Reload(membership);

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
                if (user.aspnet_Membership == null)
                {
                    throw new Exception(string.Format("The user '{0}' has missed membership information! ", user.UserName));
                }

                var membership = membershipRepo.Get(user.aspnet_Membership.UserId);

                aspnet_Membership_SetPassword_InputParameter paramObject = new aspnet_Membership_SetPassword_InputParameter();

                paramObject.applicationName = applicationInfo.ApplicationName;
                paramObject.currentTimeUtc = DateTime.UtcNow;
                paramObject.newPassword = passwordHash;
                paramObject.passwordFormat = (int)MembershipPasswordFormat.Hashed;
                paramObject.passwordSalt = user.aspnet_Membership.PasswordSalt;
                paramObject.userName = user.UserName;

                UnitOfWork.GetTypedContext<AspNetDbEntities2>().aspnet_Membership_SetPassword(paramObject);

                if (paramObject.ReturnValue != 0)
                {
                    return Task.FromException(new Exception(string.Format("Has Error.(ErrorCode:{0})", paramObject.ReturnValue)));
                }
                user = userRepo.Reload(user);
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

                var membership = membershipRepo.Get(user.aspnet_Membership.UserId);

                if (phoneNumber != membership.PhoneNumber)
                {
                    UnitOfWork.TranscationMode = true;
                    membership.PhoneNumber = phoneNumber;
                    if (GetTwoFactorEnabledAsync(user).Result)
                    {
                        SetPhoneNumberConfirmedAsync(user, false);
                    }
                    else
                    {
                        SetPhoneNumberConfirmedAsync(user, true);
                    }
                    UnitOfWork.TranscationMode = false;
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
                //if (user.aspnet_PersonalizationPerUser == null)
                //{
                //    throw new Exception(string.Format("The user '{0}' has missed page setting information! ", user.UserName));
                //}

                //var membership = membershipRepo.Get(user.aspnet_Membership.UserId);
                
                //user.SetProfile<ProfileModel>((s) => s.SecurityStamp = stamp);
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
                //if (user.aspnet_Membership == null)
                //{
                //    throw new Exception(string.Format("The user '{0}' has missed membership information! ", user.UserName));
                //}

                //var membership = membershipRepo.Get(user.aspnet_Membership.UserId);

                //ProfileModel profile = user.GetProfile<ProfileModel>();

                //if (profile == null)
                //    return Task.FromResult<string>("");

                //return Task.FromResult(profile.SecurityStamp);
                return Task.FromResult(string.Empty);
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

                var membership = membershipRepo.Get(user.aspnet_Membership.UserId);

                ProfileModel profile = user.SetProfile<ProfileModel>((s) => s.TwoFactorEnabled = enabled);
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

                var membership = membershipRepo.Get(user.aspnet_Membership.UserId);

                ProfileModel profile = user.GetProfile<ProfileModel>();

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


    }
}