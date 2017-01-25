using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Security.Claims;
using EdiuxTemplateWebApp.Models.AspNetModels;
using System.Runtime.Caching;

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
        // private Iaspnet_ApplicationsRepository appRepo;
        //private Iaspnet_UsersRepository userRepo;
        //private Iaspnet_RolesRepository roleRepo;
        //private Iaspnet_UserLoginRepository userloginRepo;
        private aspnet_Applications applicationInfo;

        public EdiuxAspNetSqlUserStore(AspNetModels.IUnitOfWork dbUnitOfWork)
        {

            applicationInfo = this.getApplicationInfo();

            if (applicationInfo == null)
            {
                applicationInfo = aspnet_Applications.Create(Startup.getApplicationNameFromConfiguationFile());
            }
            //userRepo = RepositoryHelper.Getaspnet_UsersRepository(dbUnitOfWork);
            //roleRepo = RepositoryHelper.Getaspnet_RolesRepository(dbUnitOfWork);
            //userloginRepo = RepositoryHelper.Getaspnet_UserLoginRepository(dbUnitOfWork);
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
                applicationInfo.AddUser(user.UserName, user.aspnet_Membership.Password);
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
                applicationInfo.DeleteUser(user);
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
                return Task.FromResult(applicationInfo.FindUserById(userId));
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
                return Task.FromResult(applicationInfo.FindUserByName(userName));
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
                user.Update();
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
                user.AddToRole(roleName);
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
                return Task.FromResult(user.GetRoles());
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
                return Task.FromResult(user.IsInRole(roleName));
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
                user.RemoveFromRole(roleName);
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
                applicationInfo.AddRole(role.Name, role.Description);
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
                role.Update();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return Task.FromException(ex);
            }
        }

        public Task DeleteAsync(aspnet_Roles role)
        {
            try
            {

                applicationInfo.aspnet_Roles.DeleteRole(role);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return Task.FromException(ex);
            }
        }

        Task<aspnet_Roles> IRoleStore<aspnet_Roles, Guid>.FindByIdAsync(Guid roleId)
        {
            try
            {
                return Task.FromResult(applicationInfo.GetRoleById(roleId));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return Task.FromException<aspnet_Roles>(ex);
            }
        }

        Task<aspnet_Roles> IRoleStore<aspnet_Roles, Guid>.FindByNameAsync(string roleName)
        {
            try
            {

                return Task.FromResult(applicationInfo.GetRoleByName(roleName));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return Task.FromException<aspnet_Roles>(ex);
            }
        }
        #endregion

        #region Email Store
        public Task SetEmailAsync(aspnet_Users user, string email)
        {
            try
            {
                user.aspnet_Membership.Email = email;
                aspnet_Membership.MembershipRepository.UnitOfWork.Context.Entry(user.aspnet_Membership).State = System.Data.Entity.EntityState.Modified;
                return aspnet_Membership.MembershipRepository.UnitOfWork.CommitAsync();
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
                return Task.FromResult(user.aspnet_Membership.Email);
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
                return Task.FromResult(user.aspnet_Profile.GetProfile<ProfileModel>().eMailConfirmed);
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
                user.aspnet_Profile.GetProfile<ProfileModel>().eMailConfirmed = confirmed;
                user.Update();
                return Task.CompletedTask;
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
                return Task.FromResult(applicationInfo.FindUserByEmail(email));
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

                user.Update();

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

                user.Update();

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
                user.aspnet_Membership.IsLockedOut = enabled;

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

                user.AddLogin(login);
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
                user.RemoveLogin(login);
                // userloginRepo.RemoveLoginAsync(user, login);
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
                aspnet_Users foundUser = applicationInfo.FindUserByLogin(login);
                return Task.FromResult(foundUser);
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

                user.aspnet_Membership.Password = passwordHash;
                user.aspnet_Membership.LastPasswordChangedDate = DateTime.UtcNow;

                user.Update();
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
                string getPasswordHash = user.aspnet_Membership.Password;
                return Task.FromResult(getPasswordHash);
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
                bool hasPasswordTask = false;

                if (user.aspnet_Membership == null)
                    return Task.FromResult(hasPasswordTask);

                if (user.aspnet_Membership.Password.Length == 0)
                    return Task.FromResult(hasPasswordTask);

                hasPasswordTask = true;

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
                ProfileModel profile = user.aspnet_Profile.GetProfile<ProfileModel>();

                if (profile == null)
                    return Task.FromException(new Exception("Profile is not existed."));

                profile.PhoneNumber = phoneNumber;


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
                ProfileModel profile = user.aspnet_Profile.GetProfile<ProfileModel>();

                if (profile == null)
                    return Task.FromException<string>(new Exception("Profile is not existed."));

                return Task.FromResult(profile.PhoneNumber);
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
                ProfileModel profile = user.aspnet_Profile.GetProfile<ProfileModel>();

                if (profile == null)
                    return Task.FromException<bool>(new Exception("Profile is not existed."));

                return Task.FromResult(profile.PhoneConfirmed);
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
                ProfileModel profile = user.aspnet_Profile.GetProfile<ProfileModel>();

                if (profile == null)
                    return Task.FromException(new Exception("Profile is not existed."));

                profile.PhoneConfirmed = confirmed;

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
                ProfileModel profile = user.aspnet_Profile.GetProfile<ProfileModel>();

                if (profile == null)
                    return Task.FromException(new Exception("Profile is not existed."));

                profile.SecurityStamp = stamp;

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
                ProfileModel profile = user.aspnet_Profile.GetProfile<ProfileModel>();

                if (profile == null)
                    return Task.FromException<string>(new Exception("Profile is not existed."));

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
                ProfileModel profile = user.aspnet_Profile.GetProfile<ProfileModel>();

                if (profile == null)
                    return Task.FromException(new Exception("Profile is not existed."));

                profile.TwoFactorEnabled = enabled;

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
                ProfileModel profile = user.aspnet_Profile.GetProfile<ProfileModel>();

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