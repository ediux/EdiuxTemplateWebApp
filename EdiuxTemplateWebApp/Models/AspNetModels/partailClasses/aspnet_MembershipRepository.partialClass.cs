using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using EdiuxTemplateWebApp;
using EdiuxTemplateWebApp.Helpers;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_MembershipRepository : EFRepository<aspnet_Membership>, Iaspnet_MembershipRepository
    {
        public Task<aspnet_Membership> FindByEmailAsync(string email)
        {
            var db = UnitOfWork.GetAspNetMembershipDbContext();
            string appName = Startup.getApplicationNameFromConfiguationFile();

            var result = from membership in db.aspnet_Membership
                         where membership.aspnet_Applications.ApplicationName == appName &&
                         membership.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase)
                         select membership;

            return Task.FromResult(result.FirstOrDefault());
        }

        public System.Web.Security.MembershipCreateStatus CreateUser(string applicationName, string userName,
            string password, string passwordSalt, string eMail, out Guid userId, string passwordQuestion = "",
            string passwordAnswer = "", bool isApproved = false, bool uniqueEmail = false,
            System.Web.Security.MembershipPasswordFormat passwordFormat = System.Web.Security.MembershipPasswordFormat.Hashed)
        {
            try
            {
                aspnet_Applications app;

                try
                {
                    Iaspnet_ApplicationsRepository appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository(UnitOfWork);
                    app = appRepo.FindByName(applicationName);

                    if (app == null)
                    {
                        app = new aspnet_Applications();
                        app.ApplicationId = Guid.NewGuid();
                        app.ApplicationName = applicationName;
                        app.Description = string.Empty;
                        app.LoweredApplicationName = applicationName.ToLowerInvariant();

                        app = appRepo.Add(app);
                    }
                }
                catch
                {
                    throw;
                }

                DateTime CreateDate = DateTime.UtcNow;

                aspnet_Users user;

                try
                {
                    Iaspnet_UsersRepository userRepo = RepositoryHelper.Getaspnet_UsersRepository(UnitOfWork);
                    user = userRepo.FindByName(applicationName, userName);

                    if (user == null)
                    {
                        user = new aspnet_Users();
                        user.ApplicationId = app.ApplicationId;
                        user.Id = Guid.NewGuid();
                        user.IsAnonymous = false;
                        user.LastActivityDate = CreateDate;
                        user.LoweredUserName = userName.ToLowerInvariant();
                        user.MobileAlias = userName;
                        user.UserName = userName;

                        user = userRepo.Add(user);
                        userId = user.Id;
                    }
                    else
                    {
                        userId = user.Id;
                        return System.Web.Security.MembershipCreateStatus.DuplicateUserName;
                    }

                }
                catch
                {

                    throw;
                }

                aspnet_Membership membership;

                try
                {
                    int total;
                    membership = FindUsersByName(applicationName, userName, out total).FirstOrDefault();
                    if (membership == null)
                    {
                        membership = new aspnet_Membership();
                        membership.AccessFailedCount = 0;
                        membership.ApplicationId = app.ApplicationId;
                        membership.Comment = string.Empty;
                        membership.CreateDate = CreateDate;
                        membership.Email = eMail;
                        membership.EmailConfirmed = false;
                        membership.FailedPasswordAnswerAttemptCount = 0;
                        membership.FailedPasswordAnswerAttemptWindowStart = new DateTime(1754, 1, 1).ToUniversalTime();
                        membership.FailedPasswordAttemptCount = 0;
                        membership.FailedPasswordAttemptWindowStart = new DateTime(1754, 1, 1).ToUniversalTime();
                        membership.IsApproved = isApproved;
                        membership.IsLockedOut = false;
                        membership.LastActivityTime = null;
                        membership.LastLockoutDate = new DateTime(1754, 1, 1);
                        membership.LastLoginDate = new DateTime(1754, 1, 1);
                        membership.LastPasswordChangedDate = new DateTime(1754, 1, 1);
                        membership.LastUpdateTime = CreateDate;
                        membership.LoweredEmail = eMail.ToLowerInvariant();
                        membership.MobilePIN = string.Empty;
                        membership.Password = password;
                        membership.PasswordAnswer = passwordAnswer;
                        membership.PasswordFormat = (int)passwordFormat;
                        membership.PasswordQuestion = passwordQuestion;
                        membership.PasswordSalt = passwordSalt;
                        membership.PhoneConfirmed = true;
                        membership.PhoneNumber = "+88609";
                        byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
                        byte[] key = Guid.NewGuid().ToByteArray();
                        string token = Convert.ToBase64String(time.Concat(key).ToArray());
                        membership.ResetPasswordToken = token;
                        membership.UserId = user.Id;

                        membership = Add(membership);
                    }
                    else
                    {
                        userId = user.Id;
                        return System.Web.Security.MembershipCreateStatus.UserRejected;
                    }
                }
                catch
                {

                    throw;
                }

                if (uniqueEmail)
                {
                    if (Where(w => w.LoweredEmail == membership.LoweredEmail).Any())
                    {
                        return System.Web.Security.MembershipCreateStatus.DuplicateEmail;
                    }
                }

                UnitOfWork.Commit();

                return System.Web.Security.MembershipCreateStatus.Success;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                userId = Guid.Empty;
                return System.Web.Security.MembershipCreateStatus.ProviderError;
            }

        }

        public bool ChangePasswordQuestionAndAnswer(string applicationName, string userName, string newPasswordQuestion, string newPasswordAnswer)
        {

            try
            {
                return UnitOfWork.GetAspNetMembershipDbContext()
                    .aspnet_Membership_ChangePasswordQuestionAndAnswer(applicationName, userName, newPasswordQuestion, newPasswordAnswer);
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
                var result = from m in ObjectSet
                             from u in UnitOfWork.GetAspNetMembershipDbContext().aspnet_Users
                             where m.LoweredEmail.Equals(emailToMatch, StringComparison.InvariantCultureIgnoreCase)
                             && u.Id == m.UserId
                             && u.aspnet_Applications.LoweredApplicationName.Equals(applicationName, StringComparison.InvariantCultureIgnoreCase)
                             orderby u.UserName
                             select m;

                TotalRecords = result.Count();

                return result.Skip(PageSize * (PageIndex - 1)).Take(PageSize).AsEnumerable();


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
                var result = from m in ObjectSet
                             from u in UnitOfWork.GetAspNetMembershipDbContext().aspnet_Users
                             where u.UserName.Equals(userNameToMatch, StringComparison.InvariantCultureIgnoreCase)
                             && u.Id == m.UserId
                             && u.aspnet_Applications.LoweredApplicationName.Equals(applicationName, StringComparison.InvariantCultureIgnoreCase)
                             orderby u.UserName
                             select m;

                TotalRecords = result.Count();

                return result.Skip(PageSize * (PageIndex - 1)).Take(PageSize).AsEnumerable();
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
             int MinutesSinceLastInActive)
        {
            try
            {
                int numberOfUsers = UnitOfWork.GetAspNetMembershipDbContext().aspnet_Membership_GetNumberOfUsersOnline(applicationName, MinutesSinceLastInActive, DateTime.UtcNow);
                return numberOfUsers;
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

                var result = UnitOfWork.GetAspNetMembershipDbContext().aspnet_Membership_GetPassword(applicationName,
                    userName, maxInvalidPasswordAttempts, PasswordAttemptWindow, CurrentTimeUtc, PasswordAnswer, null);
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
            string passwordAnswer = "", bool isApproved = false, bool uniqueEmail = false,
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