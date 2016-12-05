using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Security.Claims;

namespace EdiuxTemplateWebApp.Models
{
    public class EdiuxAspNetSqlUserStore : IUserStore<ApplicationUser, int>
        , IUserRoleStore<ApplicationUser, int>, IRoleStore<ApplicationRole, int>
        , IUserEmailStore<ApplicationUser, int>, IUserLockoutStore<ApplicationUser, int>
        , IUserLoginStore<ApplicationUser, int>, IUserPasswordStore<ApplicationUser, int>
        , IUserPhoneNumberStore<ApplicationUser, int>, IUserSecurityStampStore<ApplicationUser, int>
        , IUserTwoFactorStore<ApplicationUser, int>, IUserClaimStore<ApplicationUser, int>
        , IQueryableUserStore<ApplicationUser, int>, IQueryableRoleStore<ApplicationRole, int>
    {
        #region 變數宣告區
        private IApplicationUserRepository userRepo;
        private IApplicationRoleRepository roleRepo;
        private IApplicationUserLoginRepository userloginRepo;
        #endregion

        public IUnitOfWork UnitOfWork
        {
            get { return userRepo.UnitOfWork; }
        }
        #region 建構式
        public EdiuxAspNetSqlUserStore(IUnitOfWork dbUnitOfWork)
        {
            userRepo = RepositoryHelper.GetApplicationUserRepository(dbUnitOfWork);
            roleRepo = RepositoryHelper.GetApplicationRoleRepository(dbUnitOfWork);
            userloginRepo = RepositoryHelper.GetApplicationUserLoginRepository(dbUnitOfWork);
        }
        #endregion

        #region Queryable User Store
        public IQueryable<ApplicationUser> Users
        {
            get
            {
                return userRepo.All();
            }
        }
        #endregion

        #region Queryable Role Store
        public IQueryable<ApplicationRole> Roles
        {
            get
            {
                return roleRepo.All();
            }
        }
        #endregion

        #region User Store(使用者帳號的CRUD)
        public Task CreateAsync(ApplicationUser user)
        {
            try
            {
                return userRepo.CreateAsync(user);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            try
            {
                return userRepo.DeleteAsync(user);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
        }

        public Task<ApplicationUser> FindByIdAsync(int userId)
        {
            try
            {
                return userRepo.FindByIdAsync(userId);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            try
            {
                return userRepo.FindByNameAsync(userName);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
        }



        public Task UpdateAsync(ApplicationUser user)
        {
            try
            {
                return userRepo.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
        }
        #endregion

        #region User Role Store
        public Task AddToRoleAsync(ApplicationUser user, string roleName)
        {
            try
            {
                return userRepo.AddToRoleAsync(user, roleName);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            try
            {
                return userRepo.GetRolesAsync(user);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
        }

        public Task<bool> IsInRoleAsync(ApplicationUser user, string roleName)
        {
            try
            {
                return userRepo.IsInRoleAsync(user, roleName);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
        }

        public Task RemoveFromRoleAsync(ApplicationUser user, string roleName)
        {
            try
            {
                return userRepo.RemoveFromRoleAsync(user, roleName);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
        }
        #endregion

        #region Role Store
        public Task CreateAsync(ApplicationRole role)
        {
            try
            {
                return roleRepo.CreateAsync(role);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                throw;
            }
        }

        public Task UpdateAsync(ApplicationRole role)
        {
            try
            {
                return roleRepo.UpdateAsync(role);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                throw;
            }
        }

        public Task DeleteAsync(ApplicationRole role)
        {
            try
            {
                return roleRepo.DeleteAsync(role);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                throw;
            }
        }

        Task<ApplicationRole> IRoleStore<ApplicationRole, int>.FindByIdAsync(int roleId)
        {
            try
            {
                return roleRepo.FindByIdAsync(roleId);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                throw;
            }
        }

        Task<ApplicationRole> IRoleStore<ApplicationRole, int>.FindByNameAsync(string roleName)
        {
            try
            {
                return roleRepo.FindByNameAsync(roleName);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                throw;
            }
        }
        #endregion

        #region Email Store
        public Task SetEmailAsync(ApplicationUser user, string email)
        {
            try
            {
                return userRepo.SetEmailAsync(user, email);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
        }

        public Task<string> GetEmailAsync(ApplicationUser user)
        {
            try
            {
                return userRepo.GetEmailAsync(user);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user)
        {
            try
            {
                return userRepo.GetEmailConfirmedAsync(user);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            try
            {
                return userRepo.SetEmailConfirmedAsync(user, confirmed);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task<ApplicationUser> FindByEmailAsync(string email)
        {
            try
            {
                return userRepo.FindByEmailAsync(email);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        #endregion

        #region User Lockout Store
        public async Task<DateTimeOffset> GetLockoutEndDateAsync(ApplicationUser user)
        {
            try
            {
                return await userRepo.GetLockoutEndDateAsync(user);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public async Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset lockoutEnd)
        {
            try
            {
                await userRepo.SetLockoutEndDateAsync(user, lockoutEnd);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public async Task<int> IncrementAccessFailedCountAsync(ApplicationUser user)
        {
            try
            {
                return await userRepo.IncrementAccessFailedCountAsync(user);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
           
        }

        public async Task ResetAccessFailedCountAsync(ApplicationUser user)
        {
            try
            {
                await userRepo.ResetAccessFailedCountAsync(user);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public async Task<int> GetAccessFailedCountAsync(ApplicationUser user)
        {
            try
            {
                return await userRepo.GetAccessFailedCountAsync(user);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public async Task<bool> GetLockoutEnabledAsync(ApplicationUser user)
        {
            try
            {
               return await userRepo.GetLockoutEnabledAsync(user);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public async Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled)
        {
            try
            {
                await userRepo.ResetAccessFailedCountAsync(user);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }
        #endregion

        #region User Login Store
        public Task AddLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            try
            {
                userloginRepo.AddLoginAsync(user, login).Wait();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task RemoveLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            try
            {
                userloginRepo.RemoveLoginAsync(user, login);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user)
        {
            try
            {
                Task<IList<UserLoginInfo>> getLoginTask = userloginRepo.GetLoginsAsync(user);
                getLoginTask.Wait();
                return getLoginTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task<ApplicationUser> FindAsync(UserLoginInfo login)
        {
            try
            {
                Task<ApplicationUser> getFindTask = userloginRepo.FindAsync(login);
                getFindTask.Wait();
                return getFindTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }
        #endregion

        #region User Password Store
        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            try
            {
                Task setSetPasswordTask = userRepo.SetPasswordHashAsync(user, passwordHash);
                setSetPasswordTask.Wait();
                return setSetPasswordTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            try
            {
                Task<string> getPasswordHashTask = userRepo.GetPasswordHashAsync(user);
                getPasswordHashTask.Wait();
                return getPasswordHashTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            try
            {
                Task<bool> hasPasswordTask = userRepo.HasPasswordAsync(user);
                hasPasswordTask.Wait();
                return hasPasswordTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }
        #endregion

        #region User Phone Number Store
        public Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber)
        {
            try
            {
                Task setPhoneNumberTask = userRepo.SetPhoneNumberAsync(user, phoneNumber);
                setPhoneNumberTask.Wait();
                return setPhoneNumberTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task<string> GetPhoneNumberAsync(ApplicationUser user)
        {
            try
            {
                Task<string> getPhoneNumberTask = userRepo.GetPhoneNumberAsync(user);
                getPhoneNumberTask.Wait();
                return getPhoneNumberTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user)
        {
            try
            {
                Task<bool> getPhoneNumberConfirmedTask = userRepo.GetPhoneNumberConfirmedAsync(user);
                getPhoneNumberConfirmedTask.Wait();
                return getPhoneNumberConfirmedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            try
            {
                Task setPhoneNumberConfirmed = userRepo.SetPhoneNumberConfirmedAsync(user, confirmed);
                setPhoneNumberConfirmed.Wait();
                return setPhoneNumberConfirmed;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }
        #endregion

        #region User Security Stamp Store
        public Task SetSecurityStampAsync(ApplicationUser user, string stamp)
        {
            try
            {
                Task setSecurityStampTask = userRepo.SetSecurityStampAsync(user, stamp);
                setSecurityStampTask.Wait();
                return setSecurityStampTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task<string> GetSecurityStampAsync(ApplicationUser user)
        {
            try
            {
                Task<string> getSecurityStampTask = userRepo.GetSecurityStampAsync(user);
                getSecurityStampTask.Wait();
                return getSecurityStampTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }
        #endregion

        #region User Two Factor Store
        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled)
        {
            try
            {
                Task setTwoFactorEnabledTask = userRepo.SetTwoFactorEnabledAsync(user, enabled);
                setTwoFactorEnabledTask.Wait();
                return setTwoFactorEnabledTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user)
        {
            try
            {
                Task<bool> getTwoFactorEnabledTask = userRepo.GetTwoFactorEnabledAsync(user);
                getTwoFactorEnabledTask.Wait();
                return getTwoFactorEnabledTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }
        #endregion

        #region User Claim Store
        public Task<IList<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            try
            {
                Task<IList<Claim>> getClaimsTask = userRepo.GetClaimsAsync(user);
                getClaimsTask.Wait();
                return getClaimsTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task AddClaimAsync(ApplicationUser user, Claim claim)
        {
            try
            {
                Task addClaimTask = userRepo.AddClaimAsync(user, claim);
                addClaimTask.Wait();
                return addClaimTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task RemoveClaimAsync(ApplicationUser user, Claim claim)
        {
            try
            {
                Task removeClaimTask = userRepo.RemoveClaimAsync(user, claim);
                removeClaimTask.Wait();
                return removeClaimTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
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