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
    public class ApplicationRoleRepositoryTests
    {
        private IApplicationRoleRepository testRoleRepo;
        private ApplicationRole _role;
#if CS6
        private const string KeyName = nameof(ApplicationRole);
#else
        private static string KeyName = typeof(ApplicationRole).Name;
#endif

        [TestInitialize]
        public void init()
        {
            testRoleRepo = RepositoryHelper.GetApplicationRoleRepository();
        }

        [TestCleanup]
        public void GC()
        {
            IQueryable<ApplicationRole> _queryResult = testRoleRepo.Where(w => w.Name == "TestUsers");
            if (_queryResult.Any())
            {
                List<ApplicationRole> getRoleTest = testRoleRepo.Where(w => w.Name.Equals("TestUsers", StringComparison.InvariantCultureIgnoreCase)).ToList();

                if (getRoleTest != null)
                {
                    foreach (ApplicationRole role in getRoleTest)
                    {
                        testRoleRepo.Delete(role);
                    }
                    testRoleRepo.UnitOfWork.Commit();
                }
            }

            testRoleRepo.Dispose();
        }

        [TestMethod()]
        public void CreateAsyncTest()
        {
            try
            {
                if (_role != null)
                {
                    Assert.Fail("Role '{0}' is existed.", _role.Name);
                    return;
                }

                _role = ApplicationRole.Create();
                _role.Name = "TestUsers";
                testRoleRepo.CreateAsync(_role).Wait();
                FindByNameAsyncTest();
                Assert.IsNotNull(_role);
                Assert.IsTrue(_role.Id != 0 && _role.Name == "TestUsers");
            }
            catch (Exception ex)
            {
                Assert.Fail("Message:{0}\nSource:{1}\nStackTrace:{2}\n",
                    ex.Message,
                    ex.Source,
                    ex.StackTrace);
            }

        }

        [TestMethod()]
        public void DeleteAsyncTest()
        {
            try
            {
                if (_role == null)
                    FindByNameAsyncTest();

                if (_role == null)
                {
                    CreateAsyncTest();
                    FindByNameAsyncTest();
                }

                testRoleRepo.DeleteAsync(_role).Wait();
                _role = testRoleRepo.Get(_role.Id);
                Assert.AreEqual(true, _role.Void);
            }
            catch (Exception ex)
            {
                Assert.Fail("Message:{0}\nSource:{1}\nStackTrace:{2}\n",
                    ex.Message,
                    ex.Source,
                    ex.StackTrace);
            }

        }

        [TestMethod()]
        public void FindByIdAsyncTest()
        {
            try
            {
                if (_role == null)
                    FindByNameAsyncTest();

                Task<ApplicationRole> testcaseTask = testRoleRepo.FindByIdAsync(_role.Id);
                testcaseTask.Wait();
                _role = testcaseTask.Result;
                Assert.IsNotNull(_role);
                Assert.AreEqual("TestUsers", _role.Name);
            }
            catch (Exception ex)
            {
                Assert.Fail("Message:{0}\nSource:{1}\nStackTrace:{2}\n",
                ex.Message,
                ex.Source,
                ex.StackTrace);
            }
        }

        [TestMethod()]
        public void FindByNameAsyncTest()
        {
            try
            {
                Task<ApplicationRole> testcaseTask = testRoleRepo.FindByNameAsync("TestUsers");
                testcaseTask.Wait();
                _role = testcaseTask.Result;
                if (_role == null)
                {
                    CreateAsyncTest();
                }
                testcaseTask = testRoleRepo.FindByNameAsync("TestUsers");
                testcaseTask.Wait();
                _role = testcaseTask.Result;
                Assert.IsNotNull(_role);
                Assert.AreEqual("TestUsers", _role.Name);
            }
            catch (Exception ex)
            {
                Assert.Fail("Message:{0}\nSource:{1}\nStackTrace:{2}\n",
                ex.Message,
                ex.Source,
                ex.StackTrace);
            }
        }

        [TestMethod()]
        public void UpdateAsyncTest()
        {
            try
            {
                if (_role == null)
                    FindByNameAsyncTest();

                if (_role == null)
                {
                    CreateAsyncTest();
                    FindByNameAsyncTest();
                }

                _role.Void = true;
                _role.LastUpdateTime = DateTime.UtcNow;
                _role.LastUpdateUserId = 0;

                testRoleRepo.UpdateAsync(_role).Wait();
                _role = testRoleRepo.Reload(_role);
                Assert.AreEqual(true, _role.Void);
                Assert.AreEqual(0, _role.LastUpdateUserId);
            }
            catch (Exception ex)
            {
                Assert.Fail("Message:{0}\nSource:{1}\nStackTrace:{2}\n",
  ex.Message,
  ex.Source,
  ex.StackTrace);

            }
        }
    }
}