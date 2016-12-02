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
    public class ApplicationUserRepositoryTests
    {
        private IApplicationUserRepository testUserRepo;
        private ApplicationUser _user;
#if CS6
        private const string KeyName = nameof(ApplicationUser);
#else
        private static string KeyName = typeof(ApplicationUser).Name;
#endif
        [TestInitialize]
        public void init()
        {
            testUserRepo = RepositoryHelper.GetApplicationUserRepository();

        }

        [TestCleanup]
        public void GC()
        {
            if (testUserRepo.Where(w => w.UserName == "TestUser").Any())
            {
                ApplicationUser user = testUserRepo.All().FirstOrDefault(w => w.UserName == "TestUser");

                if (user != null)
                {
                    testUserRepo.Delete(user);
                    testUserRepo.UnitOfWork.Commit();
                }
            }

            testUserRepo.Dispose();
        }

        [TestMethod()]
        public void AllTest()
        {
            try
            {
                var result = testUserRepo.All();
                Assert.IsNotNull(result);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }


        }

        [TestMethod()]
        public void AddTest()
        {
            try
            {
                _user = ApplicationUser.Create();

                _user.UserName = "TestUser";
                _user.Password = "!QAZ2wsx";
                _user.DisplayName = "TestUser";
                _user.Void = false;

                var result = testUserRepo.Add(_user);

                testUserRepo.UnitOfWork.Commit();
                result = testUserRepo.Reload(result);
                Assert.AreNotSame(_user.Id, result.Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

        [TestMethod()]
        public void DeleteTest()
        {
            try
            {
                FindByNameAsyncTest();

                testUserRepo.Delete(_user);
                testUserRepo.UnitOfWork.Commit();

                var isDeleted = testUserRepo.Get(_user.Id);
                Assert.IsNotNull(isDeleted);
                Assert.AreEqual(true, isDeleted.Void);

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }


        }

        [TestMethod()]
        public void ClearCacheTest()
        {
            try
            {
                testUserRepo.ClearCache(KeyName);
                Assert.IsFalse(testUserRepo.UnitOfWork.IsSet(KeyName));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void CreateAsyncTest()
        {
            try
            {
                CreateTestUser();

                Task createTask = testUserRepo.CreateAsync(_user);
                createTask.Wait();
                //FindByNameAsyncTest();

                Assert.IsNotNull(_user);
                Assert.AreEqual("TestUser", _user.UserName);
                Assert.IsTrue(_user.ApplicationRole.Any(a => a.Name.Equals("Users", StringComparison.InvariantCultureIgnoreCase)),"User '{0}' is in roles of '{1}'.",_user.UserName,"Users");
            }
            catch (Exception ex)
            {
                if (ex.Message == "User 'TestUser' is existed.")
                    Assert.AreEqual("User 'TestUser' is existed.", ex.Message);
                else
                    Assert.Fail(ex.Message);
            }
        }

        private void CreateTestUser()
        {
            _user = ApplicationUser.Create();

            _user.UserName = "TestUser";
            _user.Password = "!QAZ2wsx";
            _user.DisplayName = "TestUser";
            _user.Void = false;
        }

        [TestMethod()]
        public async void DeleteAsyncTest()
        {
            try
            {
                FindByNameAsyncTest();
                await testUserRepo.DeleteAsync(_user);
                FindByNameAsyncTest();
                Assert.IsNull(_user);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void FindByIdAsyncTest()
        {
            try
            {
                if (_user == null)
                {
                    _user = testUserRepo.All()
                        .Where(w => w.UserName != "root").First();
                }

                Task<ApplicationUser> testinguser = testUserRepo.FindByIdAsync(_user.Id);
                testinguser.Wait();

                ApplicationUser queryResutl = testinguser.Result;
                Assert.IsNotNull(queryResutl);
            }
            catch (Exception ex)
            {

                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public async void FindByNameAsyncTest()
        {
            try
            {
                _user = await testUserRepo.FindByNameAsync("TestUser");
                Assert.IsNotNull(_user);
                Assert.AreNotEqual(0, _user.Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void GetCacheTest()
        {
            try
            {
                Assert.IsNotNull(testUserRepo.GetCache());
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void GetRolesAsyncTest()
        {
            try
            {
                if (_user == null)
                {
                    FindByNameAsyncTest();
                }

                Task<IList<string>> roles = testUserRepo.GetRolesAsync(_user);
                roles.Wait();
                Assert.IsNotNull(roles.Result);
                Assert.IsTrue(roles.Result.Count >= 0);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void IsInRoleAsyncTest()
        {
            try
            {
                if (_user == null)
                {
                    FindByNameAsyncTest();
                }

                Task<bool> isInRoleTask = testUserRepo.IsInRoleAsync(_user, "Users");
                isInRoleTask.Wait();
                             
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void RemoveFromRoleAsyncTest()
        {
            try
            {
                if (_user == null)
                {
                    FindByNameAsyncTest();
                }

                Task removeFromRoleTask = testUserRepo.RemoveFromRoleAsync(_user, "Users");
                Task<bool> isInRoleTask = testUserRepo.IsInRoleAsync(_user, "Users");
                Assert.IsFalse(isInRoleTask.Result);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void UpdateAsyncTest()
        {
            try
            {
                if (_user == null)
                {
                    FindByNameAsyncTest();
                }

                _user.LastUpdateUserId = 0;
                _user.LastUpdateTime = DateTime.UtcNow;
                testUserRepo.UpdateAsync(_user);

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}