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
            Assert.Fail();
        }
    }
}