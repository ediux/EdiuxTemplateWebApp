using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Data;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_UsersRepository : EFRepository<aspnet_Users>, Iaspnet_UsersRepository
    {
        public void AddToRole(string applicationName, string userName, string roleName)
        {
            InternalDatabaseAlias.aspnet_UsersInRoles_AddUsersToRoles(applicationName, userName, roleName, DateTime.UtcNow);
        }

        public List<string> FindUsersInRole(string applicationName, string UserNameToMatch, string roleName)
        {
            return InternalDatabaseAlias.aspnet_UsersInRoles_FindUsersInRole(applicationName, UserNameToMatch, roleName).ToList();
        }

        public bool IsInRole(string applicationName, string userName, string roleName)
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
            var result = UnitOfWork.Context.Database.SqlQuery(typeof(int), "EXEC @return_value = [dbo].[aspnet_UsersInRoles_IsUserInRole] @ApplicationName, @UserName, @RoleName", applicationNameParameter, userNameParameter, roleNameParameter, returnCode);
            // ((IObjectContextAdapter)UnitOfWork.Context).ObjectContext.ExecuteFunction("aspnet_UsersInRoles_IsUserInRole", applicationNameParameter, userNameParameter, roleNameParameter,returnCode);
            code = (int)returnCode.Value;
            return (code == 1);
        }
    }

    public partial interface Iaspnet_UsersRepository : IRepositoryBase<aspnet_Users>
    {
        void AddToRole(string applicationName, string userName, string roleName);
        bool IsInRole(string applicationName, string userName, string roleName);
        List<string> FindUsersInRole(string applicationName, string UserNameToMatch, string roleName);
    }
}