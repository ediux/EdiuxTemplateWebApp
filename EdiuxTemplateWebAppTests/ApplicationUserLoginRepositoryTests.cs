using Microsoft.VisualStudio.TestTools.UnitTesting;
using EdiuxTemplateWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.Tests
{
    [TestClass()]
    public class ApplicationUserLoginRepositoryTests
    {
        private IApplicationUserRepository testUserRepo;
        private IApplicationUserLoginRepository testUserLoginRepo;
        private ApplicationUserLogin _userLogin;
#if CS6
        private const string KeyName = nameof(ApplicationUserLogin);
#else
        private static string KeyName = typeof(ApplicationUserLogin).Name;
#endif

        [TestInitialize]
        public void Init()
        {
            testUserLoginRepo = RepositoryHelper.GetApplicationUserLoginRepository();
        }

        [TestCleanup]
        public void GC()
        {

        }

        [TestMethod()]
        public void AddLoginAsyncTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail("Message:{0}\nSource:{1}\nStackTrace:{2}\n",
                    ex.Message,
                    ex.Source,
                    ex.StackTrace);

            }

            Assert.Fail();
        }

        [TestMethod()]
        public void FindAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetLoginsAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RemoveLoginAsyncTest()
        {
            Assert.Fail();
        }
    }
}