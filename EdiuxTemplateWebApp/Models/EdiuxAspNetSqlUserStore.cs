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
        protected IUnitOfWork<AspNetDbEntities2> UnitOfWork;
        private Iaspnet_ApplicationsRepository appRepo;
        private Iaspnet_UsersRepository userRepo;
        private Iaspnet_RolesRepository roleRepo;
        private Iaspnet_UserLoginRepository userloginRepo;
        private aspnet_Applications applicationInfo;

        public EdiuxAspNetSqlUserStore(AspNetModels.IUnitOfWork dbUnitOfWork)
        {
            UnitOfWork = dbUnitOfWork as IUnitOfWork<AspNetDbEntities2>;

            appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository(UnitOfWork);

            applicationInfo = appRepo.FindByName(Startup.getApplicationNameFromConfiguationFile()).SingleOrDefault();

            if (applicationInfo == null)
            {
                throw new Exception("Application isn't existed.");
            }
            userRepo = RepositoryHelper.Getaspnet_UsersRepository(UnitOfWork);
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
                aspnet_Membership_CreateUser_InputParameter paramObject = new aspnet_Membership_CreateUser_InputParameter();
                paramObject.ApplicationName = applicationInfo.ApplicationName;

                paramObject.CurrentTimeUtc = DateTime.UtcNow;
                paramObject.CreateDate = paramObject.CurrentTimeUtc.Date;
                paramObject.Email = user.aspnet_Membership.Email;
                paramObject.IsApproved = user.aspnet_Membership.IsApproved;
                paramObject.Password = user.aspnet_Membership.Password;
                paramObject.PasswordAnswer = user.aspnet_Membership.PasswordAnswer;
                paramObject.PasswordFormat = user.aspnet_Membership.PasswordFormat;
                paramObject.PasswordQuestion = user.aspnet_Membership.PasswordQuestion;
                paramObject.PasswordSalt = user.aspnet_Membership.PasswordSalt;
                paramObject.UniqueEmail = Membership.Provider.RequiresUniqueEmail ? 1 : 0;
                paramObject.UserName = user.UserName;

                UnitOfWork.Repositories.aspnet_Membership_CreateUser(paramObject);

                if (paramObject.ReturnValue == 0)
                {
                    if (paramObject.OutputParameter.CreateStatus == (int)MembershipCreateStatus.Success)
                    {
                        UnitOfWork.Context.Set<aspnet_Users>().Attach(userRepo.Get(paramObject.OutputParameter.UserId));
                    }
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
                aspnet_Users_DeleteUser_InputParameter paramObject = new aspnet_Users_DeleteUser_InputParameter();
                paramObject.ApplicationName = applicationInfo.ApplicationName;
                paramObject.TablesToDeleteFrom = (int)(TablesToCheck.aspnet_Membership | TablesToCheck.aspnet_PersonalizationPerUser | TablesToCheck.aspnet_Profile | TablesToCheck.aspnet_Roles);
                paramObject.UserName = user.UserName;
                UnitOfWork.Repositories.aspnet_Users_DeleteUser(paramObject);
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
                aspnet_Users _user = userRepo.Get(userId);
                return Task.FromResult(_user);
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
                aspnet_Membership_GetUserByName_InputParameter paramObject = new aspnet_Membership_GetUserByName_InputParameter();

                paramObject.ApplicationName = applicationInfo.ApplicationName;
                paramObject.CurrentTimeUtc = DateTime.UtcNow;
                paramObject.UpdateLastActivity = true;
                paramObject.UserName = userName;

                var singleUser = UnitOfWork.Repositories.aspnet_Membership_GetUserByName(paramObject);

                aspnet_Users _user = null;

                _user = userRepo.ConvertFrom(singleUser.Single());

                _user.ApplicationId = applicationInfo.ApplicationId;
                //_user.Id = singleUser.UserId;
                //_user.LastActivityDate = singleUser.LastActivityDate.HasValue ? singleUser.LastActivityDate.Value : default(DateTime);
                //_user.LoweredUserName = singleUser.UserName.ToLowerInvariant();
                //_user.UserName = singleUser.UserName;
                //_user.aspnet_Membership = new aspnet_Membership();
                //_user.aspnet_Membership.ApplicationId = _user.ApplicationId;
                //_user.aspnet_Membership.Comment = singleUser.Comment;
                //_user.aspnet_Membership.CreateDate = singleUser.CreateDate;
                //_user.aspnet_Membership.Email = singleUser.Email;
                //_user.aspnet_Membership.IsApproved = singleUser.IsApproved;
                //_user.aspnet_Membership.LastActivityTime = singleUser.LastActivityDate; ;
                //_user.aspnet_Membership.LastLockoutDate = singleUser.LastLockoutDate;
                //_user.aspnet_Membership.LastLoginDate = singleUser.LastLoginDate;
                //_user.aspnet_Membership.LastPasswordChangedDate = singleUser.LastPasswordChangedDate;
                //_user.aspnet_Membership.PasswordQuestion = singleUser.PasswordQuestion;

                userRepo.Attach(_user); //加入變更追蹤

                return Task.FromResult(_user);
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
                aspnet_Membership_UpdateUser_InputParameter paramObject = new aspnet_Membership_UpdateUser_InputParameter();
                paramObject.applicationName = user.aspnet_Applications.ApplicationName;
                paramObject.comment = user.aspnet_Membership.Comment;
                paramObject.currentTimeUtc = DateTime.UtcNow;
                paramObject.email = user.aspnet_Membership.Email;
                paramObject.isApproved = user.aspnet_Membership.IsApproved;
                paramObject.lastActivityDate = DateTime.Now.Date;
                paramObject.lastLoginDate = user.aspnet_Membership.LastLoginDate;
                paramObject.uniqueEmail = Membership.Provider.RequiresUniqueEmail ? 1 : 0;

                UnitOfWork.Repositories.aspnet_Membership_UpdateUser(paramObject);

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
                aspnet_UsersInRoles_AddUsersToRoles_InputParameter paramObject = new aspnet_UsersInRoles_AddUsersToRoles_InputParameter();

                paramObject.applicationName = applicationInfo.ApplicationName;
                paramObject.currentTimeUtc = DateTime.UtcNow;
                paramObject.roleNames = roleName;
                paramObject.userNames = user.UserName;
                UnitOfWork.Repositories.aspnet_UsersInRoles_AddUsersToRoles(paramObject);

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
                aspnet_UsersInRoles_GetRolesForUser_InputParameter paramObject = new aspnet_UsersInRoles_GetRolesForUser_InputParameter();

                paramObject.applicationName = applicationInfo.ApplicationName;
                paramObject.userName = user.UserName;

                var result = UnitOfWork.Repositories.aspnet_UsersInRoles_GetRolesForUser(paramObject);

                if (paramObject.ReturnValue == 0)
                {
                    return Task.FromResult(result.Select(s => s.RoleName).ToList() as IList<string>);
                }

                return Task.FromResult(new List<string>() as IList<string>);
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
                aspnet_UsersInRoles_IsUserInRole_InputParameter paramObject = new aspnet_UsersInRoles_IsUserInRole_InputParameter();

                paramObject.applicationName = applicationInfo.ApplicationName;
                paramObject.roleName = roleName;
                paramObject.userName = user.UserName;

                if (paramObject.ReturnValue == 1)
                {
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
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
                aspnet_UsersInRoles_RemoveUsersFromRoles_InputParameter paramObject = new aspnet_UsersInRoles_RemoveUsersFromRoles_InputParameter();

                paramObject.applicationName = applicationInfo.ApplicationName;
                paramObject.roleNames = roleName;
                paramObject.userNames = user.UserName;

                UnitOfWork.Repositories.aspnet_UsersInRoles_RemoveUsersFromRoles(paramObject);

                if (paramObject.ReturnValue != 0)
                {
                    Task.FromException(new Exception(string.Format("發生錯誤!(錯誤碼:{0})", paramObject.ReturnValue)));
                }

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
                aspnet_Roles_CreateRole_InputParameter paramObject = new aspnet_Roles_CreateRole_InputParameter();

                paramObject.applicationName = applicationInfo.ApplicationName;
                paramObject.roleName = role.Name;

                UnitOfWork.Repositories.aspnet_Roles_CreateRole(paramObject);
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
                aspnet_Roles existedRole = roleRepo.Get(role.Id);

                existedRole.LoweredRoleName = role.LoweredRoleName;
                existedRole.Name = role.Name;
                existedRole.ApplicationId = role.ApplicationId;
                existedRole.Description = role.Description;
                existedRole.aspnet_Users = role.aspnet_Users;
                existedRole.Menus = role.Menus;

                roleRepo.UnitOfWork.Commit();

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
                aspnet_Roles_DeleteRole_InputParameter paramObject = new aspnet_Roles_DeleteRole_InputParameter();

                paramObject.applicationName = applicationInfo.ApplicationName;
                paramObject.deleteOnlyIfRoleIsEmpty = false;
                paramObject.roleName = role.Name;

                UnitOfWork.Repositories.aspnet_Roles_DeleteRole(paramObject);

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
                return roleRepo.FindByNameAsync(roleName);
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

                Iaspnet_MembershipRepository membershipRepo = userRepo.DependcyRepository["aspnet_MembershipRepository"] as Iaspnet_MembershipRepository;

                aspnet_Membership membership = membershipRepo.Get(user.Id);

                membership.Email = email;
                membership.LoweredEmail = email.ToLowerInvariant();

                membershipRepo.UnitOfWork.Context.Entry(user.aspnet_Membership).State = System.Data.Entity.EntityState.Modified;

                return membershipRepo.UnitOfWork.CommitAsync();
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
                Iaspnet_MembershipRepository membershipRepo = userRepo.DependcyRepository["aspnet_MembershipRepository"] as Iaspnet_MembershipRepository;
                aspnet_Membership membership = membershipRepo.Get(user.Id);
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
                Iaspnet_MembershipRepository membershipRepo = userRepo.DependcyRepository["aspnet_MembershipRepository"] as Iaspnet_MembershipRepository;
                aspnet_Membership membership = membershipRepo.Get(user.Id);
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

                Iaspnet_MembershipRepository membershipRepo = userRepo.DependcyRepository["aspnet_MembershipRepository"] as Iaspnet_MembershipRepository;
                aspnet_Membership membership = membershipRepo.Get(user.Id);
                if (membership != null)
                {
                    membership.EmailConfirmed = confirmed;
                    membershipRepo.UnitOfWork.Context.Entry(user.aspnet_Membership).State = System.Data.Entity.EntityState.Modified;

                    return membershipRepo.UnitOfWork.CommitAsync();
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
                aspnet_Membership_GetUserByEmail_InputParameter paramObject = new aspnet_Membership_GetUserByEmail_InputParameter();
                paramObject.applicationName = applicationInfo.ApplicationName;
                paramObject.email = email;
                var result = UnitOfWork.Repositories.aspnet_Membership_GetUserByEmail(paramObject);
                if (paramObject.ReturnValue == 0)
                {
                    return Task.FromResult(userRepo.ConvertFrom(result.SingleOrDefault()));
                }
                return Task.FromException<aspnet_Users>(new Exception("user not found."));
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
                return Task.FromResult(new DateTimeOffset(user.aspnet_Membership.LastLockoutDate));
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
                user.aspnet_Membership.LastLockoutDate = new DateTime(lockoutEnd.Ticks);
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
                user.aspnet_Membership.FailedPasswordAttemptCount += 1;
                user.aspnet_Membership.FailedPasswordAttemptWindowStart = DateTime.UtcNow;

                userRepo.Attach(user);
                userRepo.UnitOfWork.Context.Entry(user).State = System.Data.Entity.EntityState.Modified;
                userRepo.UnitOfWork.Commit();
                user = userRepo.Reload(user);
                return Task.FromResult(user.aspnet_Membership.FailedPasswordAttemptCount);
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
                user.aspnet_Membership.FailedPasswordAttemptCount = 0;
                user.aspnet_Membership.LastLockoutDate = DateTime.Now;
                user.aspnet_Membership.IsLockedOut = false;

                userRepo.Attach(user);
                userRepo.UnitOfWork.Context.Entry(user).State = System.Data.Entity.EntityState.Modified;
                userRepo.UnitOfWork.Commit();
                user = userRepo.Reload(user);

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
                return Task.FromResult(user.aspnet_Membership.FailedPasswordAttemptCount);
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
                return Task.FromResult(user.aspnet_Membership.IsLockedOut);
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
                aspnet_Users _user = userRepo.Get(user.Id);
                if (_user != null)
                {
                    _user.aspnet_Membership.IsLockedOut = enabled;
                    UnitOfWork.Context.Entry(_user).State = System.Data.Entity.EntityState.Modified;
                    UnitOfWork.Commit();
                }

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
                //Iaspnet_UserLoginRepository userLoginRepo = RepositoryHelper.Getaspnet_UserLoginRepository(applicationInfo.ApplicationRepository.UnitOfWork);
                // userloginRepo.AddLoginAsync(user, login);
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
                return Task.FromResult(user.GetLogins());
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
                aspnet_Membership_SetPassword_InputParameter paramObject = new aspnet_Membership_SetPassword_InputParameter();

                paramObject.applicationName = applicationInfo.ApplicationName;
                paramObject.currentTimeUtc = DateTime.UtcNow;
                paramObject.newPassword = passwordHash;
                paramObject.passwordFormat = (int)MembershipPasswordFormat.Hashed;
                paramObject.passwordSalt = user.aspnet_Membership.PasswordSalt;
                paramObject.userName = user.UserName;

                UnitOfWork.Repositories.aspnet_Membership_SetPassword(paramObject);

                if (paramObject.ReturnValue != 0)
                {
                    return Task.FromException(new Exception(string.Format("Has Error.(ErrorCode:{0})", paramObject.ReturnValue)));
                }
                //Task setSetPasswordTask = userRepo.SetPasswordHashAsync(user, passwordHash);
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
                aspnet_Membership_GetPasswordWithFormat_InputParameter paramObject = new aspnet_Membership_GetPasswordWithFormat_InputParameter();

                paramObject.applicationName = applicationInfo.ApplicationName;
                paramObject.currentTimeUtc = DateTime.UtcNow;
                paramObject.updateLastLoginActivityDate = true;
                paramObject.userName = user.UserName;

                var result = UnitOfWork.Repositories.aspnet_Membership_GetPasswordWithFormat(paramObject).FirstOrDefault();
                if (paramObject.ReturnValue != 0)
                {
                    return Task.FromException<string>(new Exception(string.Format("Has Error.(ErrorCode:{0})", paramObject.ReturnValue)));
                }
                if (paramObject.OutputParameter.ReturnCode != 0)
                {
                    return Task.FromException<string>(new Exception(string.Format("Has Error.(ErrorCode:{0})", paramObject.OutputParameter.ReturnCode)));
                }
                if (result != null && result.PasswordFormat == (int)MembershipPasswordFormat.Hashed)
                {
                    return Task.FromResult(result.Password);
                }

                return Task.FromResult(string.Empty);
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
                bool hasPasswordTask = (user != null) &&
                    (user?.aspnet_Membership != null) &&
                    !string.IsNullOrEmpty(user.aspnet_Membership.Password);

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
                user.aspnet_Membership.PhoneNumber = phoneNumber;
                userRepo.Attach(user);

                UnitOfWork.Repositories.Entry(user).State = EntityState.Modified;
                UnitOfWork.Commit();
                user = userRepo.Reload(user);
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
                if (user.aspnet_Membership != null)
                {
                    return Task.FromResult(user.aspnet_Membership.PhoneNumber);
                }

                return Task.FromException<string>(new Exception(string.Format("The membership of '{0}' is missing.", user.UserName)));

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
                if (user.aspnet_Membership != null)
                {
                    return Task.FromResult(user.aspnet_Membership.PhoneConfirmed);
                }

                return Task.FromException<bool>(new Exception(string.Format("The membership of '{0}' is missing.", user.UserName)));
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
                if (user.aspnet_Membership != null)
                {
                    userRepo.Attach(user);
                    user.aspnet_Membership.PhoneConfirmed = confirmed;

                    userRepo.UnitOfWork.Context.Entry(user).State = EntityState.Modified;
                    user = userRepo.Reload(user);

                    return userRepo.UnitOfWork.CommitAsync();
                }

                return Task.FromException<bool>(new Exception(string.Format("The membership of '{0}' is missing.", user.UserName)));
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
                user.SetProfile<ProfileModel>((s) => s.SecurityStamp = stamp);
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
                ProfileModel profile = user.GetProfile<ProfileModel>();

                if (profile == null)
                    return Task.FromResult<string>("");

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
                ProfileModel profile  = user.SetProfile<ProfileModel>((s) => s.TwoFactorEnabled = enabled);
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