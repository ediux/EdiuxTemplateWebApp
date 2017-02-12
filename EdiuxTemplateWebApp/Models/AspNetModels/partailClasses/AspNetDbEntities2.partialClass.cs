using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Collections;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class AspNetDbEntities2
    {
        /// <summary>
        /// 取得應用程式的使用者。
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual PagedList<MembershipUser> aspnet_Membership_GetAllUsers(string applicationName, int pageIndex = 1, int pageSize = 5)
        {
            int TotalRecords = 0;

            List<aspnet_Membership_GetAllUsersResult> Members = this.ExecuteStoredProcedure<aspnet_Membership_GetAllUsersResult>(
                "aspnet_Membership_GetAllUsers",
                "ApplicationName,PageIndex,PageSize",
                "TotalRecords",
                applicationName,
                pageIndex,
                pageSize,
                TotalRecords);

            return new MyPagedList<MembershipUser>(Members.ConvertAll(c => aspnet_Membership_GetAllUsersResult.ConvertToMembershipUser(c)),
                pageIndex, pageSize == -1 ? TotalRecords : pageSize, TotalRecords);
        }

        /// <summary>
        /// 檢查只指定資料表是否有資料?
        /// </summary>
        /// <param name="tableToCheck">會員資料表列舉</param>
        /// <returns>回傳哪個資料表有資料。</returns>
        public virtual string aspnet_AnyDataInTables(TablesToCheck tableToCheck)
        {
            List<string> result = this.ExecuteStoredProcedure<string>("aspnet_AnyDataInTables", "TablesToCheck", tableToCheck);

            if (result != null && result.Count > 0)
            {
                return result.First();
            }

            return null;
        }

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
                    "ApplicationName,MinutesSinceLastInActive,CurrentTimeUtc"
                    , applicationName, MinutesSinceLastInActive, CurrentTimeUtc);
        }

        /// <summary>
        /// 檢查結構描述版本號
        /// </summary>
        /// <param name="Feature">功能名稱</param>
        /// <param name="CompatibleSchemaVersion">相容結構版本號</param>
        /// <returns></returns>
        public virtual int aspnet_CheckSchemaVersion(string Feature, string CompatibleSchemaVersion)
        {
            return this.ExecuteStoredProcedure(
                "aspnet_CheckSchemaVersion",
                "Feature,CompatibleSchemaVersion",
                Feature,
                CompatibleSchemaVersion
                );
        }

        public virtual bool aspnet_Membership_ChangePasswordQuestionAndAnswer(string applicationName, string userName, string NewPasswordQuestion, string NewPasswordAnswer)
        {
            int code = this.ExecuteStoredProcedure("aspnet_Membership_ChangePasswordQuestionAndAnswer",
                "ApplicationName,UserName,NewPasswordQuestion,NewPasswordAnswer",
                applicationName, userName, NewPasswordQuestion, NewPasswordAnswer);

            if (code == 1)
                return false;

            return true;
        }

        public virtual MembershipCreateStatus aspnet_Membership_CreateUser(string applicationName, string userName,
            string password, string passwordSalt, string eMail, out Guid userId, string passwordQuestion = "",
            string passwordAnswer = "", bool isApproved = false, int uniqueEmail = 0,
            MembershipPasswordFormat passwordFormat = MembershipPasswordFormat.Hashed)
        {
            try
            {
                userId = Guid.Empty;

                int code = -1;

                code = this.ExecuteStoredProcedure("aspnet_Membership_CreateUser",
                        "ApplicationName,UserName,Password,PasswordSalt,Email,PasswordQuestion,PasswordAnswer,IsApproved,CurrentTimeUtc,CreateDate,UniqueEmail,PasswordFormat",
                        "UserId,CreateStatus",
                        applicationName,
                        userName,
                        password,
                        passwordSalt,
                        eMail,
                        passwordQuestion,
                        passwordAnswer,
                        isApproved,
                        DateTime.UtcNow,
                        DateTime.Now.Date,
                        uniqueEmail,
                        (int)passwordFormat,
                        userId,
                        code);

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
            catch
            {
                throw;
            }
        }

        public virtual PagedList<MembershipUser> aspnet_Membership_FindUsersByEmail(string applicationName, string emailToMatch, int pageIndex = 1, int pageSize = 5)
        {
            List<aspnet_Membership_FindUsersByEmailResult> result;

            int TotalRecords = this.ExecuteStoredProcedure(
                "aspnet_Membership_FindUsersByEmail",
                "ApplicationName,EmailToMatch,PageIndex,PageSize",
                out result, applicationName, emailToMatch, pageIndex, pageSize);

            return new MyPagedList<MembershipUser>(result.ConvertAll(c => aspnet_Membership_FindUsersByEmailResult.ConvertToMembershipUser(c)),
                pageIndex, pageSize, TotalRecords);
        }

        public virtual PagedList<MembershipUser> aspnet_Membership_FindUsersByName(string applicationName, string usernameToMatch, int pageIndex = 1, int pageSize = 5)
        {
            List<aspnet_Membership_FindUsersByNameResult> result;

            int TotalRecords = this.ExecuteStoredProcedure(
                "aspnet_Membership_FindUsersByName",
                "ApplicationName,UserNameToMatch,PageIndex,PageSize",
                out result, applicationName, usernameToMatch, pageIndex, pageSize);

            return new MyPagedList<MembershipUser>(result.ConvertAll(c => aspnet_Membership_FindUsersByNameResult.ConvertToMembershipUser(c)),
                pageIndex, pageSize, TotalRecords);
        }


    }

}