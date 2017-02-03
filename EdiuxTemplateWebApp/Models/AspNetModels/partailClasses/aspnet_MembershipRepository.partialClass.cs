using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using EdiuxTemplateWebApp;

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
                return UnitOfWork.GetDbContext<AspNetDbEntities2>().aspnet_Membership_CreateUser(
                          applicationName,
                          userName,
                          password,
                          passwordSalt,
                          eMail,
                           passwordQuestion,
                           passwordAnswer,
                           isApproved,
                            DateTime.UtcNow,
                            DateTime.UtcNow.Date,
                            uniqueEmail,
                        (int)passwordFormat,
                            out userId
                          );


                //switch (code)
                //{
                //    case -1:
                //        return System.Web.Security.MembershipCreateStatus.ProviderError;
                //    case 0:
                //        return System.Web.Security.MembershipCreateStatus.Success;
                //    case 6:
                //        return System.Web.Security.MembershipCreateStatus.DuplicateProviderUserKey;
                //    case 7:
                //        return System.Web.Security.MembershipCreateStatus.DuplicateEmail;
                //    case 10:
                //        return System.Web.Security.MembershipCreateStatus.DuplicateUserName;
                //    default:
                //        return System.Web.Security.MembershipCreateStatus.UserRejected;
                //}
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public bool ChangePasswordQuestionAndAnswer(string applicationName, string userName, string newPasswordQuestion, string newPasswordAnswer)
        {

            try
            {
                var applicationNameParameter = applicationName != null ?
                    new SqlParameter("@ApplicationName", applicationName) :
                    new SqlParameter("@ApplicationName", SqlDbType.NVarChar, 256);

                var userNameParameter = userName != null ?
                    new SqlParameter("@UserName", userName) :
                    new SqlParameter("@UserName", SqlDbType.NVarChar, 256);

                var newPasswordQuestionParameter = newPasswordQuestion != null ?
                    new SqlParameter("@NewPasswordQuestion", newPasswordQuestion) :
                    new SqlParameter("@NewPasswordQuestion", SqlDbType.NVarChar, 256);

                var newPasswordAnswerParameter = newPasswordAnswer != null ?
                   new SqlParameter("@NewPasswordAnswer ", newPasswordAnswer) :
                   new SqlParameter("@NewPasswordAnswer ", SqlDbType.NVarChar, 128);

                var returnCode = new SqlParameter();
                returnCode.ParameterName = "@return_value";
                returnCode.SqlDbType = SqlDbType.Int;
                returnCode.Direction = ParameterDirection.Output;

                int code = 0;
                var result = UnitOfWork.Context.Database.ExecuteSqlCommand(
                    "EXEC @return_value = [dbo].[aspnet_Membership_ChangePasswordQuestionAndAnswer] @ApplicationName, @UserName, @NewPasswordQuestion, @NewPasswordAnswer",
                    applicationNameParameter, userNameParameter, newPasswordQuestionParameter, newPasswordAnswerParameter, returnCode);

                code = (int)returnCode.Value;
                return (code == 1);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public IEnumerable<aspnet_Membership> FindUsersByEmail(string applicationName, string emailToMatch, out int TotalRecords, int PageIndex = 1, int PageSize = 5)
        {


            try
            {
                var applicationNameParameter = applicationName != null ?
                new SqlParameter("@ApplicationName", applicationName) :
                new SqlParameter("@ApplicationName", SqlDbType.NVarChar, 256);

                var emailToMatchParameter = emailToMatch != null ?
                    new SqlParameter("@EmailToMatch", emailToMatch) :
                    new SqlParameter("@EmailToMatch", SqlDbType.NVarChar, 256);

                var PageIndexParameter =
                    new SqlParameter("@PageIndex", PageIndex);

                var PageSizeParameter = new SqlParameter("@PageSize", PageSize);

                var returnCode = new SqlParameter();
                returnCode.ParameterName = "@return_value";
                returnCode.SqlDbType = SqlDbType.Int;
                returnCode.Direction = ParameterDirection.Output;

                var result = UnitOfWork.Context.Database.SqlQuery<aspnet_Membership>(
                    "EXEC @return_value = [dbo].[aspnet_Membership_FindUsersByEmail] @ApplicationName, @EmailToMatch, @PageIndex, @PageSize", applicationNameParameter, emailToMatchParameter, PageIndexParameter, PageSizeParameter, returnCode);

                TotalRecords = (int)returnCode.Value;

                return result.AsEnumerable();
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public IEnumerable<aspnet_Membership> FindUsersByName(string applicationName, string userNameToMatch, out int TotalRecords, int PageIndex = 1, int PageSize = 5)
        {
            try
            {
                var applicationNameParameter = applicationName != null ?
                new SqlParameter("@ApplicationName", applicationName) :
                new SqlParameter("@ApplicationName", SqlDbType.NVarChar, 256);

                var userNameToMatchParameter = userNameToMatch != null ?
                    new SqlParameter("@UserNameToMatch", userNameToMatch) :
                    new SqlParameter("@UserNameToMatch", SqlDbType.NVarChar, 256);

                var PageIndexParameter =
                    new SqlParameter("@PageIndex", PageIndex);

                var PageSizeParameter = new SqlParameter("@PageSize", PageSize);

                var returnCode = new SqlParameter();
                returnCode.ParameterName = "@return_value";
                returnCode.SqlDbType = SqlDbType.Int;
                returnCode.Direction = ParameterDirection.Output;

                var result = UnitOfWork.Context.Database.SqlQuery<aspnet_Membership>(
                    "EXEC @return_value = [dbo].[aspnet_Membership_FindUsersByName] @ApplicationName, @UserNameToMatch, @PageIndex, @PageSize", applicationNameParameter, userNameToMatch, PageIndexParameter, PageSizeParameter, returnCode);

                TotalRecords = (int)returnCode.Value;

                return result.AsEnumerable();
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public IEnumerable<aspnet_Membership> GetAllUsers(string applicationName, out int TotalRecords, int PageIndex = 1, int PageSize = 5)
        {
            try
            {
                var applicationNameParameter = applicationName != null ?
                    new SqlParameter("@ApplicationName", applicationName) :
                    new SqlParameter("@ApplicationName", SqlDbType.NVarChar, 256);

                var PageIndexParameter =
                    new SqlParameter("@PageIndex", PageIndex);

                var PageSizeParameter = new SqlParameter("@PageSize", PageSize);

                var returnCode = new SqlParameter();
                returnCode.ParameterName = "@return_value";
                returnCode.SqlDbType = SqlDbType.Int;
                returnCode.Direction = ParameterDirection.Output;

                var result = UnitOfWork.Context.Database.SqlQuery<aspnet_Membership>(
                    "EXEC @return_value = [dbo].[aspnet_Membership_GetAllUsers] @ApplicationName, @PageIndex, @PageSize", applicationNameParameter, PageIndexParameter, PageSizeParameter, returnCode);

                TotalRecords = (int)returnCode.Value;

                return result.AsEnumerable();
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public int GetNumberOfUsersOnline(string applicationName,
           int MinutesSinceLastInActive,DateTime CurrentTimeUtc)
        {
            try
            {
                var db = UnitOfWork.GetDbContext<AspNetDbEntities2>();
                var result = db.aspnet_Membership_GetNumberOfUsersOnline(applicationName, MinutesSinceLastInActive, CurrentTimeUtc);
                return result;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public string GetPassword(string applicationName, string userName, int maxInvalidPasswordAttempts, int PasswordAttemptWindow, DateTime CurrentTimeUtc, out System.Web.Security.MembershipPasswordFormat passwordFormat, string PasswordAnswer = null)
        {

            try
            {
                ICollection<aspnet_Membership_GetPassword_Result> output = null;

                var result = ((AspNetDbEntities2)UnitOfWork.Context).ExecuteStoredProcedureOrSqlFunction("aspnet_Membership_GetPassword",
              out output, applicationName, userName, maxInvalidPasswordAttempts, PasswordAttemptWindow, CurrentTimeUtc, PasswordAnswer);
                //var result = InternalDatabaseAlias.aspnet_Membership_GetPassword(applicationName, userName, maxInvalidPasswordAttempts, PasswordAttemptWindow, CurrentTimeUtc, PasswordAnswer).SingleOrDefault();

                //if (result == null)
                //    throw new Exception("User not found.");

                //int code = result.Column2 ?? (int)System.Web.Security.MembershipPasswordFormat.Clear;

                //switch (code)
                //{
                //    case 0:
                //        passwordFormat = System.Web.Security.MembershipPasswordFormat.Clear;
                //        break;
                //    default:
                //    case 1:
                //        passwordFormat = System.Web.Security.MembershipPasswordFormat.Hashed;
                //        break;
                //    case 2:
                //        passwordFormat = System.Web.Security.MembershipPasswordFormat.Encrypted;
                //        break;
                //}

                //return result.Column1;
                if (output.Any())
                {
                    aspnet_Membership_GetPassword_Result singleRow = output.SingleOrDefault();

                    int code = singleRow.Column2 ?? (int)System.Web.Security.MembershipPasswordFormat.Clear;

                    switch (code)
                    {
                        case 0:
                            passwordFormat = System.Web.Security.MembershipPasswordFormat.Clear;
                            break;
                        default:
                        case 1:
                            passwordFormat = System.Web.Security.MembershipPasswordFormat.Hashed;
                            break;
                        case 2:
                            passwordFormat = System.Web.Security.MembershipPasswordFormat.Encrypted;
                            break;
                    }

                    return singleRow.Column1;
                }
                passwordFormat = System.Web.Security.MembershipPasswordFormat.Hashed;
                return null;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }

        }

        public string GetPasswordWithFormat(string applicationName, string userName, bool updateLastLoginActivityDate, DateTime CurrentTimeUtc)
        {

            try
            {
                ICollection<aspnet_Membership_GetPasswordWithFormat_Result> result = null;
                int rtn = UnitOfWork.GetDbContext<AspNetDbEntities2>().ExecuteStoredProcedureOrSqlFunction("aspnet_Membership_GetPasswordWithFormat", out result, applicationName, userName, updateLastLoginActivityDate, CurrentTimeUtc);
                if (rtn != 0)
                    throw new Exception("Error");
                return result.SingleOrDefault()?.Column1;
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

        bool ChangePasswordQuestionAndAnswer(string applicationName, string userName, string newPasswordQuestion, string newPasswordAnswer);
        IEnumerable<aspnet_Membership> FindUsersByEmail(string applicationName, string emailToMatch, out int TotalRecords, int PageIndex = 1, int PageSize = 5);

        IEnumerable<aspnet_Membership> FindUsersByName(string applicationName, string userNameToMatch, out int TotalRecords, int PageIndex = 1, int PageSize = 5);

        IEnumerable<aspnet_Membership> GetAllUsers(string applicationName, out int TotalRecords, int PageIndex = 1, int PageSize = 5);

        int GetNumberOfUsersOnline(string applicationName, int MinutesSinceLastInActive, DateTime CurrentTimeUtc);

        string GetPassword(string applicationName, string userName, int maxInvalidPasswordAttempts, int PasswordAttemptWindow, DateTime CurrentTimeUtc, out System.Web.Security.MembershipPasswordFormat passwordFormat, string PasswordAnswer = null);

        string GetPasswordWithFormat(string applicationName, string userName, bool updateLastLoginActivityDate, DateTime CurrentTimeUtc);
    }
}