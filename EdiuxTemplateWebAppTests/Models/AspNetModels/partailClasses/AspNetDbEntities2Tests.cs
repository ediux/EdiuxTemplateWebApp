using Microsoft.VisualStudio.TestTools.UnitTesting;
using EdiuxTemplateWebApp.Models.AspNetModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EdiuxTemplateWebApp.Models.AspNetModels.Tests
{
    [TestClass()]
    public class AspNetDbEntities2Tests
    {
        private AspNetDbEntities2 db;

        [ClassInitialize]
        public static void class_init(TestContext t)
        {
        }

        [TestInitialize]
        public void TestInit()
        {
            db = new AspNetDbEntities2();
        }
        [TestMethod()]
        public void aspnet_AnyDataInTablesTest()
        {
            try
            {
                //db = new AspNetDbEntities2();

                string table = db.aspnet_AnyDataInTables(TablesToCheck.aspnet_Membership | TablesToCheck.aspnet_PersonalizationPerUser | TablesToCheck.aspnet_Profile | TablesToCheck.aspnet_Roles | TablesToCheck.aspnet_WebEvent_Events);

                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }


        }

        [TestMethod()]
        public void aspnet_Applications_CreateApplicationTest()
        {
            try
            {
                Guid AppId = db.aspnet_Applications_CreateApplication("EdiuxTemplateWebAppTest");
                Assert.IsTrue(AppId != Guid.Empty);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_CheckSchemaVersionTest()
        {
            try
            {
                int rtn = db.aspnet_CheckSchemaVersion("Test","0.0.0.1");
                Assert.AreEqual(0,rtn);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Membership_ChangePasswordQuestionAndAnswerTest()
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Membership_CreateUserTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Membership_FindUsersByEmailTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Membership_FindUsersByNameTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Membership_GetAllUsersTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Membership_GetNumberOfUsersOnlineTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Membership_GetPasswordTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Membership_GetPasswordWithFormatTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Membership_GetUserByEmailTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Membership_GetUserByNameTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Membership_GetUserByUserIdTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Membership_ResetPasswordTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Membership_SetPasswordTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Membership_UnlockUserTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Membership_UpdateUserTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Membership_UpdateUserInfoTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Paths_CreatePathTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Personalization_GetApplicationIdTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_PersonalizationAdministration_DeleteAllStateTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_PersonalizationAdministration_FindStateTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_PersonalizationAdministration_GetCountOfStateTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_PersonalizationAdministration_ResetSharedStateTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_PersonalizationAdministration_ResetUserStateTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_PersonalizationAllUsers_GetPageSettingsTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_PersonalizationAllUsers_ResetPageSettingsTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_PersonalizationAllUsers_SetPageSettingsTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_PersonalizationPerUser_GetPageSettingsTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_PersonalizationPerUser_ResetPageSettingsTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_PersonalizationPerUser_SetPageSettingsTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Profile_DeleteInactiveProfilesTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Profile_DeleteProfilesTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Profile_GetProfilesTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Profile_GetPropertiesTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Profile_SetPropertiesTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_RegisterSchemaVersionTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Roles_CreateRoleTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Roles_DeleteRoleTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Roles_GetAllRolesTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Roles_RoleExistsTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Users_CreateUserTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_Users_DeleteUserTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_UsersInRoles_AddUsersToRolesTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_UsersInRoles_FindUsersInRoleTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_UsersInRoles_GetRolesForUserTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_UsersInRoles_IsUserInRoleTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_UsersInRoles_RemoveUsersFromRolesTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void aspnet_WebEvent_LogEventTest()
        {
            try
            {
                db.aspnet_WebEvent_LogEvent("event_01", DateTime.UtcNow, DateTime.Now, "Test", 0, 0, 1, 1003, "e", HttpRuntime.AppDomainAppPath, HttpRuntime.AppDomainAppVirtualPath, Environment.MachineName, "/", "Null", "adfd");

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}