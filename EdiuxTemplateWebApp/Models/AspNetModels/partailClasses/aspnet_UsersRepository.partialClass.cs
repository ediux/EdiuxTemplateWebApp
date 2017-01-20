using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity.Core.Objects;
using System.Security.Claims;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_UsersRepository : EFRepository<aspnet_Users>, Iaspnet_UsersRepository
    {
        public Task<string> GetEmailAsync(aspnet_Users user)
        {
            return Task.FromResult(user.aspnet_Membership.GetEmail());
        }

        public Task SetEmailAsync(aspnet_Users user, string email)
        {
            user.aspnet_Membership.SetEmail(email);
            return Task.CompletedTask;
        }

        public Task AddToRoleAsync(aspnet_Users user, string roleName)
        {
            try
            {
                if (user.ApplicationId == Guid.Empty)
                    return Task.FromException(new ArgumentNullException(nameof(user)));

                if (string.IsNullOrEmpty(roleName) || string.IsNullOrWhiteSpace(roleName))
                    return Task.FromException(new ArgumentNullException(nameof(roleName)));

                string appName = user.aspnet_Applications.ApplicationName;
                string userName = user.UserName;

                return Task.FromResult(InternalDatabaseAlias.aspnet_UsersInRoles_AddUsersToRoles(appName, userName, roleName, DateTime.UtcNow));
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }
        }

        public Task CreateAsync(aspnet_Users user)
        {
            try
            {
                ObjectParameter userId = new ObjectParameter("UserId", typeof(Guid));

                InternalDatabaseAlias.aspnet_Users_CreateUser(user.ApplicationId,
                    user.UserName, false, DateTime.UtcNow, userId);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }
        }

        public Task DeleteAsync(aspnet_Users user)
        {
            try
            {
                ObjectParameter numTablesDeletedFrom = new ObjectParameter("NumTablesDeletedFrom", typeof(Guid));
                InternalDatabaseAlias.aspnet_Users_DeleteUser(user.aspnet_Applications.ApplicationName, user.UserName, 14, numTablesDeletedFrom);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }
        }
        
        public Task<aspnet_Users> FindByIdAsync(Guid userId)
        {
            //Remove
            try
            {
                return Task.FromResult(Get(userId));
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<aspnet_Users>(ex);
            }
        }

        public Task<aspnet_Users> FindByNameAsync(string userName)
        {
            //Remove
            try
            {
                return Task.FromResult(Where(s => s.UserName == userName).SingleOrDefault());
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<aspnet_Users>(ex);
            }

        }

        public Task<IList<string>> GetRolesAsync(aspnet_Users user)
        {
            //Remove
            try
            {
                return Task.FromResult((IList<string>)InternalDatabaseAlias.aspnet_UsersInRoles_GetRolesForUser(user.aspnet_Applications.ApplicationName, user.UserName).ToList());
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

                int returnCode = InternalDatabaseAlias.aspnet_UsersInRoles_IsUserInRole(user.aspnet_Applications.ApplicationName, user.UserName, roleName);

                if (returnCode == 2)
                    return Task.FromException<bool>(new NullReferenceException("The 'ApplicationId' or 'UserId' is null."));

                if (returnCode == 3)
                    return Task.FromException<bool>(new NullReferenceException(string.Format("The name of role, {0} is not found.", roleName)));

                if (returnCode == 1)
                    return Task.FromResult(true);
                else
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
                string appName = user.aspnet_Applications.ApplicationName;
                var executeResult = InternalDatabaseAlias.aspnet_UsersInRoles_RemoveUsersFromRoles(appName, user.UserName, roleName);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }
        }

        public Task UpdateAsync(aspnet_Users user)
        {
            try
            {
                aspnet_Users foundUser = Get(user.Id);

                if (foundUser == null)
                    return Task.FromException(new NullReferenceException(string.Format("The user '{0}' is not found.", user.UserName)));

                foundUser.ApplicationId = user.ApplicationId;
                foundUser.aspnet_Applications = user.aspnet_Applications;
                foundUser.aspnet_Membership = user.aspnet_Membership;
                foundUser.aspnet_PersonalizationPerUser = user.aspnet_PersonalizationPerUser;
                foundUser.aspnet_Profile = user.aspnet_Profile;
                foundUser.aspnet_Roles = user.aspnet_Roles;
                foundUser.aspnet_UserLogin = user.aspnet_UserLogin;
                foundUser.IsAnonymous = user.IsAnonymous;
                foundUser.LastActivityDate = user.LastActivityDate;
                foundUser.LoweredUserName = user.LoweredUserName;
                foundUser.MobileAlias = user.MobileAlias;
                foundUser.UserName = user.UserName;

                UnitOfWork.Context.Entry(foundUser).State = System.Data.Entity.EntityState.Modified;
                UnitOfWork.Commit();

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException(ex);
            }

        }

        public Task AddClaimAsync(aspnet_Users user, Claim claim)
        {

            throw new NotImplementedException();
        }

        public Task<aspnet_Users> FindByEmailAsync(string email)
        {

            try
            {
                aspnet_Users foundUser = Where(w => w.aspnet_Membership.Email == email).SingleOrDefault();
                return Task.FromResult(foundUser);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                return Task.FromException<aspnet_Users>(ex);
            }
        }

        public Task<int> GetAccessFailedCountAsync(aspnet_Users user)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Claim>> GetClaimsAsync(aspnet_Users user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetEmailConfirmedAsync(aspnet_Users user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetLockoutEnabledAsync(aspnet_Users user)
        {
            throw new NotImplementedException();
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(aspnet_Users user)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(aspnet_Users user)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPhoneNumberAsync(aspnet_Users user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(aspnet_Users user)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetSecurityStampAsync(aspnet_Users user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetTwoFactorEnabledAsync(aspnet_Users user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(aspnet_Users user)
        {
            throw new NotImplementedException();
        }

        public Task<int> IncrementAccessFailedCountAsync(aspnet_Users user)
        {
            throw new NotImplementedException();
        }

        public Task RemoveClaimAsync(aspnet_Users user, Claim claim)
        {
            throw new NotImplementedException();
        }

        public Task ResetAccessFailedCountAsync(aspnet_Users user)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(aspnet_Users user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEndDateAsync(aspnet_Users user, DateTimeOffset lockoutEnd)
        {
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(aspnet_Users user, string passwordHash)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberAsync(aspnet_Users user, string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberConfirmedAsync(aspnet_Users user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        public Task SetSecurityStampAsync(aspnet_Users user, string stamp)
        {
            throw new NotImplementedException();
        }

        public Task SetTwoFactorEnabledAsync(aspnet_Users user, bool enabled)
        {
            throw new NotImplementedException();
        }
    }

    public partial interface Iaspnet_UsersRepository : IRepositoryBase<aspnet_Users>
    {
        Task AddToRoleAsync(aspnet_Users user, string roleName);
        Task CreateAsync(aspnet_Users user);
        Task DeleteAsync(aspnet_Users user);
        Task<aspnet_Users> FindByIdAsync(Guid userId);
        Task<aspnet_Users> FindByNameAsync(string userName);
        Task UpdateAsync(aspnet_Users user);
        Task<IList<string>> GetRolesAsync(aspnet_Users user);
        Task<bool> IsInRoleAsync(aspnet_Users user, string roleName);
        Task RemoveFromRoleAsync(aspnet_Users user, string roleName);
        Task<aspnet_Users> FindByEmailAsync(string email);
        Task<DateTimeOffset> GetLockoutEndDateAsync(aspnet_Users user);
        Task SetLockoutEndDateAsync(aspnet_Users user, DateTimeOffset lockoutEnd);
        Task<int> IncrementAccessFailedCountAsync(aspnet_Users user);
        Task ResetAccessFailedCountAsync(aspnet_Users user);
        Task<int> GetAccessFailedCountAsync(aspnet_Users user);
        Task<bool> GetLockoutEnabledAsync(aspnet_Users user);
        Task SetPasswordHashAsync(aspnet_Users user, string passwordHash);
        Task<string> GetPasswordHashAsync(aspnet_Users user);
        Task SetPhoneNumberAsync(aspnet_Users user, string phoneNumber);
        Task<string> GetPhoneNumberAsync(aspnet_Users user);
        Task<bool> HasPasswordAsync(aspnet_Users user);
        Task<bool> GetPhoneNumberConfirmedAsync(aspnet_Users user);
        Task SetPhoneNumberConfirmedAsync(aspnet_Users user, bool confirmed);
        Task SetSecurityStampAsync(aspnet_Users user, string stamp);
        Task<string> GetSecurityStampAsync(aspnet_Users user);
        Task SetTwoFactorEnabledAsync(aspnet_Users user, bool enabled);
        Task<bool> GetTwoFactorEnabledAsync(aspnet_Users user);
        Task<IList<Claim>> GetClaimsAsync(aspnet_Users user);
        Task AddClaimAsync(aspnet_Users user, Claim claim);
        Task RemoveClaimAsync(aspnet_Users user, Claim claim);
    }
}