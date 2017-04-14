﻿using EdiuxTemplateWebApp.Helpers;
using EdiuxTemplateWebApp.Models.AspNetModels;
using EdiuxTemplateWebApp.Models.Shared;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
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
        , IApplicationStore<aspnet_Applications, Guid>
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

            userRepo = RepositoryHelper.Getaspnet_UsersRepository(UnitOfWork);
            membershipRepo = RepositoryHelper.Getaspnet_MembershipRepository(UnitOfWork);
            roleRepo = RepositoryHelper.Getaspnet_RolesRepository(UnitOfWork);
            userloginRepo = RepositoryHelper.Getaspnet_UserLoginRepository(UnitOfWork);



            createApplicationIfNotExisted();



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
                throw new NotImplementedException();
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

            var asynctask = createApplicationIfNotExisted();
            asynctask.Start();
            asynctask.Wait();

            if (user.ApplicationId == null || user.ApplicationId == Guid.Empty)
            {
                user.ApplicationId = applicationInfo.ApplicationId;
                user.aspnet_Applications = applicationInfo;
            }

            var users = userRepo.GetUserByName(user.aspnet_Applications.ApplicationName,
                                               user.UserName,
                                               DateTime.UtcNow,
                                               true);

            aspnet_Users newUser = new aspnet_Users();

            if (users == null)
            {
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

                userRepo.Add(newUser);

                UnitOfWork.Commit();

                newUser = userRepo.Reload(newUser);
            }
            else
            {
                newUser = userRepo.GetUserByName(user.aspnet_Applications.ApplicationName, user.UserName, DateTime.UtcNow, true);
                return Task.FromResult(newUser);
            }

            if (newUser.aspnet_Membership == null)
            {
                if (user.aspnet_Membership != null)
                {
                    newUser.aspnet_Membership = membershipRepo.Add(user.aspnet_Membership);
                    newUser.aspnet_Membership.UserId = newUser.Id;
                }
                else
                {
                    newUser.aspnet_Membership = new aspnet_Membership();
                }

                userRepo.UnitOfWork.BatchCommitStart();
            }


            if (!newUser.aspnet_PersonalizationPerUser.Any())
            {
                //add

            }


            if (newUser.aspnet_Profile == null)
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



            }

            if (!newUser.aspnet_Roles.Any())
            {
                AddToRoleAsync(newUser, "Users");
            }

            if (!newUser.aspnet_UserClaims.Any())
            {

            }

            if (!newUser.aspnet_UserLogin.Any())
            {

            }

            UnitOfWork.Commit();

            return Task.CompletedTask;
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

            aspnet_Users rootUser = userRepo.GetUserByName(applicationInfo.ApplicationName, "root", DateTime.UtcNow, false);

            if (rootUser != null)
            {
                if (userRepo.IsInRole(rootUser, "Admins") == false)
                    userRepo.AddToRole(rootUser, "Admins");
                return;
            }
            throw new NullReferenceException(string.Format("The username , '{0}', is not found.", "root"));
        }

        private bool checkCurrentAppHasRootUser()
        {
            if (applicationInfo != null)
            {
                return applicationInfo.aspnet_Users.Any(s => s.UserName.Equals("root", StringComparison.InvariantCultureIgnoreCase));
            }

            return false;
        }

        private void createRootUser()
        {
            aspnet_Users rootUser = new aspnet_Users();  // ("root", "!QAZ2wsx");
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
            rootUser.aspnet_Membership.PasswordAnswer = "";
            rootUser.aspnet_Membership.PasswordFormat = (int)System.Web.Security.MembershipPasswordFormat.Hashed;
            rootUser.aspnet_Membership.PasswordQuestion = "";
            rootUser.aspnet_Membership.PasswordSalt = Path.GetRandomFileName();
            rootUser.aspnet_Membership.PhoneConfirmed = true;
            rootUser.aspnet_Membership.PhoneNumber = "0901-123-456";
            rootUser.aspnet_Membership.ResetPasswordToken = "";

            var asynctask = CreateAsync(rootUser);
            asynctask.ContinueWith((x) =>
            {
                var asynctaskb = AddToRoleAsync(rootUser, "Admins");
                asynctaskb.Start();
            });
        }

        private bool checkCurrentAppHasRoles()
        {
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
                    }
                };

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
        private aspnet_Applications getApplicationInformationFromCache(string appName)
        {
            var appInfo = appName.getApplicationGlobalVariable<aspnet_Applications>(ApplicationInfoKey);

            if (appInfo == null)
            {
                appInfo = appRepo.FindByName(ConfigHelper.GetConfig(ApplicationName)).SingleOrDefault();
                WebHelper.setApplicationGlobalVariable(appName, appName, appInfo);
            }

            return appInfo;
        }

        private void createDefaultRoles()
        {
            string appName = GetApplicationNameFromConfiguratinFileAsync().Result;

            aspnet_Applications appInfo = getApplicationInformationFromCache(appName);

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
                    }
                };

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

                string applicationName = ConfigHelper.GetConfig(getAppNameTask.Result);

                newApplication.ApplicationName = applicationName;
                newApplication.Description = applicationName;
                newApplication.LoweredApplicationName = applicationName.ToLowerInvariant();

                CreateAsync(newApplication);

                applicationInfo = newApplication;

                setToMemoryCache();

            }
            else
            {
                applicationInfo = this.getApplicationGlobalVariable<aspnet_Applications>(ApplicationInfoKey);

                if (applicationInfo == null)
                {
                    applicationInfo = appRepo.FindByName(GetApplicationNameFromConfiguratinFileAsync().Result).Single();
                    setToMemoryCache();
                }
                else
                {
                    applicationInfo = this.getApplicationInformationFromCache(GetApplicationNameFromConfiguratinFileAsync().Result);
                }
                
                
            }

            if (checkCurrentAppHasRoles() == false)
            {
                createDefaultRoles();
            }

            if (checkCurrentAppHasRootUser() == false)
            {
                createRootUser();
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

            appRepo.Add(newApplication);
            appRepo.UnitOfWork.Commit();

            app = appRepo.Reload(app);


            return Task.CompletedTask;
        }

        public Task DeleteAsync(aspnet_Applications app)
        {
            appRepo.Delete(app);
            appRepo.UnitOfWork.Commit();
            return Task.CompletedTask;
        }

        Task<aspnet_Applications> IApplicationStore<aspnet_Applications, Guid>.FindByIdAsync(Guid appId)
        {
            return Task.FromResult(appRepo.Get(appId));
        }

        Task<aspnet_Applications> IApplicationStore<aspnet_Applications, Guid>.FindByNameAsync(string appName)
        {
            return Task.FromResult(appRepo.FindByName(appName).SingleOrDefault());
        }

        public Task UpdateAsync(aspnet_Applications app)
        {
            var existapp = appRepo.Get(app.ApplicationId);
            existapp = appRepo.CopyTo<aspnet_Applications>(app);
            return appRepo.UnitOfWork.CommitAsync();
        }

        public Task<string> GetApplicationNameFromConfiguratinFileAsync()
        {
            string appName = ConfigHelper.GetConfig(ApplicationName);
            return Task.FromResult(appName);
        }

        public const string ApplicationInfoKey = "ApplicationInfo";
        internal const string ApplicationName = "ApplicationName";
    }
}