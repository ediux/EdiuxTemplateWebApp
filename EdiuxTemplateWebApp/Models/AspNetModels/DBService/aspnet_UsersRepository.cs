using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using System.Runtime.Serialization.Formatters.Binary;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_UsersRepository : EFRepository<aspnet_Users>, Iaspnet_UsersRepository
    {
     
        public override void Delete(aspnet_Users entity)
        {
            try
            {
                var inputParam = new aspnet_Users_DeleteUser_InputParameter();

                inputParam.ApplicationName = entity.aspnet_Applications.ApplicationName;
                inputParam.TablesToDeleteFrom = (int)(TablesToCheck.aspnet_Membership | TablesToCheck.aspnet_Profile | TablesToCheck.aspnet_Roles);
                inputParam.UserName = entity.UserName;

                UnitOfWork.GetTypedContext<AspNetDbEntities2>().aspnet_Users_DeleteUser(inputParam);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public override aspnet_Users Add(aspnet_Users entity)
        {

            try
            {
                var inputParam = new aspnet_Users_CreateUser_InputParameter();

                inputParam.applicationId = entity.ApplicationId;
                inputParam.isUserAnonymous = entity.IsAnonymous;
                inputParam.lastActivityDate = entity.LastActivityDate;
                inputParam.userName = entity.UserName;

                UnitOfWork.GetTypedContext<AspNetDbEntities2>().aspnet_Users_CreateUser(inputParam);

                if (inputParam.ReturnValue == (int)MembershipCreateStatus.Success)
                {
                    return Get(inputParam.OutputParameter.UserId);
                }

                throw new Exception(string.Format("Provider Error.(ErrorCode:{0})", inputParam.ReturnValue));

            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }

        }

        public IEnumerable<aspnet_Users> FindByName(string applicationName, string userNameToMatch, int pageIndex, int pageSize)
        {
            var loweredAppName = applicationName.ToLowerInvariant();

            var memberships = (from m in All()
                               where (m.aspnet_Applications.ApplicationName == applicationName ||
                                      m.aspnet_Applications.LoweredApplicationName == loweredAppName) &&
                               (m.UserName.Contains(userNameToMatch) || m.LoweredUserName.Contains(userNameToMatch))
                               select m).AsQueryable();

            return memberships.Skip(pageSize * (pageIndex - 1)).Take(pageSize).AsEnumerable();
        }

        public IEnumerable<aspnet_Users> FindByEmail(string applicationName, string EmailToMatch, int pageIndex, int pageSize)
        {
            var loweredAppName = applicationName.ToLowerInvariant();

            var memberships = (from m in All()
                               where (m.aspnet_Applications.ApplicationName == applicationName ||
                                      m.aspnet_Applications.LoweredApplicationName == loweredAppName) &&
                               (m.aspnet_Membership.Email.Contains(EmailToMatch) ||
                                m.aspnet_Membership.LoweredEmail.Contains(EmailToMatch))
                               select m).AsQueryable();

            return memberships.Skip(pageSize * (pageIndex - 1)).Take(pageSize).AsEnumerable();
        }

        public aspnet_Users GetUserByName(string applicationName, string userName, DateTime currentTimeUtc, bool updateLastActivity)
        {
            var loweredAppName = applicationName.ToLowerInvariant();
            var loweredUserName = userName.ToLowerInvariant();
            var user = (from u in All()
                        where (u.aspnet_Applications.ApplicationName == applicationName || u.aspnet_Applications.LoweredApplicationName == loweredAppName)
                        && (u.UserName == userName || u.LoweredUserName == loweredUserName)
                        select u).SingleOrDefault();

            if (user != null)
            {
                if (updateLastActivity)
                {
                    user.aspnet_Membership.LastActivityTime = currentTimeUtc;

                    var membershipRepo = UnitOfWork.Repositories.GetRepository<aspnet_MembershipRepository>();
                    UnitOfWork.TranscationMode = true;
                    membershipRepo.Update(user.aspnet_Membership);
                    UnitOfWork.TranscationMode = false;
                    user = Reload(user);
                }
            }

            return user;
        }

        public aspnet_Users GetUserByEmail(string applicationName, string eMail, DateTime currentTimeUtc, bool updateLastActivity)
        {
            var loweredAppName = applicationName.ToLowerInvariant();
            var loweredeMail = eMail.ToLowerInvariant();
            var user = (from u in All()
                        where (u.aspnet_Applications.ApplicationName == applicationName || u.aspnet_Applications.LoweredApplicationName == loweredAppName)
                        && (u.aspnet_Membership.Email == eMail || u.aspnet_Membership.LoweredEmail == loweredeMail)
                        select u).SingleOrDefault();

            if (updateLastActivity)
            {
                user.aspnet_Membership.LastActivityTime = currentTimeUtc;

                var membershipRepo = UnitOfWork.Repositories.GetRepository<aspnet_MembershipRepository>();
                UnitOfWork.TranscationMode = true;
                membershipRepo.Update(user.aspnet_Membership);
                UnitOfWork.TranscationMode = false;
                user = Reload(user);
            }
            return user;
        }

        public IEnumerable<aspnet_Users> GetAllUsers(string applicationName, int pageIndex, int pageSize, out int totalRecords)
        {
            var loweredAppName = applicationName.ToLowerInvariant();

            var users = (from u in All()
                         where (u.aspnet_Applications.ApplicationName == applicationName
                         || u.aspnet_Applications.LoweredApplicationName == loweredAppName)
                         select u);

            if (users.Any())
            {
                totalRecords = users.Count();
                return users.Skip(pageIndex * pageSize).Take(pageSize).AsEnumerable();
            }

            totalRecords = 0;
            return users.AsEnumerable();
        }

        public aspnet_Users GetUserByUserId(Guid userId, DateTime currentTimeUtc, bool updateLastActivity)
        {

            var user = (from u in All()
                        where u.Id == userId
                        select u).SingleOrDefault();

            if (updateLastActivity)
            {
                user.aspnet_Membership.LastActivityTime = currentTimeUtc;

                var membershipRepo = UnitOfWork.Repositories.GetRepository<aspnet_MembershipRepository>();
                UnitOfWork.TranscationMode = true;
                membershipRepo.Update(user.aspnet_Membership);
                UnitOfWork.TranscationMode = false;
            }
            return user;
        }

        public int getNumberOfUsersOnline(string applicationName, int minutesSinceLastInActive, DateTime currentTimeUtc)
        {
            aspnet_Membership_GetNumberOfUsersOnline_InputParameter paramObject = new aspnet_Membership_GetNumberOfUsersOnline_InputParameter();

            paramObject.ApplicationName = applicationName;
            paramObject.CurrentTimeUtc = currentTimeUtc;
            paramObject.MinutesSinceLastInActive = minutesSinceLastInActive;

            UnitOfWork.GetTypedContext<AspNetDbEntities2>().aspnet_Membership_GetNumberOfUsersOnline(paramObject);

            if (paramObject.ReturnValue == 0)
            {
                return paramObject.OutputParameter.NumOnline;
            }

            return 0;
        }

        public void AddToRole(aspnet_Users user, string roleName)
        {
            if (user.ApplicationId == Guid.Empty || user.ApplicationId == null) {
                throw new ArgumentException("ApplicationId can't been null or empty.", nameof(user.ApplicationId));
            }

            if (string.IsNullOrEmpty(user.UserName))
            {
                if (string.IsNullOrEmpty(user.LoweredUserName))
                {
                    throw new ArgumentException("ApplicationId can't been null or empty.", nameof(user.ApplicationId));
                }
            }

            var roleRepo = UnitOfWork.Repositories.GetRepository<Iaspnet_RolesRepository>();
            var loweredRoleName = roleName.ToLowerInvariant();

            var roles = roleRepo
                .Where(s => s.ApplicationId == user.ApplicationId &&
                       (s.Name == roleName
                        || s.LoweredRoleName == loweredRoleName));

            if (!roles.SelectMany(s => s.aspnet_Users).Any(w => w.Id == user.Id))
            {
                if (!roleRepo.IsExists(new aspnet_Roles()
                {
                    Name = roleName,
                    LoweredRoleName = roleName.ToLowerInvariant(),
                    ApplicationId = user.ApplicationId,
                    aspnet_Applications = user.aspnet_Applications
                }))
                {
                    throw new Exception(string.Format("The Role '{0}' not found.", roleName));
                }

                var existedUser = Get(user.Id);

                if (existedUser == null)
                {
                    throw new Exception(string.Format("The User '{0}' not found.", user.UserName));
                }

                var loweredUserName = user.UserName.ToLowerInvariant();

                var foundrole = roles.SingleOrDefault();

                if (foundrole != null)
                {
                    existedUser.aspnet_Roles.Add(foundrole);
                    UnitOfWork.Commit();
                }
            }
        }

        public bool IsInRole(aspnet_Users user, string roleName)
        {
            var paramObject = new aspnet_UsersInRoles_IsUserInRole_InputParameter();

            paramObject.applicationName = user.aspnet_Applications.ApplicationName;
            paramObject.roleName = roleName;
            paramObject.userName = user.UserName;

            if (paramObject.ReturnValue == 1)
            {
                return true;
            }

            return false;
        }

        public void RemoveFromRole(aspnet_Users user, string roleName)
        {
            var paramObject = new aspnet_UsersInRoles_RemoveUsersFromRoles_InputParameter();

            paramObject.applicationName = user?.aspnet_Applications.ApplicationName;
            paramObject.roleNames = roleName;
            paramObject.userNames = user.UserName;

            UnitOfWork.GetTypedContext<AspNetDbEntities2>().aspnet_UsersInRoles_RemoveUsersFromRoles(paramObject);

            if (paramObject.ReturnValue != 0)
            {
                throw new Exception(string.Format("發生錯誤!(錯誤碼:{0})", paramObject.ReturnValue));
            }
        }

        public aspnet_Users Update(aspnet_Users entity)
        {
            var appId = entity.ApplicationId;
            var appName = entity?.aspnet_Applications.ApplicationName;

            if (entity.ApplicationId == default(Guid))
            {
                if (entity.aspnet_Applications == null)
                {
                    throw new ArgumentNullException(nameof(entity.aspnet_Applications));
                }
                else
                {
                    if (string.IsNullOrEmpty(entity.aspnet_Applications.ApplicationName))
                    {
                        if (string.IsNullOrEmpty(entity.aspnet_Applications.LoweredApplicationName))
                        {
                            throw new ArgumentNullException(nameof(entity.ApplicationId));
                        }
                        else
                        {
                            appName = entity.LoweredUserName;
                        }
                    }
                    else
                    {
                        appName = entity.aspnet_Applications.ApplicationName;
                    }
                }
            }

            var userId = entity.Id;
            var userName = entity.UserName;

            if (entity.Id == default(Guid))
            {
                if (string.IsNullOrEmpty(entity.LoweredUserName))
                {
                    if (string.IsNullOrEmpty(entity.UserName))
                    {
                        throw new ArgumentNullException(nameof(entity.UserName));
                    }
                    else
                    {
                        userName = entity.UserName;
                    }
                }
                else
                {
                    userName = entity.LoweredUserName;
                }
            }

            var foundUser = Where(w => (w.Id == userId || w.LoweredUserName == userName || w.UserName == userName) &&
                                  (w.ApplicationId == appId && (
                                  w.aspnet_Applications.LoweredApplicationName == appName || w.aspnet_Applications.ApplicationName == appName))).SingleOrDefault();

            if (foundUser == null)
            {
                return foundUser;
            }

            UnitOfWork.TranscationMode = true;

            foundUser = CopyTo<aspnet_Users>(entity);

            UnitOfWork.TranscationMode = false;
            UnitOfWork.Commit();

            return Reload(entity);
        }
    }

    public partial interface Iaspnet_UsersRepository : IRepositoryBase<aspnet_Users>
    {
        /// <summary>
        /// Finds the name of the by.
        /// </summary>
        /// <returns>The by name.</returns>
        /// <param name="applicationName">Application name.</param>
        /// <param name="userNameToMatch">User name to match.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        IEnumerable<aspnet_Users> FindByName(string applicationName, string userNameToMatch, int pageIndex, int pageSize);

        /// <summary>
        /// Finds the by email.
        /// </summary>
        /// <returns>The by email.</returns>
        /// <param name="applicationName">Application name.</param>
        /// <param name="EmailToMatch">Email to match.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        IEnumerable<aspnet_Users> FindByEmail(string applicationName, string EmailToMatch, int pageIndex, int pageSize);

        /// <summary>
        /// Gets the name of the user by.
        /// </summary>
        /// <returns>The user by name.</returns>
        /// <param name="applicationName">Application name.</param>
        /// <param name="userName">User name.</param>
        /// <param name="currentTimeUtc">Current time UTC.</param>
        /// <param name="updateLastActivity">If set to <c>true</c> update last activity.</param>
        aspnet_Users GetUserByName(string applicationName, string userName, DateTime currentTimeUtc, bool updateLastActivity);

        /// <summary>
        /// Gets the user by email.
        /// </summary>
        /// <returns>The user by email.</returns>
        /// <param name="applicationName">Application name.</param>
        /// <param name="eMail">E mail.</param>
        /// <param name="currentTimeUtc"></param>
        /// <param name="updateLastActivity"></param>
        aspnet_Users GetUserByEmail(string applicationName, string eMail, DateTime currentTimeUtc, bool updateLastActivity);


        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns>The all users.</returns>
        /// <param name="applicationName">Application name.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="totalRecords">Total records.</param>
        IEnumerable<aspnet_Users> GetAllUsers(string applicationName, int pageIndex, int pageSize, out int totalRecords);

        /// <summary>
        /// Gets the user by user identifier.
        /// </summary>
        /// <returns>The user by user identifier.</returns>
        /// <param name="userId">User identifier.</param>
        /// <param name="currentTimeUtc">Current time UTC.</param>
        /// <param name="updateLastActivity">If set to <c>true</c> update last activity.</param>
        aspnet_Users GetUserByUserId(Guid userId, DateTime currentTimeUtc, bool updateLastActivity);

        /// <summary>
        /// Gets the number of users online.
        /// </summary>
        /// <returns>The number of users online.</returns>
        /// <param name="applicationName">Application name.</param>
        /// <param name="minutesSinceLastInActive">Minutes since last in active.</param>
        /// <param name="currentTimeUtc">Current time UTC.</param>
        int getNumberOfUsersOnline(string applicationName, int minutesSinceLastInActive, DateTime currentTimeUtc);

        /// <summary>
        /// Adds to role.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="roleName">Role name.</param>
        void AddToRole(aspnet_Users user, string roleName);

        /// <summary>
        /// Ises the in role.
        /// </summary>
        /// <returns><c>true</c>, if in role was ised, <c>false</c> otherwise.</returns>
        /// <param name="user">User.</param>
        /// <param name="roleName">Role name.</param>
        bool IsInRole(aspnet_Users user, string roleName);

        /// <summary>
        /// Removes from role.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="roleName">Role name.</param>
        void RemoveFromRole(aspnet_Users user, string roleName);

        /// <summary>
        /// Update the specified entity.
        /// </summary>
        /// <returns>The update.</returns>
        /// <param name="entity">Entity.</param>
        aspnet_Users Update(aspnet_Users entity);
    }
}
