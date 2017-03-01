using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_MembershipRepository : EFRepository<aspnet_Membership>, Iaspnet_MembershipRepository
    {

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
