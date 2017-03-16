using Microsoft.VisualStudio.TestTools.UnitTesting;
using EdiuxTemplateWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdiuxTemplateWebApp.Models.AspNetModels;

namespace EdiuxTemplateWebApp.Models.Tests
{
    /// <summary>
    /// 網站使用者與角色管理測試
    /// </summary>
    /// <remarks>
    /// 測試案例:
    /// 應用程式名稱:EdiuxTemplateWebAppTests
    /// 帳號名稱:root
    /// 角色:Admin
    /// 密碼:admin1234
    /// </remarks>
    [TestClass()]
    public class EdiuxAspNetSqlUserStoreTests
    {
        private static EdiuxAspNetSqlUserStore UserStore;
        private const string TestUserName = "root";
        private const string TestPassword = "admin1234";
        private const string TestRoleName = "Admins";
        private const string TestAppName = "EdiuxTemplateWebAppTests";

        private static IUnitOfWork<AspNetDbEntities2> UnitOfWork;
        private static Iaspnet_ApplicationsRepository apprepo;

        [TestInitialize]
        public static void PreparingTest()
        {
            UserStore = new EdiuxAspNetSqlUserStore(RepositoryHelper.GetUnitOfWork());
            if (UnitOfWork == null)
            {
                UnitOfWork = RepositoryHelper.GetUnitOfWork<AspNetDbEntities2>();
            }

            if (apprepo == null)
                apprepo = RepositoryHelper.Getaspnet_ApplicationsRepository(UnitOfWork);

            var query_Apps = apprepo.Where(w => w.ApplicationName == TestAppName);
            if (query_Apps.Any())
            {
                var result_apps = query_Apps.ToList();
                foreach(var delapp in result_apps)
                {
                    apprepo.Delete(delapp);
                }
                apprepo.UnitOfWork.Commit();
            }
        }

        [TestCleanup]
        public static void FinishTest()
        {
            UserStore.Dispose();
        }

        [TestMethod()]
        public void EdiuxAspNetSqlUserStoreTest()
        {
            try
            {
                Assert.IsNotNull(UserStore);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message + ex.StackTrace);
            }
        }

        [TestMethod()]
        public void CreateAsyncTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message + ex.StackTrace);
            }
        }

        [TestMethod()]
        public void DeleteAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void FindByIdAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void FindByNameAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UpdateAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddToRoleAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetRolesAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void IsInRoleAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RemoveFromRoleAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateAsyncTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UpdateAsyncTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteAsyncTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetEmailAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetEmailAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetEmailConfirmedAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetEmailConfirmedAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void FindByEmailAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetLockoutEndDateAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetLockoutEndDateAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void IncrementAccessFailedCountAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ResetAccessFailedCountAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAccessFailedCountAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetLockoutEnabledAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetLockoutEnabledAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddLoginAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RemoveLoginAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetLoginsAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void FindAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetPasswordHashAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetPasswordHashAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void HasPasswordAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetPhoneNumberAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetPhoneNumberAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetPhoneNumberConfirmedAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetPhoneNumberConfirmedAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetSecurityStampAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetSecurityStampAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetTwoFactorEnabledAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetTwoFactorEnabledAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetClaimsAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddClaimAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RemoveClaimAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DisposeTest()
        {
            Assert.Fail();
        }
    }
}