using Microsoft.VisualStudio.TestTools.UnitTesting;
using EdiuxTemplateWebApp.Models.AspNetModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels.Tests
{
    [TestClass()]
    public class aspnet_MembershipRepositoryTests
    {
        internal Iaspnet_MembershipRepository membershipRepo;
        internal static string getApplicationNameFromConfiguationFile()
        {
            string appName;
            if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("ApplicationName"))
            {
                appName = System.Configuration.ConfigurationManager.AppSettings["ApplicationName"];
            }
            else
            {
                appName = typeof(MvcApplication).Namespace;
            }

            return appName;
        }

        [TestMethod()]
        public void GetNumberOfUsersOnlineTest()
        {
            try
            {
                membershipRepo = RepositoryHelper.Getaspnet_MembershipRepository();

                int checkpoint = membershipRepo.GetNumberOfUsersOnline(getApplicationNameFromConfiguationFile(), 30, DateTime.UtcNow);
                Assert.IsTrue(checkpoint >= 0);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message + ex.StackTrace);
            }

        }

        [TestMethod()]
        public void GetPasswordTest()
        {
            try
            {
                membershipRepo = RepositoryHelper.Getaspnet_MembershipRepository();
                System.Web.Security.MembershipPasswordFormat PwdFormat;
                string pwd = membershipRepo.GetPassword(getApplicationNameFromConfiguationFile(), "root", 30, 5, DateTime.UtcNow, out PwdFormat, "");
                Assert.IsTrue(string.IsNullOrEmpty(pwd) != true);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message + ex.StackTrace);
            }
        }

        [TestMethod()]
        public void CreateUserTest()
        {
            try
            {
                membershipRepo = RepositoryHelper.Getaspnet_MembershipRepository();
                Guid UserId = Guid.Empty;
                System.Web.Security.MembershipCreateStatus createUserStatus;
                createUserStatus = membershipRepo.CreateUser(getApplicationNameFromConfiguationFile(), "root" + Guid.NewGuid().ToString("N")
                    , System.Web.Security.Membership.GeneratePassword(8, 1), Guid.NewGuid().ToString("N"), "local@localhost", out UserId);
                Assert.IsTrue(createUserStatus == System.Web.Security.MembershipCreateStatus.Success && UserId != Guid.Empty);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message + ex.StackTrace);
            }
        }
    }
}