using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Security;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class AspNetDbEntities2
    {
        #region aspnet_AnyDataInTables
        /// <summary>
        /// 檢查只指定資料表是否有資料?
        /// </summary>
        /// <param name="tableToCheck">會員資料表列舉</param>
        /// <returns>回傳哪個資料表有資料。</returns>
        [InputParameterMapping("aspnet_AnyDataInTables")]
        public virtual string aspnet_AnyDataInTables(TablesToCheck tableToCheck)
        {
#if STOREPRODUCE
         
          
            IEnumerable<string> result = null;

            int rtn; result = this.ExecuteStoredProcedure<string>(
                "aspnet_AnyDataInTables",
                out rtn,
               new { TablesToCheck = (int)tableToCheck });

            if (result != null && result.Count() > 0)
            {
                return result.Single();
            }

            return null;
#else
            switch (tableToCheck)
            {
                case TablesToCheck.aspnet_Membership:
                    if (aspnet_Membership.Any())
                        return nameof(aspnet_Membership);
                    break;
                case TablesToCheck.aspnet_PersonalizationPerUser:
                    if (aspnet_PersonalizationPerUser.Any())
                        return nameof(aspnet_PersonalizationPerUser);
                    break;
                case TablesToCheck.aspnet_Profile:
                    if (aspnet_Profile.Any())
                        return nameof(aspnet_Profile);
                    break;
                case TablesToCheck.aspnet_Roles:
                    if (aspnet_Roles.Any())
                        return nameof(aspnet_Roles);
                    break;
                case TablesToCheck.aspnet_WebEvent_Events:
                    if (aspnet_WebEvent_Events.Any())
                        return nameof(aspnet_WebEvent_Events);
                    break;
                default:
                    if ((tableToCheck & TablesToCheck.aspnet_Membership) != 0 &&
                        (tableToCheck & TablesToCheck.aspnet_Roles) != 0 &&
                        (tableToCheck & TablesToCheck.aspnet_Profile) != 0 &&
                        (tableToCheck & TablesToCheck.aspnet_PersonalizationPerUser) != 0 &&
                        ((int)tableToCheck & 32) != 0 &&
                        ((int)tableToCheck & 128) != 0 &&
                        ((int)tableToCheck & 256) != 0 &&
                        ((int)tableToCheck & 512) != 0 &&
                        ((int)tableToCheck & 1024) != 0)
                    {
                        if (aspnet_Users.Any())
                            return nameof(aspnet_Users);

                        if (aspnet_Applications.Any())
                            return nameof(aspnet_Applications);
                    }

                    break;
            }

            return string.Empty;
#endif
        }
        #endregion

        #region aspnet_Applications_CreateApplication
        /// <summary>
        /// 建立應用程式資訊
        /// </summary>
        /// <param name="applicationName">應用名稱</param>
        /// <returns></returns>
        public virtual Guid aspnet_Applications_CreateApplication(string applicationName)
        {
#if STOREPRODUCE
            
            aspnet_Applications_CreateApplication_Result result;
            int rtn = 0;
            this.ExecuteStoredProcedure(
                "aspnet_Applications_CreateApplication",
                out result, 
                out rtn, 
                new { ApplicationName = applicationName } );
            return (result == null) ? Guid.Empty : result.ApplicationId;
#else
            string loweredApplicationName = applicationName.ToLowerInvariant();
            IQueryable<Guid> idResult = from app in aspnet_Applications
                                        where app.LoweredApplicationName == loweredApplicationName
                                        select app.ApplicationId;

            if (!idResult.Any())
            {
                aspnet_Applications newApp = new aspnet_Applications();

                newApp.ApplicationId = Guid.NewGuid();
                newApp.ApplicationName = applicationName;
                newApp.LoweredApplicationName = loweredApplicationName;

                aspnet_Applications.Add(newApp);
                SaveChanges();
                Entry(newApp).Reload();

                return newApp.ApplicationId;
            }

            return idResult.SingleOrDefault();
#endif
        }
        #endregion

        #region aspnet_CheckSchemaVersion
        /// <summary>
        /// 檢查結構描述版本號
        /// </summary>
        /// <param name="Feature">功能名稱</param>
        /// <param name="CompatibleSchemaVersion">相容結構版本號</param>
        /// <returns></returns>
        public virtual int aspnet_CheckSchemaVersion(string Feature, string CompatibleSchemaVersion)
        {
#if STOREPRODUCE
            return this.ExecuteStoredProcedure(
                "aspnet_CheckSchemaVersion",
                new { Feature = Feature, CompatibleSchemaVersion = CompatibleSchemaVersion }
                
                );
#else
            string loweredFeature = Feature.ToLowerInvariant();
            return (from q in aspnet_SchemaVersions
                    where q.Feature.Equals(Feature, StringComparison.InvariantCultureIgnoreCase)
                    && CompatibleSchemaVersion == q.CompatibleSchemaVersion
                    select q).Any() ? 0 : 1;
#endif

        }
        #endregion

        #region aspnet_Membership_ChangePasswordQuestionAndAnswer
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="userName"></param>
        /// <param name="NewPasswordQuestion"></param>
        /// <param name="NewPasswordAnswer"></param>
        /// <returns></returns>
        public virtual bool aspnet_Membership_ChangePasswordQuestionAndAnswer(string applicationName, string userName, string NewPasswordQuestion, string NewPasswordAnswer)
        {
#if STOREPRODUCE
            int code = this.ExecuteStoredProcedure(
                "aspnet_Membership_ChangePasswordQuestionAndAnswer",
                new { ApplicationName= applicationName, UserName = userName, NewPasswordQuestion, NewPasswordAnswer }
                );

            if (code == 1)
                return false;

            return true;
#else
            string loweredAppName = applicationName.ToLowerInvariant();
            string loweredUserName = userName.ToLowerInvariant();
            IQueryable<aspnet_Users> UserId = from u in aspnet_Users
                                              where loweredAppName == u.LoweredUserName &&
                                              u.aspnet_Applications.LoweredApplicationName == loweredUserName
                                              select u;

            if (!UserId.Any())
                return false;

            aspnet_Users foundUser = UserId.Single();

            foundUser.aspnet_Membership.PasswordQuestion = NewPasswordQuestion;
            foundUser.aspnet_Membership.PasswordAnswer = NewPasswordAnswer;

            SaveChanges();

            return true;
#endif
        }
        #endregion

        #region aspnet_Membership_CreateUser
        public virtual MembershipCreateStatus aspnet_Membership_CreateUser(string applicationName, string userName,
    string password, string passwordSalt, string eMail, DateTime CurrentTimeUtc, DateTime CreateDate, out Guid userId, string passwordQuestion = "",
    string passwordAnswer = "", bool isApproved = false, int uniqueEmail = 0,
    MembershipPasswordFormat passwordFormat = MembershipPasswordFormat.Clear)
        {
            try
            {
                aspnet_Membership_CreateUser_OutputParameter userKey = new aspnet_Membership_CreateUser_OutputParameter();
                userId = Guid.Empty;

                int code = -1;

                this.ExecuteStoredProcedure("aspnet_Membership_CreateUser",
                        out userKey,
                        out code,
                        applicationName,
                        userName,
                        password,
                        passwordSalt,
                        eMail,
                        passwordQuestion,
                        passwordAnswer,
                        isApproved,
                        CurrentTimeUtc,
                        CreateDate,
                        uniqueEmail,
                        (int)passwordFormat);

                switch (code)
                {
                    case -1:
                        return MembershipCreateStatus.ProviderError;
                    case 0:
                        return MembershipCreateStatus.Success;
                    case 6:
                        return MembershipCreateStatus.DuplicateProviderUserKey;
                    case 7:
                        return MembershipCreateStatus.DuplicateEmail;
                    case 10:
                        return MembershipCreateStatus.DuplicateUserName;
                    default:
                        return MembershipCreateStatus.UserRejected;
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region aspnet_Membership_FindUsersByEmail
        public virtual PagedList<aspnet_Membership_FindUsersByEmailResult> aspnet_Membership_FindUsersByEmail(string applicationName, string emailToMatch, int pageIndex = 1, int pageSize = 5)
        {
            IEnumerable<aspnet_Membership_FindUsersByEmailResult> result;
            int TotalRecords = 0;

            result = this.ExecuteStoredProcedure<aspnet_Membership_FindUsersByEmailResult>(
               "aspnet_Membership_FindUsersByEmail", out TotalRecords,
               applicationName, emailToMatch, pageIndex, pageSize);

            return new MyPagedList<aspnet_Membership_FindUsersByEmailResult>(result.ToList(),
                pageIndex, pageSize, TotalRecords);
        }
        #endregion

        #region aspnet_Membership_FindUsersByName
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="usernameToMatch"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual PagedList<aspnet_Membership_FindUsersByNameResult> aspnet_Membership_FindUsersByName(string applicationName, string usernameToMatch, int pageIndex = 1, int pageSize = 5)
        {
            IEnumerable<aspnet_Membership_FindUsersByNameResult> result;

            int TotalRecords;
            result = this.ExecuteStoredProcedure<aspnet_Membership_FindUsersByNameResult>(
                 "aspnet_Membership_FindUsersByName",
                 out TotalRecords, applicationName, usernameToMatch, pageIndex, pageSize);

            return new MyPagedList<aspnet_Membership_FindUsersByNameResult>(result,
                pageIndex, pageSize, TotalRecords);
        }
        #endregion

        #region aspnet_Membership_GetAllUsers
        /// <summary>
        /// 取得應用程式的使用者。
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual PagedList<aspnet_Membership_GetAllUsersResult> aspnet_Membership_GetAllUsers(string applicationName, int pageIndex = 1, int pageSize = 5)
        {
            int TotalRecords = 0;

            IEnumerable<aspnet_Membership_GetAllUsersResult> Members = this.ExecuteStoredProcedure<aspnet_Membership_GetAllUsersResult>(
                "aspnet_Membership_GetAllUsers",
                out TotalRecords,
                applicationName,
                pageIndex,
                pageSize);

            return new MyPagedList<aspnet_Membership_GetAllUsersResult>(Members,
                pageIndex, pageSize == -1 ? TotalRecords : pageSize, TotalRecords);
        }
        #endregion

        #region aspnet_Membership_GetNumberOfUsersOnline
        /// <summary>
        /// 取得指定應用程式下的線上人數。
        /// </summary>
        /// <param name="applicationName">應用程式名稱</param>
        /// <param name="MinutesSinceLastInActive">自上次活動時間以來的間隔時間區間(分鐘)</param>
        /// <param name="CurrentTimeUtc">目前系統的格林威治時間。</param>
        /// <returns></returns>
        public virtual int aspnet_Membership_GetNumberOfUsersOnline(string applicationName, int MinutesSinceLastInActive, DateTime CurrentTimeUtc)
        {
            return this.ExecuteStoredProcedure(
                    "aspnet_Membership_GetNumberOfUsersOnline",
                     applicationName, MinutesSinceLastInActive, CurrentTimeUtc);
        }
        #endregion

        #region aspnet_Membership_GetPassword
        /// <summary>
        /// Aspnets the membership get password.
        /// </summary>
        /// <returns>The membership get password.</returns>
        /// <param name="applicationName">Application name.</param>
        /// <param name="userName">User name.</param>
        /// <param name="maxInvalidPasswordAttempts">Max invalid password attempts.</param>
        /// <param name="passwordAttemptWindow">Password attempt window.</param>
        /// <param name="currentTimeUtc">Current time UTC.</param>
        /// <param name="passwordAnswer">Password answer.</param>
        public virtual aspnet_Membership_GetPasswordResult aspnet_Membership_GetPassword(string applicationName, string userName, int maxInvalidPasswordAttempts, int passwordAttemptWindow, DateTime currentTimeUtc, string passwordAnswer = null)
        {
            IEnumerable<aspnet_Membership_GetPasswordResult> result = null;

            int rtn;
            result = this.ExecuteStoredProcedure<aspnet_Membership_GetPasswordResult>(
                "aspnet_Membership_GetPassword",
                out rtn,
                applicationName,
                userName,
                maxInvalidPasswordAttempts,
                passwordAttemptWindow,
                currentTimeUtc,
                passwordAnswer);

            return result.SingleOrDefault();

        }
        #endregion

        #region aspnet_Membership_GetPasswordWithFormat
        /// <summary>
        /// Aspnets the membership get password with format.
        /// </summary>
        /// <returns>The membership get password with format.</returns>
        /// <param name="applicationName">Application name.</param>
        /// <param name="userName">User name.</param>
        /// <param name="updateLastLoginActivityDate">If set to <c>true</c> update last login activity date.</param>
        /// <param name="currentTimeUtc">Current time UTC.</param>
        public virtual aspnet_Membership_GetPasswordWithFormatResult aspnet_Membership_GetPasswordWithFormat(string applicationName, string userName, bool updateLastLoginActivityDate, DateTime currentTimeUtc)
        {
            IEnumerable<aspnet_Membership_GetPasswordWithFormatResult> result = null;
            int rtn;
            result = this.ExecuteStoredProcedure<aspnet_Membership_GetPasswordWithFormatResult>(
                 "aspnet_Membership_GetPasswordWithFormat",
                 out rtn,
                 applicationName,
                 userName,
                 updateLastLoginActivityDate,
                 currentTimeUtc);
            if (rtn == 1)
            {
                throw new Exception(string.Format("Error:{0}", rtn));
            }
            return result.SingleOrDefault();
        }
        #endregion

        #region aspnet_Membership_GetUserByEmail
        /// <summary>
        /// Aspnets the membership get user by email.
        /// </summary>
        /// <returns>The membership get user by email.</returns>
        /// <param name="applicationName">Application name.</param>
        /// <param name="email">Email.</param>
        public virtual aspnet_Membership_GetUserByEmail_Result aspnet_Membership_GetUserByEmail(string applicationName, string email)
        {
            IEnumerable<aspnet_Membership_GetUserByEmail_Result> result = null;
            int rtn;
            result = this.ExecuteStoredProcedure<aspnet_Membership_GetUserByEmail_Result>(
               "aspnet_Membership_GetUserByName",
               out rtn,
               applicationName,
               email);

            if (result?.Count() > 0)
            {
                return result.SingleOrDefault();
            }

            return null;
        }
        #endregion

        #region aspnet_Membership_GetUserByName
        /// <summary>
        /// Aspnets the name of the membership get user by.
        /// </summary>
        /// <returns>The membership get user by name.</returns>
        /// <param name="applicationName">Application name.</param>
        /// <param name="userName">User name.</param>
        /// <param name="currentTimeUtc">Current time UTC.</param>
        /// <param name="updateLastActivity">If set to <c>true</c> update last activity.</param>
        public virtual aspnet_Membership_GetUserByName_Result aspnet_Membership_GetUserByName(string applicationName, string userName, DateTime currentTimeUtc, bool updateLastActivity)
        {
            IEnumerable<aspnet_Membership_GetUserByName_Result> result = null;
            int rtn;
            result = this.ExecuteStoredProcedure<aspnet_Membership_GetUserByName_Result>(
                "aspnet_Membership_GetUserByName",
                out rtn,
                applicationName,
                userName,
                currentTimeUtc,
                updateLastActivity);

            if (rtn == 1)
            {
                throw new Exception(string.Format("Error:{0}", rtn));
            }
            return result.SingleOrDefault();
        }
        #endregion

        #region aspnet_Membership_GetUserByUserId
        /// <summary>
        /// Aspnets the membership get user by user identifier.
        /// </summary>
        /// <returns>The membership get user by user identifier.</returns>
        /// <param name="UserId">User identifier.</param>
        /// <param name="CurrentTimeUtc">Current time UTC.</param>
        /// <param name="UpdateLastActivity">If set to <c>true</c> update last activity.</param>
        public virtual aspnet_Membership_GetUserByUserId_Result aspnet_Membership_GetUserByUserId(Guid UserId, DateTime CurrentTimeUtc, bool UpdateLastActivity = false)
        {
            IEnumerable<aspnet_Membership_GetUserByUserId_Result> result = null;
            int rtn;
            result = this.ExecuteStoredProcedure<aspnet_Membership_GetUserByUserId_Result>(
                 "aspnet_Membership_GetUserByName",
                 out rtn,
                 UserId,
                 CurrentTimeUtc,
                 UpdateLastActivity);

            if (rtn == 1)
            {
                throw new Exception(string.Format("Error:{0}", rtn));
            }

            return result.SingleOrDefault();
        }
        #endregion

        #region aspnet_Membership_ResetPassword
        /// <summary>
        /// Aspnets the membership reset password.
        /// </summary>
        /// <returns>The membership reset password.</returns>
        /// <param name="applicationName">Application name.</param>
        /// <param name="userName">User name.</param>
        /// <param name="newPassword">New password.</param>
        /// <param name="maxInvalidPasswordAttempts">Max invalid password attempts.</param>
        /// <param name="passwordAttemptWindow">Password attempt window.</param>
        /// <param name="passwordSalt">Password salt.</param>
        /// <param name="currentTimeUtc">Current time UTC.</param>
        /// <param name="passwordFormat">Password format.</param>
        /// <param name="passwordAnswer">Password answer.</param>
        public virtual int aspnet_Membership_ResetPassword(string applicationName, string userName, string newPassword,
            int maxInvalidPasswordAttempts, int passwordAttemptWindow, string passwordSalt,
            DateTime currentTimeUtc, MembershipPasswordFormat passwordFormat = MembershipPasswordFormat.Clear,
            string passwordAnswer = null)
        {
            int rtn = this.ExecuteStoredProcedure(
                "aspnet_Membership_ResetPassword",
                applicationName,
                userName,
                newPassword,
                maxInvalidPasswordAttempts,
                passwordAttemptWindow,
                passwordSalt,
                currentTimeUtc,
                (int)passwordFormat,
                passwordAnswer);

            return rtn;
        }
        #endregion

        #region aspnet_Membership_SetPassword
        /// <summary>
        /// 設定使用者密碼.
        /// </summary>
        /// <returns>The membership set password.</returns>
        /// <param name="applicationName">Application name.</param>
        /// <param name="userName">User name.</param>
        /// <param name="newPassword">New password.</param>
        /// <param name="passwordSalt">Password salt.</param>
        /// <param name="currentTimeUtc">Current time UTC.</param>
        /// <param name="passwordFormat">Password format.</param>
        public virtual int aspnet_Membership_SetPassword(string applicationName, string userName, string newPassword, string passwordSalt,
            DateTime currentTimeUtc, MembershipPasswordFormat passwordFormat = MembershipPasswordFormat.Clear)
        {
            int rtn = this.ExecuteStoredProcedure(
                "aspnet_Membership_SetPassword",
                applicationName,
                userName,
                newPassword,
                passwordSalt,
                currentTimeUtc,
                (int)passwordFormat);

            return rtn;
        }
        #endregion

        #region aspnet_Membership_UnlockUser
        /// <summary>
        /// Aspnets the membership unlock user.
        /// </summary>
        /// <returns>The membership unlock user.</returns>
        /// <param name="applicationName">Application name.</param>
        /// <param name="userName">User name.</param>
        public virtual int aspnet_Membership_UnlockUser(string applicationName, string userName)
        {
            int rtn = this.ExecuteStoredProcedure(
                "aspnet_Membership_UnlockUser",
                applicationName,
                userName);

            return rtn;
        }
        #endregion

        #region aspnet_Membership_UpdateUser
        public virtual int aspnet_Membership_UpdateUser(string applicationName, string userName,
            string email, string comment, bool isApproved, DateTime LastLoginDate, DateTime LastActivityDate,
            int UniqueEmail, DateTime currentTimeUtc)
        {
            //ApplicationName,UserName,Email,Comment,IsApproved,LastLoginDate,LastActivityDate,UniqueEmail,CurrentTimeUtc
            int rtn = this.ExecuteStoredProcedure(
                "aspnet_Membership_UpdateUser",
                applicationName,
                userName,
                email,
                comment,
                isApproved,
                LastLoginDate,
                LastActivityDate,
                UniqueEmail,
                currentTimeUtc);

            return rtn;
        }
        #endregion

        #region aspnet_Membership_UpdateUserInfo
        /// <summary>
        /// Aspnets the membership update user info.
        /// </summary>
        /// <returns>The membership update user info.</returns>
        /// <param name="applicationName">Application name.</param>
        /// <param name="userName">User name.</param>
        /// <param name="isPasswordCorrect">If set to <c>true</c> is password correct.</param>
        /// <param name="updateLastLoginActivityDate">If set to <c>true</c> update last login activity date.</param>
        /// <param name="maxInvalidPasswordAttempts">Max invalid password attempts.</param>
        /// <param name="passwordAttemptWindow">Password attempt window.</param>
        /// <param name="currentTimeUtc">Current time UTC.</param>
        /// <param name="lastLoginDate">Last login date.</param>
        /// <param name="lastActivityDate">Last activity date.</param>
        public virtual int aspnet_Membership_UpdateUserInfo(string applicationName, string userName,
                  bool isPasswordCorrect, bool updateLastLoginActivityDate, int maxInvalidPasswordAttempts,
                  int passwordAttemptWindow, DateTime currentTimeUtc, DateTime lastLoginDate,
                  DateTime lastActivityDate)
        {
            int rtn = this.ExecuteStoredProcedure(
                "aspnet_Membership_UpdateUser",
                applicationName,
                userName,
              isPasswordCorrect,
              updateLastLoginActivityDate,
              maxInvalidPasswordAttempts,
              passwordAttemptWindow,
              currentTimeUtc,
              lastLoginDate,
              lastActivityDate
          );

            return rtn;
        }
        #endregion

        #region aspnet_Paths_CreatePath
        /// <summary>
        /// Aspnets the paths create path.
        /// </summary>
        /// <returns>The paths create path.</returns>
        /// <param name="applicationId">Application identifier.</param>
        /// <param name="path">Path.</param>
        /// <param name="pathId">Path identifier.</param>
        public virtual int aspnet_Paths_CreatePath(Guid applicationId, string path, out Guid pathId)
        {
            pathId = Guid.Empty;
            int rtn;
            this.ExecuteStoredProcedure(
                   "aspnet_Paths_CreatePath",
                   out pathId,
                   out rtn,
                   applicationId,
                   path);

            return rtn;
        }
        #endregion

        #region aspnet_Personalization_GetApplicationId
        /// <summary>
        /// Aspnets the personalization get application identifier.
        /// </summary>
        /// <returns>The personalization get application identifier.</returns>
        /// <param name="applicationName">Application name.</param>
        public virtual Guid aspnet_Personalization_GetApplicationId(string applicationName)
        {
            Guid applicationId = Guid.Empty;
            int rtn;
            this.ExecuteStoredProcedure(
                "aspnet_Personalization_GetApplicationId",
                out applicationId,
                out rtn,
                applicationName);
            return applicationId;
        }
        #endregion

        #region aspnet_PersonalizationAdministration_DeleteAllState
        public virtual int aspnet_PersonalizationAdministration_DeleteAllState(bool allUsersScope, string applicationName)
        {
            aspnet_PersonalizationAdministration_DeleteAllState_Result Count = null;
            int rtn;
            this.ExecuteStoredProcedure(
                "aspnet_PersonalizationAdministration_DeleteAllState",
                out Count,
                out rtn,
                allUsersScope,
                applicationName
            );
            return Count.Count;
        }
        #endregion

        #region aspnet_PersonalizationAdministration_FindState
        /// <summary>
        /// 
        /// </summary>
        /// <param name="allUsersScope"></param>
        /// <param name="appliactionName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="Path"></param>
        /// <param name="userName"></param>
        /// <param name="InactiveSinceDate"></param>
        /// <returns></returns>
        public PagedList<aspnet_PersonalizationAdministration_FindState_Result> aspnet_PersonalizationAdministration_FindState(bool allUsersScope, string appliactionName, int pageIndex, int pageSize, string Path = null, string userName = null, DateTime InactiveSinceDate = default(DateTime))
        {
            IEnumerable<aspnet_PersonalizationAdministration_FindState_Result> result;
            int rtn;
            result = this.ExecuteStoredProcedure<aspnet_PersonalizationAdministration_FindState_Result>(
                "aspnet_PersonalizationAdministration_FindState",
                out rtn,
                allUsersScope,
                appliactionName,
                pageIndex,
                pageSize,
                Path,
                userName,
                InactiveSinceDate);
            return new MyPagedList<aspnet_PersonalizationAdministration_FindState_Result>(result, pageIndex, pageSize, rtn);
        }
        #endregion

        #region aspnet_PersonalizationAdministration_GetCountOfState
        /// <summary>
        /// 
        /// </summary>
        /// <param name="allUsersScope"></param>
        /// <param name="applicationName"></param>
        /// <param name="path"></param>
        /// <param name="userName"></param>
        /// <param name="InactiveSinceDate"></param>
        /// <returns></returns>
        public int aspnet_PersonalizationAdministration_GetCountOfState(bool allUsersScope, string applicationName, string path = null, string userName = null, DateTime? InactiveSinceDate = default(DateTime?))
        {
            int result = 0;
            int rtn;
            this.ExecuteStoredProcedure(
                      "aspnet_PersonalizationAdministration_GetCountOfState",
                      out result,
                      out rtn,
                      allUsersScope,
                      applicationName,
                      path,
                      userName);

            return result;
        }
        #endregion

        #region aspnet_PersonalizationAdministration_ResetSharedState
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public int aspnet_PersonalizationAdministration_ResetSharedState(string applicationName, string path)
        {
            int result = 0;
            int rtn;
            this.ExecuteStoredProcedure(
              "aspnet_PersonalizationAdministration_ResetSharedState",
              out result,
              out rtn,
              applicationName,
              path);
            return result;
        }
        #endregion

        #region aspnet_PersonalizationAdministration_ResetUserState
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="InactiveSinceDate"></param>
        /// <param name="userName"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public int aspnet_PersonalizationAdministration_ResetUserState(string applicationName, DateTime InactiveSinceDate = default(DateTime), string userName = null, string path = null)
        {
            int result = 0;
            int rtn;
            this.ExecuteStoredProcedure(
                "aspnet_PersonalizationAdministration_ResetUserState",
                out result,
                out rtn,
                applicationName,
                InactiveSinceDate,
                userName,
                path);
            return result;
        }
        #endregion

        #region aspnet_PersonalizationAllUsers_GetPageSettings
        public aspnet_PersonalizationAllUsers_GetPageSettings_Result aspnet_PersonalizationAllUsers_GetPageSettings(string ApplicationName, string Path)
        {
            IEnumerable<aspnet_PersonalizationAllUsers_GetPageSettings_Result> result = null;
            int rtn;
            result = this.ExecuteStoredProcedure<aspnet_PersonalizationAllUsers_GetPageSettings_Result>("aspnet_PersonalizationAllUsers_GetPageSettings", out rtn, ApplicationName, Path);
            if (result != null && result.Count() > 0)
            {
                return result.SingleOrDefault();
            }
            return null;
        }
        #endregion

        #region aspnet_PersonalizationAllUsers_ResetPageSettings
        public bool aspnet_PersonalizationAllUsers_ResetPageSettings(string ApplicationName, string Path)
        {
            int rtn = this.ExecuteStoredProcedure(
                "aspnet_PersonalizationAllUsers_ResetPageSettings",
                ApplicationName,
                Path);

            return rtn == 0;
        }
        #endregion

        #region aspnet_PersonalizationAllUsers_SetPageSettings
        public void aspnet_PersonalizationAllUsers_SetPageSettings(string applicationName, string path, byte[] pageSettings, DateTime currentTimeUtc)
        {
            int rtn = this.ExecuteStoredProcedure(
                "aspnet_PersonalizationAllUsers_SetPageSettings",
                applicationName,
                path,
                pageSettings,
                currentTimeUtc);
        }
        #endregion

        #region aspnet_PersonalizationPerUser_GetPageSettings
        public IEnumerable<aspnet_PersonalizationPerUser_GetPageSettings_Result> aspnet_PersonalizationPerUser_GetPageSettings(string applicationName, string userName, string path, DateTime currentTimeUtc)
        {
            int rtn;
            IEnumerable<aspnet_PersonalizationPerUser_GetPageSettings_Result> result =
                this.ExecuteStoredProcedure<aspnet_PersonalizationPerUser_GetPageSettings_Result>(
                    "aspnet_PersonalizationPerUser_GetPageSettings",
                    out rtn,
                    applicationName,
                    userName,
                    path,
                    currentTimeUtc
                    );
            return result;
        }
        #endregion

        #region aspnet_PersonalizationPerUser_ResetPageSettings
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="userName"></param>
        /// <param name="path"></param>
        /// <param name="currentTimeUtc"></param>
        public void aspnet_PersonalizationPerUser_ResetPageSettings(string applicationName, string userName, string path, DateTime currentTimeUtc)
        {
            int rtn =
                this.ExecuteStoredProcedure("aspnet_PersonalizationPerUser_ResetPageSettings",
                applicationName,
                userName,
                path,
                currentTimeUtc);
        }
        #endregion

        #region aspnet_PersonalizationPerUser_SetPageSettings
        public void aspnet_PersonalizationPerUser_SetPageSettings(string applicationName,
            string userName,
            string path,
            byte[] PageSettings,
            DateTime currentTimeUtc)
        {
            int rtn =
                this.ExecuteStoredProcedure(
                    "aspnet_PersonalizationPerUser_SetPageSettings",
                    applicationName,
                    userName,
                    path,
                    PageSettings,
                    currentTimeUtc
                    );
        }
        #endregion

        #region aspnet_Profile_DeleteInactiveProfiles
        public void aspnet_Profile_DeleteInactiveProfiles(string applicationName,
            int profileAuthOptions,
            DateTime inactiveSinceDate)
        {
            this.ExecuteStoredProcedure(
                "aspnet_Profile_DeleteInactiveProfiles",
                applicationName,
                profileAuthOptions,
                inactiveSinceDate);
        }
        #endregion

        #region aspnet_Profile_DeleteProfiles
        public int aspnet_Profile_DeleteProfiles(string applicationName, string userNames)
        {
            int rtn;
            IEnumerable<aspnet_Profile_DeleteProfiles_Result> result
                = this.ExecuteStoredProcedure<aspnet_Profile_DeleteProfiles_Result>(
                    "aspnet_Profile_DeleteProfiles",
                    out rtn,
                    applicationName,
                    userNames);
            if (result != null && result.Any())
                return result.Single().Column1;

            return 0;
        }
        #endregion

        #region aspnet_Profile_GetProfiles
        public PagedList<aspnet_Profile_GetProfiles_Result> aspnet_Profile_GetProfiles(string applicationName, int profileAuthOptions, int pageIndex, int pageSize, string userNameToMatch = null, DateTime inactiveSinceDate = default(DateTime))
        {
            int totalRecord;
            int rtn;
            IEnumerable<aspnet_Profile_GetProfiles_Result> result =
                this.ExecuteStoredProcedure<aspnet_Profile_GetProfiles_Result, int>(
                    "aspnet_Profile_GetProfiles",
                    out totalRecord,
                    out rtn,
                    applicationName,
                    profileAuthOptions,
                    pageIndex,
                    pageSize,
                    userNameToMatch,
                    inactiveSinceDate);

            return new MyPagedList<aspnet_Profile_GetProfiles_Result>(result,
                pageIndex, pageSize, totalRecord);

        }
        #endregion

        #region aspnet_Profile_GetProperties
        public IEnumerable<aspnet_Profile_GetProperties_Result> aspnet_Profile_GetProperties(string applicationName, string userName, DateTime currentTimeUtc)
        {
            int rtn;
            IEnumerable<aspnet_Profile_GetProperties_Result> result
                = this.ExecuteStoredProcedure<aspnet_Profile_GetProperties_Result>(
                    "aspnet_Profile_GetProperties",
                    out rtn,
                    applicationName,
                    userName,
                    currentTimeUtc);
            return result;
        }
        #endregion

        #region aspnet_Profile_SetProperties
        public int aspnet_Profile_SetProperties(string applicationName,
            string propertyNames,
            string propertyValuesString,
            byte[] propertyValuesBinary,
            string userName,
            bool isUserAnonymous,
            DateTime currentTimeUtc)
        {
            int rtn = this.ExecuteStoredProcedure("aspnet_Profile_SetProperties",
                applicationName,
                propertyNames,
                propertyValuesString,
                propertyValuesBinary,
                userName,
                isUserAnonymous,
                currentTimeUtc);
            return rtn;
        }
        #endregion

        #region aspnet_RegisterSchemaVersion
        public void aspnet_RegisterSchemaVersion(string feature, string compatibleSchemaVersion,
            bool isCurrentVersion,
            bool removeIncompatibleSchema)
        {
            this.ExecuteStoredProcedure(
                "aspnet_RegisterSchemaVersion",
                feature,
                compatibleSchemaVersion,
                isCurrentVersion,
                removeIncompatibleSchema);
        }
        #endregion

        #region aspnet_Roles_CreateRole
        public void aspnet_Roles_CreateRole(string applicationName, string roleName)
        {
            int rtn = this.ExecuteStoredProcedure("aspnet_Roles_CreateRole",
                applicationName,
                roleName);

            if (rtn != 0)
                throw new Exception("Error.");
        }
        #endregion

        #region aspnet_Roles_DeleteRole
        /// <summary>
        /// 刪除指定應用程式的角色
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="roleName"></param>
        /// <param name="deleteOnlyIfRoleIsEmpty"></param>
        /// <returns></returns>
        public int aspnet_Roles_DeleteRole(string applicationName, string roleName, bool deleteOnlyIfRoleIsEmpty)
        {
            int rtn = this.ExecuteStoredProcedure(
                "aspnet_Roles_DeleteRole",
                applicationName,
                roleName,
                deleteOnlyIfRoleIsEmpty);
            return rtn;
        }
        #endregion

        #region aspnet_Roles_GetAllRoles
        /// <summary>
        /// 取得指定應用程式的全部角色。
        /// </summary>
        /// <param name="applicationName"></param>
        /// <returns></returns>
        public IEnumerable<aspnet_Roles_GetAllRoles_Result> aspnet_Roles_GetAllRoles(string applicationName)
        {
            IEnumerable<aspnet_Roles_GetAllRoles_Result> result = null;
            int rtn = 0;
            result = this.ExecuteStoredProcedure<aspnet_Roles_GetAllRoles_Result>(
                    "aspnet_Roles_GetAllRoles",
                    out rtn,
                    applicationName);
            return result.AsEnumerable();
        }
        #endregion

        #region aspnet_Roles_RoleExists
        /// <summary>
        /// 指出某個應用程式角色是否存在?
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public bool aspnet_Roles_RoleExists(string applicationName, string roleName)
        {

#if STOREPRODUCE
            int rtn = this.ExecuteStoredProcedure(
        "aspnet_Roles_RoleExists",
        applicationName,
        roleName);

            if (rtn == 1)
                return true;
            else
                return false;
#endif
        }
        #endregion

        #region aspnet_Users_CreateUser
        public int aspnet_Users_CreateUser(Guid applicationId, string userName, bool isUserAnonymous, DateTime lastActivityDate, out Guid userId)
        {
#if STOREPRODUCE
            userId = Guid.Empty;

            int rtn; this.ExecuteStoredProcedure(
                "aspnet_Users_CreateUser",
                out userId,
                out rtn,
                applicationId,
                userName,
                isUserAnonymous,
                lastActivityDate);

            return rtn;
#else
            Guid UserId = Guid.NewGuid();
            IQueryable<aspnet_Users> result = from u in aspnet_Users
                                              where u.Id == UserId
                                              select u;

            if (result.Any())
            {
                userId = UserId;
                return -1;
            }

            IQueryable<aspnet_VoidUsers> voidresult = from voidu in aspnet_VoidUsers
                                                      where voidu.LoweredUserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
                                                      && voidu.ApplicationId == applicationId
                                                      select voidu;

            if (voidresult.Any())
            {
                aspnet_VoidUsers voidUser = voidresult.Single();
                aspnet_Users newUser = new AspNetModels.aspnet_Users();

                newUser.ApplicationId = voidUser.ApplicationId;
                newUser.Id = voidUser.UserId;
                newUser.IsAnonymous = isUserAnonymous;
                newUser.LastActivityDate = lastActivityDate;
                newUser.LoweredUserName = voidUser.LoweredUserName;
                newUser.MobileAlias = voidUser.MobileAlias;
                newUser.UserName = voidUser.UserName;

                aspnet_Users.Add(newUser);
                aspnet_VoidUsers.Remove(voidUser);

            }
            else
            {
                aspnet_Users allnewUser = new AspNetModels.aspnet_Users();
                allnewUser.ApplicationId = applicationId;
                allnewUser.Id = UserId;
                allnewUser.IsAnonymous = isUserAnonymous;
                allnewUser.LastActivityDate = lastActivityDate;
                allnewUser.LoweredUserName = userName.ToLowerInvariant();
                allnewUser.MobileAlias = string.Empty;
                allnewUser.UserName = userName;

                aspnet_Users.Add(allnewUser);
            }

            SaveChanges();
            userId = UserId;

            return 0;
#endif
        }
        #endregion

        #region aspnet_Users_DeleteUser
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="userName"></param>
        /// <param name="tablesToDeleteFrom"></param>
        /// <param name="numTablesDeletedFrom"></param>
        /// <returns></returns>
        public int aspnet_Users_DeleteUser(string applicationName, string userName, TablesToCheck tablesToDeleteFrom, out int numTablesDeletedFrom)
        {
#if STOREPRODUCE
            IEnumerable<int> _NumTablesDeletedFrom = null;
            int rtn; _NumTablesDeletedFrom = this.ExecuteStoredProcedure<int>("aspnet_Users_DeleteUser", out rtn, applicationName, userName, (int)tablesToDeleteFrom);
            numTablesDeletedFrom = _NumTablesDeletedFrom.SingleOrDefault();
            return rtn;
#else
            return 1;
#endif 
        }
        #endregion

        #region aspnet_UsersInRoles_AddUsersToRoles
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="userNames"></param>
        /// <param name="RoleNames"></param>
        /// <param name="currentTimeUtc"></param>
        /// <returns></returns>
        public IEnumerable<aspnet_UsersInRoles_AddUsersToRoles_Result> aspnet_UsersInRoles_AddUsersToRoles(string applicationName, string userNames, string RoleNames, DateTime currentTimeUtc)
        {
            int rtn;
            IEnumerable<aspnet_UsersInRoles_AddUsersToRoles_Result> result
                = this.ExecuteStoredProcedure<aspnet_UsersInRoles_AddUsersToRoles_Result>(
                    "aspnet_UsersInRoles_AddUsersToRoles",
                    out rtn,
                    applicationName,
                    userNames,
                    RoleNames,
                    currentTimeUtc);
            return result;
        }
        #endregion

        #region aspnet_UsersInRoles_FindUsersInRole
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="roleName"></param>
        /// <param name="userNameToMatch"></param>
        /// <returns></returns>
        public IEnumerable<aspnet_UsersInRoles_FindUsersInRole_Result> aspnet_UsersInRoles_FindUsersInRole(string applicationName, string roleName, string userNameToMatch)
        {
            int rtn;
            IEnumerable<aspnet_UsersInRoles_FindUsersInRole_Result> result =
                this.ExecuteStoredProcedure<aspnet_UsersInRoles_FindUsersInRole_Result>(
                    "aspnet_UsersInRoles_FindUsersInRole",
                    out rtn,
                    applicationName,
                    roleName,
                    userNameToMatch);
            return result;
        }

        #endregion

        #region aspnet_UsersInRoles_GetRolesForUser
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public IEnumerable<aspnet_UsersInRoles_GetRolesForUser_Result> aspnet_UsersInRoles_GetRolesForUser(string applicationName, string userName)
        {
            int rtn;
            IEnumerable<aspnet_UsersInRoles_GetRolesForUser_Result> result
                = this.ExecuteStoredProcedure<aspnet_UsersInRoles_GetRolesForUser_Result>(
                    "aspnet_UsersInRoles_GetRolesForUser",
                    out rtn,
                    applicationName,
                    userName);
            return result;
        }
        #endregion

        #region aspnet_UsersInRoles_IsUserInRole
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="userName"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public bool aspnet_UsersInRoles_IsUserInRole(string applicationName, string userName, string roleName)
        {
            int rtn = this.ExecuteStoredProcedure(
                "ApplicationName",
                applicationName,
                userName,
                roleName);

            return rtn == 1;
        }
        #endregion

        #region aspnet_UsersInRoles_RemoveUsersFromRoles
        public IEnumerable<aspnet_UsersInRoles_RemoveUsersFromRoles_Result> aspnet_UsersInRoles_RemoveUsersFromRoles(string applicationName, string userNames, string roleNames, out int rtnvalue)
        {
            int rtn;
            IEnumerable<aspnet_UsersInRoles_RemoveUsersFromRoles_Result> result =
                this.ExecuteStoredProcedure<aspnet_UsersInRoles_RemoveUsersFromRoles_Result>(
                    "aspnet_UsersInRoles_RemoveUsersFromRoles",
                    out rtn,
                    applicationName,
                    userNames,
                    roleNames);
            rtnvalue = rtn;
            return result;
        }
        #endregion

        #region aspnet_WebEvent_LogEvent
        public void aspnet_WebEvent_LogEvent(
            string eventId,
            DateTime eventTimeUtc,
            DateTime eventTime,
            string eventType,
            decimal eventSequence,
            decimal eventOccurrence,
            int eventCode,
            int eventDetailCode,
            string message,
            string applicationPath,
            string applicationVirtualPath,
            string machineName,
            string requestUrl,
            string exceptionType,
            string details
            )
        {
            this.ExecuteStoredProcedure("",
                eventId,
                eventTimeUtc,
                eventTime,
                eventType,
                eventSequence,
                eventOccurrence,
                eventCode,
                eventDetailCode,
                message,
                applicationPath,
                applicationVirtualPath,
                machineName,
                requestUrl,
                exceptionType,
                details);
        }
        #endregion
    }

}