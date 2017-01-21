﻿using Microsoft.AspNet.Identity;
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
        #endregion

        public AspNetModels.IUnitOfWork UnitOfWork
        {
            get { return applicationInfo.ApplicationRepository.UnitOfWork; }
        }
        #region 建構式
        public EdiuxAspNetSqlUserStore(AspNetModels.IUnitOfWork dbUnitOfWork)
        {

            applicationInfo = getApplicationInformation();

            if (applicationInfo == null)
            {
                setApplicationInfo();
            }
            //userRepo = RepositoryHelper.Getaspnet_UsersRepository(dbUnitOfWork);
            //roleRepo = RepositoryHelper.Getaspnet_RolesRepository(dbUnitOfWork);
            //userloginRepo = RepositoryHelper.Getaspnet_UserLoginRepository(dbUnitOfWork);
        }

        private void setApplicationInfo()
        {
            string appName = Startup.getApplicationNameFromConfiguationFile();
            Iaspnet_ApplicationsRepository appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();
            applicationInfo = appRepo.FindByName(appName);
            applicationInfo.ApplicationRepository = appRepo;

            MemoryCache.Default.Set("ApplicationInfo", applicationInfo, DateTime.UtcNow.AddMinutes(38400));
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
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                aspnet_Applications appInfo = getApplicationInformation();

                appInfo.CreateUser(user);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
        }

        private aspnet_Applications getApplicationInformation()
        {
            aspnet_Applications fromCache = MemoryCache.Default.Get("ApplicatinInfo") as aspnet_Applications;
            return fromCache;
        }

        public Task DeleteAsync(aspnet_Users user)
        {
            try
            {
                aspnet_Applications appInfo = getApplicationInformation();
                return user.UserRepository.DeleteAsync(user);
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
                aspnet_Applications appInfo = getApplicationInformation();
                return Task.FromResult(appInfo.FindUserById(userId));
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
                aspnet_Applications appInfo = getApplicationInformation();
                return Task.FromResult(appInfo.FindUserByName(userName));
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
                return Task.FromResult(user.RemoveFromRole(roleName));
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
                aspnet_Applications appInfo = getApplicationInformation();
                appInfo.CreateRole(role);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return Task.FromException(ex);
            }
        }

        public Task UpdateAsync(aspnet_Roles role)
        {
            try
            {
                return role.Update();
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
                aspnet_Applications appInfo = getApplicationInformation();
                appInfo.DeleteRole(role);
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
                aspnet_Applications appInfo = getApplicationInformation();
                return Task.FromResult(appInfo.FindRoleById(roleId));
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
                aspnet_Applications appInfo = getApplicationInformation();
                return Task.FromResult(appInfo.FindRoleByName(roleName));
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
                user.SetEmail(email);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
        }

        public Task<string> GetEmailAsync(aspnet_Users user)
        {
            try
            {
                return Task.FromResult(user.GetEmail());
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task<bool> GetEmailConfirmedAsync(aspnet_Users user)
        {
            try
            {
                return Task.FromResult(user.GetEmailConfirmed());
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task SetEmailConfirmedAsync(aspnet_Users user, bool confirmed)
        {
            try
            {
                return user.SetEmailConfirmed(confirmed);
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
                return Task.FromResult(user.GetLockoutEndDate());
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
                user.SetLockoutEndDate(lockoutEnd);
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
                return Task.FromResult(user.IncrementAccessFailedCount());
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<int>(ex);
            }

        }

        public async Task ResetAccessFailedCountAsync(aspnet_Users user)
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

        public async Task<int> GetAccessFailedCountAsync(aspnet_Users user)
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

        public async Task<bool> GetLockoutEnabledAsync(aspnet_Users user)
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

        public async Task SetLockoutEnabledAsync(aspnet_Users user, bool enabled)
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
        public async Task AddLoginAsync(aspnet_Users user, UserLoginInfo login)
        {
            try
            {
                await userloginRepo.AddLoginAsync(user, login);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task RemoveLoginAsync(aspnet_Users user, UserLoginInfo login)
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

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(aspnet_Users user)
        {
            try
            {
                return await userloginRepo.GetLoginsAsync(user);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public async Task<aspnet_Users> FindAsync(UserLoginInfo login)
        {
            try
            {
                return await userloginRepo.FindAsync(login);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }
        #endregion

        #region User Password Store
        public Task SetPasswordHashAsync(aspnet_Users user, string passwordHash)
        {
            try
            {
                Task setSetPasswordTask = userRepo.SetPasswordHashAsync(user, passwordHash);
                return setSetPasswordTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task<string> GetPasswordHashAsync(aspnet_Users user)
        {
            try
            {
                Task<string> getPasswordHashTask = userRepo.GetPasswordHashAsync(user);
                return getPasswordHashTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task<bool> HasPasswordAsync(aspnet_Users user)
        {
            try
            {
                Task<bool> hasPasswordTask = userRepo.HasPasswordAsync(user);
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
        public Task SetPhoneNumberAsync(aspnet_Users user, string phoneNumber)
        {
            try
            {
                Task setPhoneNumberTask = userRepo.SetPhoneNumberAsync(user, phoneNumber);
                return setPhoneNumberTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task<string> GetPhoneNumberAsync(aspnet_Users user)
        {
            try
            {
                Task<string> getPhoneNumberTask = userRepo.GetPhoneNumberAsync(user);
                return getPhoneNumberTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(aspnet_Users user)
        {
            try
            {
                Task<bool> getPhoneNumberConfirmedTask = userRepo.GetPhoneNumberConfirmedAsync(user);
                return getPhoneNumberConfirmedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task SetPhoneNumberConfirmedAsync(aspnet_Users user, bool confirmed)
        {
            try
            {
                Task setPhoneNumberConfirmed = userRepo.SetPhoneNumberConfirmedAsync(user, confirmed);
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
        public Task SetSecurityStampAsync(aspnet_Users user, string stamp)
        {
            try
            {
                Task setSecurityStampTask = userRepo.SetSecurityStampAsync(user, stamp);
                return setSecurityStampTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task<string> GetSecurityStampAsync(aspnet_Users user)
        {
            try
            {
                Task<string> getSecurityStampTask = userRepo.GetSecurityStampAsync(user);
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
        public Task SetTwoFactorEnabledAsync(aspnet_Users user, bool enabled)
        {
            try
            {
                Task setTwoFactorEnabledTask = userRepo.SetTwoFactorEnabledAsync(user, enabled);
                return setTwoFactorEnabledTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task<bool> GetTwoFactorEnabledAsync(aspnet_Users user)
        {
            try
            {
                Task<bool> getTwoFactorEnabledTask = userRepo.GetTwoFactorEnabledAsync(user);
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
        public Task<IList<Claim>> GetClaimsAsync(aspnet_Users user)
        {
            try
            {
                Task<IList<Claim>> getClaimsTask = userRepo.GetClaimsAsync(user);
                return getClaimsTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task AddClaimAsync(aspnet_Users user, Claim claim)
        {
            try
            {
                Task addClaimTask = userRepo.AddClaimAsync(user, claim);
                return addClaimTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task RemoveClaimAsync(aspnet_Users user, Claim claim)
        {
            try
            {
                Task removeClaimTask = userRepo.RemoveClaimAsync(user, claim);
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