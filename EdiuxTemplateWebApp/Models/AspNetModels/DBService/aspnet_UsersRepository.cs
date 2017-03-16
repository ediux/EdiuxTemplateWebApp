using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity;
using Microsoft.AspNet.Identity;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_UsersRepository : EFRepository<aspnet_Users>, Iaspnet_UsersRepository
    {
        public aspnet_UsersRepository() : base()
        {
            RegisterDependcyRepository(RepositoryHelper.Getaspnet_MembershipRepository(UnitOfWork));
            RegisterDependcyRepository(RepositoryHelper.Getaspnet_PersonalizationPerUserRepository(UnitOfWork));
            RegisterDependcyRepository(RepositoryHelper.Getaspnet_ProfileRepository(UnitOfWork));
            RegisterDependcyRepository(RepositoryHelper.Getaspnet_RolesRepository(UnitOfWork));
            RegisterDependcyRepository(RepositoryHelper.Getaspnet_UserLoginRepository(UnitOfWork));
        }

        #region 相依性
        protected IQueryable<aspnet_Membership> Memberships
        {
            get
            {
                return (_depencyRepos["aspnet_MembershipRepository"] as Iaspnet_MembershipRepository).All();
            }
        }

        protected IQueryable<aspnet_PersonalizationPerUser> PersonalizationPerUsers
        {
            get
            {
                return (_depencyRepos["aspnet_PersonalizationPerUserRepository"] as Iaspnet_PersonalizationPerUserRepository).All();
            }
        }

        protected IQueryable<aspnet_Profile> Profiles
        {
            get
            {
                return (DependcyRepository["aspnet_ProfileRepository"] as Iaspnet_ProfileRepository).All();
            }
        }

        protected IQueryable<aspnet_Roles> Roles
        {
            get
            {
                return (DependcyRepository["aspnet_RolesRepository"] as Iaspnet_RolesRepository).All();
            }
        }

        protected IQueryable<aspnet_UserLogin> UserLogins
        {
            get
            {
                return (DependcyRepository["aspnet_UserLoginRepository"] as Iaspnet_UserLoginRepository).All();
            }
        }

        #endregion

        public IQueryable<aspnet_Users> All(aspnet_Applications application)
        {
            try
            {

                IQueryable<aspnet_Users> query =
                    from u in ObjectSet
                    from r in Roles
                    from ub in r.aspnet_Users
                    join m in Memberships on u.Id equals m.UserId
                    join ppu in PersonalizationPerUsers on u.Id equals ppu.UserId
                    join p in Profiles on u.Id equals p.UserId
                    where ub.Id == u.Id && u.ApplicationId == application.ApplicationId
                    select u;


                query.Load();

                return query;
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
                IQueryable<aspnet_Users> query =
                    from u in ObjectSet
                    from r in Roles
                    from ub in r.aspnet_Users
                    join m in Memberships on u.Id equals m.UserId
                    join ppu in PersonalizationPerUsers on u.Id equals ppu.UserId
                    join p in Profiles on u.Id equals p.UserId
                    where ub.Id == u.Id
                    select u;


                query.Load();

                return query;
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
                aspnet_Users_DeleteUser_InputParameter inputParam = new aspnet_Users_DeleteUser_InputParameter();

                inputParam.ApplicationName = entity.aspnet_Applications.ApplicationName;
                inputParam.TablesToDeleteFrom = (int)(TablesToCheck.aspnet_Membership | TablesToCheck.aspnet_Profile | TablesToCheck.aspnet_Roles);
                inputParam.UserName = entity.UserName;

                UnitOfWork.Repositories.aspnet_Users_DeleteUser(inputParam);
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
                aspnet_Users_CreateUser_InputParameter inputParam = new AspNetModels.aspnet_Users_CreateUser_InputParameter();

                inputParam.applicationId = entity.ApplicationId;
                inputParam.isUserAnonymous = entity.IsAnonymous;
                inputParam.lastActivityDate = entity.LastActivityDate;
                inputParam.userName = entity.UserName;

                UnitOfWork.Repositories.aspnet_Users_CreateUser(inputParam);

                if (inputParam.ReturnValue == (int)System.Web.Security.MembershipCreateStatus.Success)
                {
                    return Get(inputParam.OutputParameter.UserId);
                }
                else
                {
                    throw new Exception(string.Format("Provider Error.(ErrorCode:{0})", inputParam.ReturnValue));
                }

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
            return InternalDatabaseAlias.aspnet_UsersInRoles_FindUsersInRole(applicationName, UserNameToMatch, roleName)
                .Select(s => s.UserName).ToList();
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
                return InternalDatabaseAlias.aspnet_UsersInRoles_GetRolesForUser(applicationName, userName)
                    .Select(s => s.RoleName).ToList();
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

        public aspnet_Users FindByName(string applicationName, string userName)
        {
            try
            {
                Iaspnet_ApplicationsRepository appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository(UnitOfWork);
                var app = appRepo.FindByName(applicationName);
                var loweredUserName = userName.ToLowerInvariant();
                if (app == null)
                {
                    return null;
                }
                var users = Where(s => s.ApplicationId == app.ApplicationId
                && (s.UserName == userName || s.LoweredUserName == loweredUserName))
                .OrderByDescending(o => o.LastActivityDate);

                return users.FirstOrDefault();
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }

        }
    }

    public partial interface Iaspnet_UsersRepository : IRepositoryBase<aspnet_Users>
    { 
        IQueryable<aspnet_Users> All(aspnet_Applications application);
        void AddToRole(string applicationName, string userName, string roleName);
        bool IsInRole(string applicationName, string userName, string roleName);
        IList<string> FindUsersInRole(string applicationName, string UserNameToMatch, string roleName);
        IList<string> GetRolesForUser(string applicationName, string userName);
        aspnet_Users FindByName(string applicationName, string userName);
    }
}
