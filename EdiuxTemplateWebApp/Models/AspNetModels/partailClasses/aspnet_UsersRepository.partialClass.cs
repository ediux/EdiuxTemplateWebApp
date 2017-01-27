using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity.Core.Objects;
using System.Security.Claims;
using System.Data.Entity;
using System.Runtime.Caching;
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;
using System.Data;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_UsersRepository : EFRepository<aspnet_Users>, Iaspnet_UsersRepository
    {
        public IQueryable<aspnet_Users> All(aspnet_Applications application)
        {
            try
            {
                InternalDatabaseAlias.aspnet_Users.Where(w => w.ApplicationId == application.ApplicationId).Load();
                InternalDatabaseAlias.aspnet_Membership.Where(w => w.ApplicationId == application.ApplicationId).Load();
                InternalDatabaseAlias.aspnet_PersonalizationPerUser.Where(w => w.aspnet_Users.ApplicationId == application.ApplicationId).Load();
                InternalDatabaseAlias.aspnet_Profile.Where(w => w.aspnet_Users.ApplicationId == application.ApplicationId).Load();
                InternalDatabaseAlias.aspnet_Roles.Where(w => w.aspnet_Applications.ApplicationId == application.ApplicationId).Load();
                InternalDatabaseAlias.aspnet_UserLogin.Where(w => w.aspnet_Users.ApplicationId == application.ApplicationId).Load();

                return InternalDatabaseAlias.aspnet_Users
                    .Where(w => w.ApplicationId == application.ApplicationId)
                    .Include(p => p.aspnet_Applications)
                    .Include(p => p.aspnet_Membership)
                    .Include(p => p.aspnet_PersonalizationPerUser)
                    .Include(p => p.aspnet_Profile)
                    .Include(p => p.aspnet_Roles)
                    .Include(p => p.aspnet_UserClaims)
                    .Include(p => p.aspnet_UserLogin);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }

        }

        public override IQueryable<aspnet_Users> All()
        {
            try
            {
                InternalDatabaseAlias.aspnet_Membership.Load();
                InternalDatabaseAlias.aspnet_PersonalizationPerUser.Load();
                InternalDatabaseAlias.aspnet_Profile.Load();
                InternalDatabaseAlias.aspnet_Roles.Load();
                InternalDatabaseAlias.aspnet_UserLogin.Load();

                return InternalDatabaseAlias.aspnet_Users
                    .Include(p => p.aspnet_Applications)
                    .Include(p => p.aspnet_Membership)
                    .Include(p => p.aspnet_PersonalizationPerUser)
                    .Include(p => p.aspnet_Profile)
                    .Include(p => p.aspnet_Roles)
                    .Include(p => p.aspnet_UserClaims)
                    .Include(p => p.aspnet_UserLogin);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }

        }

        public override void Delete(aspnet_Users entity)
        {
            try
            {
                ObjectParameter numTablesDeletedFrom = new ObjectParameter("NumTablesDeletedFrom", typeof(Guid));
                InternalDatabaseAlias.aspnet_Users_DeleteUser(entity.aspnet_Applications.ApplicationName, entity.UserName, 14, numTablesDeletedFrom);
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
                var applicationIdParameter = entity != null ?
                    new SqlParameter("@ApplicationId", entity.ApplicationId) :
                    new SqlParameter("@ApplicationId", SqlDbType.UniqueIdentifier);

                var userNameParameter = entity != null ?
                    new SqlParameter("@UserName", entity.UserName) :
                    new SqlParameter("@UserName", SqlDbType.NVarChar, 256);

                var isUserAnonymousParameter = entity != null ?
                    new SqlParameter("@IsUserAnonymous", entity.IsAnonymous) :
                    new SqlParameter("@IsUserAnonymous", SqlDbType.Bit);


                var lastActivityDateParameter = entity != null ?
                   new SqlParameter("@LastActivityDate", entity.LastActivityDate) :
                   new SqlParameter("@LastActivityDate", SqlDbType.DateTime);


                var userIdParameter = new SqlParameter("@UserId", SqlDbType.UniqueIdentifier) { Direction = ParameterDirection.Output };

                var returnCode = new SqlParameter("@return_value", SqlDbType.Int) { Direction = ParameterDirection.Output };


                int code = 0;
                var result = UnitOfWork.Context.Database.SqlQuery(typeof(int),
                    "EXEC @return_value = [dbo].[aspnet_Users_CreateUser] @ApplicationId, @UserName, @IsUserAnonymous,@LastActivityDate,@UserId OUTPUT",
                    applicationIdParameter, userNameParameter, isUserAnonymousParameter, lastActivityDateParameter, userIdParameter, returnCode);
                // ((IObjectContextAdapter)UnitOfWork.Context).ObjectContext.ExecuteFunction("aspnet_Users_CreateUser",  applicationIdParameter, userNameParameter, RoleNameParameter,returnCode);
                code = (int)returnCode.Value;
                entity.Id = (Guid)userIdParameter.Value;
                return Get(entity.Id);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
 
        }
        public aspnet_Users Add(string userName, string password, aspnet_Applications applicationObject, string eMail = "@abc.com", bool IsUserAnonymous = false)
        {
            try
            {
                string passwordSalt = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                PasswordHasher hasher = new PasswordHasher();
                password = hasher.HashPassword(password + passwordSalt);
                Guid userId = Guid.Empty;

                Iaspnet_MembershipRepository membershipRepo = RepositoryHelper.Getaspnet_MembershipRepository(UnitOfWork);

                var status = membershipRepo.CreateUser(applicationObject.ApplicationName, userName, password, passwordSalt,
                       eMail, out userId, isApproved: true);

                if (status == System.Web.Security.MembershipCreateStatus.Success)
                {
                    return Get(userId);
                }

                return null;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public void AddToRole(string applicationName, string userName, string roleName)
        {
            InternalDatabaseAlias.aspnet_UsersInRoles_AddUsersToRoles(applicationName, userName, roleName, DateTime.UtcNow);
        }

        public IList<string> FindUsersInRole(string applicationName, string UserNameToMatch, string roleName)
        {
            return InternalDatabaseAlias.aspnet_UsersInRoles_FindUsersInRole(applicationName, UserNameToMatch, roleName).ToList();
        }

        public bool IsInRole(string applicationName, string userName, string roleName)
        {

            try
            {
                var applicationNameParameter = applicationName != null ?
                new SqlParameter("@ApplicationName", applicationName) :
                new SqlParameter("@ApplicationName", SqlDbType.NVarChar, 256);

                var userNameParameter = userName != null ?
                    new SqlParameter("@UserName", userName) :
                    new SqlParameter("@UserName", SqlDbType.NVarChar, 256);

                var roleNameParameter = roleName != null ?
                    new SqlParameter("@RoleName", roleName) :
                    new SqlParameter("@RoleName", SqlDbType.NVarChar, 256);

                var returnCode = new SqlParameter();
                returnCode.ParameterName = "@return_value";
                returnCode.SqlDbType = SqlDbType.Int;
                returnCode.Direction = ParameterDirection.Output;

                int code = 0;
                
                var result = UnitOfWork.Context.Database.ExecuteSqlCommand("EXEC @return_value = [dbo].[aspnet_UsersInRoles_IsUserInRole] @ApplicationName, @UserName, @RoleName", applicationNameParameter, userNameParameter, roleNameParameter, returnCode);
                // ((IObjectContextAdapter)UnitOfWork.Context).ObjectContext.ExecuteFunction("aspnet_UsersInRoles_IsUserInRole", applicationNameParameter, userNameParameter, roleNameParameter,returnCode);
                
                code = (int)returnCode.Value;
                return (code == 1);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }

        }

        public IList<string> GetRolesForUser(string applicationName, string userName)
        {

            try
            {
                return InternalDatabaseAlias.aspnet_UsersInRoles_GetRolesForUser(applicationName, userName).ToList();
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }

        }

        private static void updateCache(aspnet_Applications applicationObject)
        {
            try
            {
                MemoryCache.Default.Set("ApplicationInfo", applicationObject, DateTime.UtcNow.AddMinutes(38400));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }

    public partial interface Iaspnet_UsersRepository : IRepositoryBase<aspnet_Users>
    {
        aspnet_Users Add(string userName, string password, aspnet_Applications applicationObject, string eMail = "", bool IsUserAnonymous = false);
        IQueryable<aspnet_Users> All(aspnet_Applications application);
        void AddToRole(string applicationName, string userName, string roleName);
        bool IsInRole(string applicationName, string userName, string roleName);
        IList<string> FindUsersInRole(string applicationName, string UserNameToMatch, string roleName);

        IList<string> GetRolesForUser(string applicationName, string userName);

    }
}