using System;
using System.Collections.Generic;
namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class AspNetDbEntities2
    {

        public IEnumerable<aspnet_AnyDataInTables_Result> aspnet_AnyDataInTables(aspnet_AnyDataInTables_InputParameter spParameters)
        {
            int returnValue = 0;
            IEnumerable<aspnet_AnyDataInTables_Result> result;
            returnValue = this.ExecuteStoredProcedure(
                "aspnet_AnyDataInTables",
                out result,
                spParameters);
            spParameters.ReturnValue = returnValue;
            return result;
        }

        public void aspnet_Applications_CreateApplication(aspnet_Applications_CreateApplication_InputParameter spParameters)
        {
            int returnValue = 0;
            aspnet_Applications_CreateApplication_OutputParameter outputparameter;
            this.ExecuteStoredProcedure(
                "aspnet_Applications_CreateApplication",
                out outputparameter,
                out returnValue,
                spParameters);
            spParameters.OutputParameter = outputparameter;
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_CheckSchemaVersion(aspnet_CheckSchemaVersion_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
				"aspnet_CheckSchemaVersion",
				spParameters);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Membership_ChangePasswordQuestionAndAnswer(aspnet_Membership_ChangePasswordQuestionAndAnswer_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Membership_ChangePasswordQuestionAndAnswer",
    spParameters);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Membership_CreateUser(aspnet_Membership_CreateUser_InputParameter spParameters)
        {
            int returnValue = 0;
            aspnet_Membership_CreateUser_OutputParameter outputparameter;
            this.ExecuteStoredProcedure(
                "aspnet_Membership_CreateUser",
                out outputparameter,
                out returnValue,
                spParameters);
            spParameters.OutputParameter = outputparameter;
            spParameters.ReturnValue = returnValue;
        }
        public IEnumerable<aspnet_Membership_FindUsersByEmail_Result> aspnet_Membership_FindUsersByEmail(aspnet_Membership_FindUsersByEmail_InputParameter spParameters)
        {
            int returnValue = 0;
            IEnumerable<aspnet_Membership_FindUsersByEmail_Result> _result = null;
            _result = this.ExecuteStoredProcedure<aspnet_Membership_FindUsersByEmail_Result>(
    "aspnet_Membership_FindUsersByEmail",
    out returnValue,
    spParameters);
            spParameters.ReturnValue = returnValue;
            return _result;
        }
        public IEnumerable<aspnet_Membership_FindUsersByName_Result> aspnet_Membership_FindUsersByName(aspnet_Membership_FindUsersByName_InputParameter spParameters)
        {
            int returnValue = 0;
            IEnumerable<aspnet_Membership_FindUsersByName_Result> _result = null;
            _result = this.ExecuteStoredProcedure<aspnet_Membership_FindUsersByName_Result>(
     "aspnet_Membership_FindUsersByName",
     out returnValue,
     spParameters);
            spParameters.ReturnValue = returnValue;
            return _result;
        }
        public void aspnet_Membership_GetAllUsers(aspnet_Membership_GetAllUsers_InputParameter spParameters)
        {
            int returnValue = 0;
            aspnet_Membership_GetAllUsers_OutputParameter outputparameter;
            this.ExecuteStoredProcedure(
                "aspnet_Membership_GetAllUsers",
                out outputparameter,
                out returnValue,
                spParameters);
            spParameters.OutputParameter = outputparameter;
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Membership_GetNumberOfUsersOnline(aspnet_Membership_GetNumberOfUsersOnline_InputParameter spParameters)
        {
            int returnValue = 0;
            aspnet_Membership_GetNumberOfUsersOnline_OutputParameter outputparameter;
            this.ExecuteStoredProcedure(
                "aspnet_Membership_GetNumberOfUsersOnline",
                out outputparameter,
                out returnValue,
                spParameters);
            spParameters.OutputParameter = outputparameter;
            spParameters.ReturnValue = returnValue;
        }
        public IEnumerable<aspnet_Membership_GetPassword_Result> aspnet_Membership_GetPassword(aspnet_Membership_GetPassword_InputParameter spParameters)
        {
            int returnValue = 0;
            IEnumerable<aspnet_Membership_GetPassword_Result> result;
            returnValue = this.ExecuteStoredProcedure(
     "aspnet_Membership_GetPassword",
     out result,
             spParameters);
            spParameters.ReturnValue = returnValue;
            return result;
        }
        public IEnumerable<aspnet_Membership_GetPasswordWithFormat_Result> aspnet_Membership_GetPasswordWithFormat(aspnet_Membership_GetPasswordWithFormat_InputParameter spParameters)
        {
            int returnValue = 0;
            aspnet_Membership_GetPasswordWithFormat_OutputParameter outputparameter;
            IEnumerable<aspnet_Membership_GetPasswordWithFormat_Result> _result =
            this.ExecuteStoredProcedure<aspnet_Membership_GetPasswordWithFormat_Result, aspnet_Membership_GetPasswordWithFormat_OutputParameter>(
                "aspnet_Membership_GetPasswordWithFormat",
                out outputparameter,
                out returnValue,
                spParameters);
            spParameters.OutputParameter = outputparameter;
            spParameters.ReturnValue = returnValue;
            return _result;
        }
        public IEnumerable<aspnet_Membership_GetUserByEmail_Result> aspnet_Membership_GetUserByEmail(aspnet_Membership_GetUserByEmail_InputParameter spParameters)
        {
            int returnValue = 0;
            IEnumerable<aspnet_Membership_GetUserByEmail_Result> result;
            returnValue = this.ExecuteStoredProcedure(
     "aspnet_Membership_GetUserByEmail",
     out result,
             spParameters);
            spParameters.ReturnValue = returnValue;
            return result;
        }
        public IEnumerable<aspnet_Membership_GetUserByName_Result> aspnet_Membership_GetUserByName(aspnet_Membership_GetUserByName_InputParameter spParameters)
        {
            int returnValue = 0;
            IEnumerable<aspnet_Membership_GetUserByName_Result> result = null;
            result = this.ExecuteStoredProcedure<aspnet_Membership_GetUserByName_Result>(
                "aspnet_Membership_GetUserByName",
                out returnValue,
                spParameters);
            spParameters.ReturnValue = returnValue;
            return result;
        }
        public void aspnet_Membership_GetUserByUserId(aspnet_Membership_GetUserByUserId_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Membership_GetUserByUserId",
    spParameters);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Membership_ResetPassword(aspnet_Membership_ResetPassword_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Membership_ResetPassword",
    spParameters);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Membership_SetPassword(aspnet_Membership_SetPassword_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Membership_SetPassword",
    spParameters);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Membership_UnlockUser(aspnet_Membership_UnlockUser_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Membership_UnlockUser",
    spParameters);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Membership_UpdateUser(aspnet_Membership_UpdateUser_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Membership_UpdateUser",
    spParameters);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Membership_UpdateUserInfo(aspnet_Membership_UpdateUserInfo_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Membership_UpdateUserInfo",
    spParameters);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Paths_CreatePath(aspnet_Paths_CreatePath_InputParameter spParameters)
        {
            int returnValue = 0;
            aspnet_Paths_CreatePath_OutputParameter outputparameter;
            this.ExecuteStoredProcedure(
                "aspnet_Paths_CreatePath",
                out outputparameter,
                out returnValue,
                spParameters);
            spParameters.OutputParameter = outputparameter;
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Personalization_GetApplicationId(aspnet_Personalization_GetApplicationId_InputParameter spParameters)
        {
            int returnValue = 0;
            aspnet_Personalization_GetApplicationId_OutputParameter outputparameter;
            this.ExecuteStoredProcedure(
                "aspnet_Personalization_GetApplicationId",
                out outputparameter,
                out returnValue,
                spParameters);
            spParameters.OutputParameter = outputparameter;
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_PersonalizationAdministration_DeleteAllState(aspnet_PersonalizationAdministration_DeleteAllState_InputParameter spParameters)
        {
            int returnValue = 0;
            aspnet_PersonalizationAdministration_DeleteAllState_OutputParameter outputparameter;
            this.ExecuteStoredProcedure(
                "aspnet_PersonalizationAdministration_DeleteAllState",
                out outputparameter,
                out returnValue,
                spParameters);
            spParameters.OutputParameter = outputparameter;
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_PersonalizationAdministration_FindState(aspnet_PersonalizationAdministration_FindState_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_PersonalizationAdministration_FindState",
    spParameters);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_PersonalizationAdministration_GetCountOfState(aspnet_PersonalizationAdministration_GetCountOfState_InputParameter spParameters)
        {
            int returnValue = 0;
            aspnet_PersonalizationAdministration_GetCountOfState_OutputParameter outputparameter;
            this.ExecuteStoredProcedure(
                "aspnet_PersonalizationAdministration_GetCountOfState",
                out outputparameter,
                out returnValue,
                spParameters);
            spParameters.OutputParameter = outputparameter;
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_PersonalizationAdministration_ResetSharedState(aspnet_PersonalizationAdministration_ResetSharedState_InputParameter spParameters)
        {
            int returnValue = 0;
            aspnet_PersonalizationAdministration_ResetSharedState_OutputParameter outputparameter;
            this.ExecuteStoredProcedure(
                "aspnet_PersonalizationAdministration_ResetSharedState",
                out outputparameter,
                out returnValue,
                spParameters);
            spParameters.OutputParameter = outputparameter;
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_PersonalizationAdministration_ResetUserState(aspnet_PersonalizationAdministration_ResetUserState_InputParameter spParameters)
        {
            int returnValue = 0;
            aspnet_PersonalizationAdministration_ResetUserState_OutputParameter outputparameter;
            this.ExecuteStoredProcedure(
                "aspnet_PersonalizationAdministration_ResetUserState",
                out outputparameter,
                out returnValue,
                spParameters);
            spParameters.OutputParameter = outputparameter;
            spParameters.ReturnValue = returnValue;
        }
        public IEnumerable<aspnet_PersonalizationAllUsers_GetPageSettings_Result> aspnet_PersonalizationAllUsers_GetPageSettings(aspnet_PersonalizationAllUsers_GetPageSettings_InputParameter spParameters)
        {
            int returnValue = 0;
            IEnumerable<aspnet_PersonalizationAllUsers_GetPageSettings_Result> result;
            returnValue = this.ExecuteStoredProcedure(
     "aspnet_PersonalizationAllUsers_GetPageSettings",
     out result,
             spParameters);
            spParameters.ReturnValue = returnValue;
            return result;
        }
        public void aspnet_PersonalizationAllUsers_ResetPageSettings(aspnet_PersonalizationAllUsers_ResetPageSettings_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_PersonalizationAllUsers_ResetPageSettings",
    spParameters);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_PersonalizationAllUsers_SetPageSettings(aspnet_PersonalizationAllUsers_SetPageSettings_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_PersonalizationAllUsers_SetPageSettings",
    spParameters);
            spParameters.ReturnValue = returnValue;
        }
        public IEnumerable<aspnet_PersonalizationPerUser_GetPageSettings_Result> aspnet_PersonalizationPerUser_GetPageSettings(aspnet_PersonalizationPerUser_GetPageSettings_InputParameter spParameters)
        {
            int returnValue = 0;
            IEnumerable<aspnet_PersonalizationPerUser_GetPageSettings_Result> result;
            returnValue = this.ExecuteStoredProcedure(
     "aspnet_PersonalizationPerUser_GetPageSettings",
     out result,
             spParameters);
            spParameters.ReturnValue = returnValue;
            return result;
        }
        public void aspnet_PersonalizationPerUser_ResetPageSettings(aspnet_PersonalizationPerUser_ResetPageSettings_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_PersonalizationPerUser_ResetPageSettings",
    spParameters);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_PersonalizationPerUser_SetPageSettings(aspnet_PersonalizationPerUser_SetPageSettings_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_PersonalizationPerUser_SetPageSettings",
    spParameters);
            spParameters.ReturnValue = returnValue;
        }
        public IEnumerable<aspnet_Profile_DeleteInactiveProfiles_Result> aspnet_Profile_DeleteInactiveProfiles(aspnet_Profile_DeleteInactiveProfiles_InputParameter spParameters)
        {
            int returnValue = 0;
            IEnumerable<aspnet_Profile_DeleteInactiveProfiles_Result> result;
            returnValue = this.ExecuteStoredProcedure(
     "aspnet_Profile_DeleteInactiveProfiles",
     out result,
             spParameters);
            spParameters.ReturnValue = returnValue;
            return result;
        }

        public IEnumerable<aspnet_Profile_DeleteProfiles_Result> aspnet_Profile_DeleteProfiles(aspnet_Profile_DeleteProfiles_InputParameter spParameters)
        {
            int returnValue = 0;
            IEnumerable<aspnet_Profile_DeleteProfiles_Result> result;
            returnValue = this.ExecuteStoredProcedure(
     "aspnet_Profile_DeleteProfiles",
     out result,
             spParameters);
            spParameters.ReturnValue = returnValue;
            return result;
        }

        public IEnumerable<aspnet_Profile_GetNumberOfInactiveProfiles_Result> aspnet_Profile_GetNumberOfInactiveProfiles(aspnet_Profile_GetNumberOfInactiveProfiles_InputParameter spParameters)
        {
            int returnValue = 0;
            IEnumerable<aspnet_Profile_GetNumberOfInactiveProfiles_Result> result;
            returnValue = this.ExecuteStoredProcedure(
     "aspnet_Profile_GetNumberOfInactiveProfiles",
     out result,
             spParameters);
            spParameters.ReturnValue = returnValue;
            return result;
        }
        public void aspnet_Profile_GetProfiles(aspnet_Profile_GetProfiles_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Profile_GetProfiles",
    spParameters);
            spParameters.ReturnValue = returnValue;
        }
        public IEnumerable<aspnet_Profile_GetProperties_Result> aspnet_Profile_GetProperties(aspnet_Profile_GetProperties_InputParameter spParameters)
        {
            int returnValue = 0;
            IEnumerable<aspnet_Profile_GetProperties_Result> _result
                = this.ExecuteStoredProcedure<aspnet_Profile_GetProperties_Result>(
                    "aspnet_Profile_GetProperties",
                    out returnValue,
                    spParameters);
            spParameters.ReturnValue = returnValue;

            return _result;
        }
        public void aspnet_Profile_SetProperties(aspnet_Profile_SetProperties_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Profile_SetProperties",
    spParameters);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_RegisterSchemaVersion(aspnet_RegisterSchemaVersion_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_RegisterSchemaVersion",
    spParameters);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Roles_CreateRole(aspnet_Roles_CreateRole_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Roles_CreateRole",
    spParameters);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Roles_DeleteRole(aspnet_Roles_DeleteRole_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Roles_DeleteRole",
    spParameters);
            spParameters.ReturnValue = returnValue;
        }
        public IEnumerable<aspnet_Roles_GetAllRoles_Result> aspnet_Roles_GetAllRoles(aspnet_Roles_GetAllRoles_InputParameter spParameters)
        {
            int returnValue = 0;
            IEnumerable<aspnet_Roles_GetAllRoles_Result> result;
            returnValue = this.ExecuteStoredProcedure(
     "aspnet_Roles_GetAllRoles",
     out result,
             spParameters);
            spParameters.ReturnValue = returnValue;
            return result;
        }
        public void aspnet_Roles_RoleExists(aspnet_Roles_RoleExists_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Roles_RoleExists",
    spParameters);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Setup_RemoveAllRoleMembers(aspnet_Setup_RemoveAllRoleMembers_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Setup_RemoveAllRoleMembers",
    spParameters);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Setup_RestorePermissions(aspnet_Setup_RestorePermissions_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Setup_RestorePermissions",
    spParameters);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_UnRegisterSchemaVersion(aspnet_UnRegisterSchemaVersion_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_UnRegisterSchemaVersion",
    spParameters);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Users_CreateUser(aspnet_Users_CreateUser_InputParameter spParameters)
        {
            int returnValue = 0;
            aspnet_Users_CreateUser_OutputParameter outputparameter;
            this.ExecuteStoredProcedure(
                "aspnet_Users_CreateUser",
                out outputparameter,
                out returnValue,
                spParameters);
            spParameters.OutputParameter = outputparameter;
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Users_DeleteUser(aspnet_Users_DeleteUser_InputParameter spParameters)
        {
            int returnValue = 0;
            aspnet_Users_DeleteUser_OutputParameter outputparameter;
            this.ExecuteStoredProcedure(
                "aspnet_Users_DeleteUser",
                out outputparameter,
                out returnValue,
                spParameters);
            spParameters.OutputParameter = outputparameter;
            spParameters.ReturnValue = returnValue;
        }
        public IEnumerable<aspnet_UsersInRoles_AddUsersToRoles_Result> aspnet_UsersInRoles_AddUsersToRoles(aspnet_UsersInRoles_AddUsersToRoles_InputParameter spParameters)
        {
            int returnValue = 0;
            IEnumerable<aspnet_UsersInRoles_AddUsersToRoles_Result> result;
            returnValue = this.ExecuteStoredProcedure(
     "aspnet_UsersInRoles_AddUsersToRoles",
     out result,
             spParameters);
            spParameters.ReturnValue = returnValue;
            return result;
        }

        public IEnumerable<aspnet_UsersInRoles_FindUsersInRole_Result> aspnet_UsersInRoles_FindUsersInRole(aspnet_UsersInRoles_FindUsersInRole_InputParameter spParameters)
        {
            int returnValue = 0;
            IEnumerable<aspnet_UsersInRoles_FindUsersInRole_Result> result;
            returnValue = this.ExecuteStoredProcedure(
     "aspnet_UsersInRoles_FindUsersInRole",
     out result,
             spParameters);
            spParameters.ReturnValue = returnValue;
            return result;
        }

        public IEnumerable<aspnet_UsersInRoles_GetRolesForUser_Result> aspnet_UsersInRoles_GetRolesForUser(aspnet_UsersInRoles_GetRolesForUser_InputParameter spParameters)
        {
            int returnValue = 0;
            IEnumerable<aspnet_UsersInRoles_GetRolesForUser_Result> result;
            returnValue = this.ExecuteStoredProcedure(
     "aspnet_UsersInRoles_GetRolesForUser",
     out result,
             spParameters);
            spParameters.ReturnValue = returnValue;
            return result;
        }

        public IEnumerable<aspnet_UsersInRoles_GetUsersInRoles_Result> aspnet_UsersInRoles_GetUsersInRoles(aspnet_UsersInRoles_GetUsersInRoles_InputParameter spParameters)
        {
            int returnValue = 0;
            IEnumerable<aspnet_UsersInRoles_GetUsersInRoles_Result> result;
            returnValue = this.ExecuteStoredProcedure(
     "aspnet_UsersInRoles_GetUsersInRoles",
     out result,
             spParameters);
            spParameters.ReturnValue = returnValue;
            return result;
        }
        public void aspnet_UsersInRoles_IsUserInRole(aspnet_UsersInRoles_IsUserInRole_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_UsersInRoles_IsUserInRole",
    spParameters);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_UsersInRoles_RemoveUsersFromRoles(aspnet_UsersInRoles_RemoveUsersFromRoles_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_UsersInRoles_RemoveUsersFromRoles",
    spParameters);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_WebEvent_LogEvent(aspnet_WebEvent_LogEvent_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_WebEvent_LogEvent",
    spParameters);
            spParameters.ReturnValue = returnValue;
        }
    }
}
