using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_MembershipRepository : EFRepository<aspnet_Membership>, Iaspnet_MembershipRepository
    {
        public Task<aspnet_Membership> FindByEmailAsync(string email)
        {
            return Task.FromResult(Where(s => s.Email == email).SingleOrDefault());
        }

        public override aspnet_Membership Add(aspnet_Membership entity)
        {
            return base.Add(entity);
        }

        public System.Web.Security.MembershipCreateStatus CreateUser(string applicationName, string userName,
            string password, string passwordSalt, string eMail, out Guid userId, string passwordQuestion = "",
            string passwordAnswer = "", bool isApproved = false, int uniqueEmail = 0,
            System.Web.Security.MembershipPasswordFormat passwordFormat = System.Web.Security.MembershipPasswordFormat.Hashed)
        {

            try
            {
                var applicationNameParameter = applicationName != null ?
                new SqlParameter("@ApplicationName", applicationName) :
                new SqlParameter("@ApplicationName", SqlDbType.NVarChar, 256);

                var userNameParameter = userName != null ?
                    new SqlParameter("@UserName", userName) :
                    new SqlParameter("@UserName", SqlDbType.NVarChar, 256);

                var passwordParameter = password != null ?
                    new SqlParameter("@Password", password) :
                    new SqlParameter("@Password", SqlDbType.NVarChar, 128);

                var passwordSaltParameter = passwordSalt != null ?
                    new SqlParameter("@PasswordSalt", passwordSalt) :
                    new SqlParameter("@PasswordSalt", SqlDbType.NVarChar, 128);


                var eMailParameter = eMail != null ?
                   new SqlParameter("@Email", eMail) :
                   new SqlParameter("@Email", SqlDbType.NVarChar, 256);

                var passwordQuestionParameter = passwordQuestion != null ?
                    new SqlParameter("@PasswordQuestion", passwordQuestion) :
                    new SqlParameter("@PasswordQuestion", SqlDbType.NVarChar, 256);

                var passwordAnswerParameter = passwordAnswer != null ?
                   new SqlParameter("@PasswordAnswer", passwordAnswer) :
                   new SqlParameter("@PasswordAnswer", SqlDbType.NVarChar, 128);

                var isApprovedParameter = isApproved ?
                   new SqlParameter("@IsApproved", isApproved) :
                   new SqlParameter("@IsApproved", SqlDbType.Bit);

                var currentTimeUtcParameter =
                    new SqlParameter("@CurrentTimeUtc", DateTime.UtcNow);

                var createDateParameter =
                    new SqlParameter("@CreateDate", DateTime.UtcNow.Date);

                var uniqueEmailParameter =
                    new SqlParameter("@UniqueEmail", uniqueEmail);

                var passwordFormatParameter =
                    new SqlParameter("@PasswordFormat", (int)passwordFormat);

                var userIdParameter = new SqlParameter("@UserId", SqlDbType.UniqueIdentifier)
                {
                    Direction = ParameterDirection.Output
                };

                var returnCode = new SqlParameter();
                returnCode.ParameterName = "@return_value";
                returnCode.SqlDbType = SqlDbType.Int;
                returnCode.Direction = ParameterDirection.Output;

                int code = 0;
                var result = UnitOfWork.Context.Database.SqlQuery(typeof(int), "EXEC @return_value = [dbo].[aspnet_Membership_CreateUser] @ApplicationName, @UserName, @Password, @PasswordSalt, @Email, @PasswordQuestion, @PasswordAnswer, @IsApproved, @CurrentTimeUtc, @CreateDate, @UniqueEmail, @PasswordFormat, @UserId OUT",
                    applicationNameParameter, userNameParameter, passwordParameter, passwordSaltParameter,
                    eMailParameter,passwordQuestionParameter,passwordAnswerParameter,
                    isApprovedParameter,currentTimeUtcParameter,
                    createDateParameter,
                    uniqueEmailParameter,
                    passwordFormatParameter,
                    userIdParameter,
                    returnCode);
                // ((IObjectContextAdapter)UnitOfWork.Context).ObjectContext.ExecuteFunction("aspnet_UsersInRoles_IsUserInRole",  applicationNameParameter, userNameParameter, RoleNameParameter,returnCode);
                userId = (Guid)userIdParameter.Value;
                code = (int)returnCode.Value;
                switch (code)
                {
                    case -1:
                        return System.Web.Security.MembershipCreateStatus.ProviderError;
                    case 0:
                        return System.Web.Security.MembershipCreateStatus.Success;
                    case 6:
                        return System.Web.Security.MembershipCreateStatus.DuplicateProviderUserKey;
                    case 7:
                        return System.Web.Security.MembershipCreateStatus.DuplicateEmail;
                    case 10:
                        return System.Web.Security.MembershipCreateStatus.DuplicateUserName;
                    default:
                        return System.Web.Security.MembershipCreateStatus.UserRejected;
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }
    }

    public partial interface Iaspnet_MembershipRepository : IRepositoryBase<aspnet_Membership>
    {
        System.Web.Security.MembershipCreateStatus CreateUser(string applicationName, string userName,
            string password, string passwordSalt, string eMail, out Guid userId, string passwordQuestion = "",
            string passwordAnswer = "", bool isApproved = false, int uniqueEmail = 0,
            System.Web.Security.MembershipPasswordFormat passwordFormat = System.Web.Security.MembershipPasswordFormat.Hashed);



    }
}