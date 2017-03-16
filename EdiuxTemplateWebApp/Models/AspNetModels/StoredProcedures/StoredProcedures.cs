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
                spParameters.TablesToCheck);
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
                spParameters.ApplicationName);
            spParameters.OutputParameter = outputparameter;
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_CheckSchemaVersion(aspnet_CheckSchemaVersion_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_CheckSchemaVersion",
    spParameters.Feature,
spParameters.CompatibleSchemaVersion);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Membership_ChangePasswordQuestionAndAnswer(aspnet_Membership_ChangePasswordQuestionAndAnswer_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Membership_ChangePasswordQuestionAndAnswer",
    spParameters.ApplicationName,
spParameters.UserName,
spParameters.NewPasswordQuestion,
spParameters.NewPasswordAnswer);
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
                spParameters.ApplicationName,
spParameters.UserName,
spParameters.Password,
spParameters.PasswordSalt,
spParameters.Email,
spParameters.PasswordQuestion,
spParameters.PasswordAnswer,
spParameters.IsApproved,
spParameters.CurrentTimeUtc,
spParameters.CreateDate,
spParameters.UniqueEmail,
spParameters.PasswordFormat);
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
    spParameters.ApplicationName,
spParameters.EmailToMatch,
spParameters.PageIndex,
spParameters.PageSize);
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
     spParameters.ApplicationName,
 spParameters.UserNameToMatch,
 spParameters.PageIndex,
 spParameters.PageSize);
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
                spParameters.ApplicationName,
spParameters.PageIndex,
spParameters.PageSize);
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
                spParameters.ApplicationName,
spParameters.MinutesSinceLastInActive,
spParameters.CurrentTimeUtc);
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
             spParameters.applicationName,
spParameters.userName,
spParameters.maxInvalidPasswordAttempts,
spParameters.passwordAttemptWindow,
spParameters.currentTimeUtc,
spParameters.passwordAnswer);
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
                spParameters.applicationName,
                spParameters.userName,
                spParameters.updateLastLoginActivityDate,
                spParameters.currentTimeUtc);
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
             spParameters.applicationName,
spParameters.email);
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
                spParameters.ApplicationName,
                spParameters.UserName,
                spParameters.CurrentTimeUtc,
                spParameters.UpdateLastActivity);
            spParameters.ReturnValue = returnValue;
            return result;
        }
        public void aspnet_Membership_GetUserByUserId(aspnet_Membership_GetUserByUserId_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Membership_GetUserByUserId",
    spParameters.userId,
spParameters.currentTimeUtc,
spParameters.updateLastActivity);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Membership_ResetPassword(aspnet_Membership_ResetPassword_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Membership_ResetPassword",
    spParameters.applicationName,
spParameters.userName,
spParameters.newPassword,
spParameters.maxInvalidPasswordAttempts,
spParameters.passwordAttemptWindow,
spParameters.passwordSalt,
spParameters.currentTimeUtc,
spParameters.passwordFormat,
spParameters.passwordAnswer);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Membership_SetPassword(aspnet_Membership_SetPassword_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Membership_SetPassword",
    spParameters.applicationName,
spParameters.userName,
spParameters.newPassword,
spParameters.passwordSalt,
spParameters.currentTimeUtc,
spParameters.passwordFormat);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Membership_UnlockUser(aspnet_Membership_UnlockUser_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Membership_UnlockUser",
    spParameters.applicationName,
spParameters.userName);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Membership_UpdateUser(aspnet_Membership_UpdateUser_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Membership_UpdateUser",
    spParameters.applicationName,
spParameters.userName,
spParameters.email,
spParameters.comment,
spParameters.isApproved,
spParameters.lastLoginDate,
spParameters.lastActivityDate,
spParameters.uniqueEmail,
spParameters.currentTimeUtc);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Membership_UpdateUserInfo(aspnet_Membership_UpdateUserInfo_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Membership_UpdateUserInfo",
    spParameters.applicationName,
spParameters.userName,
spParameters.isPasswordCorrect,
spParameters.updateLastLoginActivityDate,
spParameters.maxInvalidPasswordAttempts,
spParameters.passwordAttemptWindow,
spParameters.currentTimeUtc,
spParameters.lastLoginDate,
spParameters.lastActivityDate);
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
                spParameters.applicationId,
spParameters.path);
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
                spParameters.applicationName);
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
                spParameters.allUsersScope,
spParameters.applicationName);
            spParameters.OutputParameter = outputparameter;
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_PersonalizationAdministration_FindState(aspnet_PersonalizationAdministration_FindState_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_PersonalizationAdministration_FindState",
    spParameters.allUsersScope,
spParameters.applicationName,
spParameters.pageIndex,
spParameters.pageSize,
spParameters.path,
spParameters.userName,
spParameters.inactiveSinceDate);
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
                spParameters.allUsersScope,
spParameters.applicationName,
spParameters.path,
spParameters.userName,
spParameters.inactiveSinceDate);
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
                spParameters.applicationName,
spParameters.path);
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
                spParameters.applicationName,
spParameters.inactiveSinceDate,
spParameters.userName,
spParameters.path);
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
             spParameters.applicationName,
spParameters.path);
            spParameters.ReturnValue = returnValue;
            return result;
        }
        public void aspnet_PersonalizationAllUsers_ResetPageSettings(aspnet_PersonalizationAllUsers_ResetPageSettings_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_PersonalizationAllUsers_ResetPageSettings",
    spParameters.applicationName,
spParameters.path);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_PersonalizationAllUsers_SetPageSettings(aspnet_PersonalizationAllUsers_SetPageSettings_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_PersonalizationAllUsers_SetPageSettings",
    spParameters.applicationName,
spParameters.path,
spParameters.pageSettings,
spParameters.currentTimeUtc);
            spParameters.ReturnValue = returnValue;
        }
        public IEnumerable<aspnet_PersonalizationPerUser_GetPageSettings_Result> aspnet_PersonalizationPerUser_GetPageSettings(aspnet_PersonalizationPerUser_GetPageSettings_InputParameter spParameters)
        {
            int returnValue = 0;
            IEnumerable<aspnet_PersonalizationPerUser_GetPageSettings_Result> result;
            returnValue = this.ExecuteStoredProcedure(
     "aspnet_PersonalizationPerUser_GetPageSettings",
     out result,
             spParameters.applicationName,
spParameters.userName,
spParameters.path,
spParameters.currentTimeUtc);
            spParameters.ReturnValue = returnValue;
            return result;
        }
        public void aspnet_PersonalizationPerUser_ResetPageSettings(aspnet_PersonalizationPerUser_ResetPageSettings_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_PersonalizationPerUser_ResetPageSettings",
    spParameters.applicationName,
spParameters.userName,
spParameters.path,
spParameters.currentTimeUtc);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_PersonalizationPerUser_SetPageSettings(aspnet_PersonalizationPerUser_SetPageSettings_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_PersonalizationPerUser_SetPageSettings",
    spParameters.applicationName,
spParameters.userName,
spParameters.path,
spParameters.pageSettings,
spParameters.currentTimeUtc);
            spParameters.ReturnValue = returnValue;
        }
        public IEnumerable<aspnet_Profile_DeleteInactiveProfiles_Result> aspnet_Profile_DeleteInactiveProfiles(aspnet_Profile_DeleteInactiveProfiles_InputParameter spParameters)
        {
            int returnValue = 0;
            IEnumerable<aspnet_Profile_DeleteInactiveProfiles_Result> result;
            returnValue = this.ExecuteStoredProcedure(
     "aspnet_Profile_DeleteInactiveProfiles",
     out result,
             spParameters.applicationName,
spParameters.profileAuthOptions,
spParameters.inactiveSinceDate);
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
             spParameters.applicationName,
spParameters.userNames);
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
             spParameters.applicationName,
spParameters.profileAuthOptions,
spParameters.inactiveSinceDate);
            spParameters.ReturnValue = returnValue;
            return result;
        }
        public void aspnet_Profile_GetProfiles(aspnet_Profile_GetProfiles_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Profile_GetProfiles",
    spParameters.applicationName,
spParameters.profileAuthOptions,
spParameters.pageIndex,
spParameters.pageSize,
spParameters.userNameToMatch,
spParameters.inactiveSinceDate);
            spParameters.ReturnValue = returnValue;
        }
        public IEnumerable<aspnet_Profile_GetProperties_Result> aspnet_Profile_GetProperties(aspnet_Profile_GetProperties_InputParameter spParameters)
        {
            int returnValue = 0;
            IEnumerable<aspnet_Profile_GetProperties_Result> _result
                = this.ExecuteStoredProcedure<aspnet_Profile_GetProperties_Result>(
                    "aspnet_Profile_GetProperties",
                    out returnValue,
                    spParameters.applicationName,
                    spParameters.userName,
                    spParameters.currentTimeUtc);
            spParameters.ReturnValue = returnValue;

            return _result;
        }
        public void aspnet_Profile_SetProperties(aspnet_Profile_SetProperties_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Profile_SetProperties",
    spParameters.applicationName,
spParameters.propertyNames,
spParameters.propertyValuesString,
spParameters.propertyValuesBinary,
spParameters.userName,
spParameters.isUserAnonymous,
spParameters.currentTimeUtc);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_RegisterSchemaVersion(aspnet_RegisterSchemaVersion_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_RegisterSchemaVersion",
    spParameters.feature,
spParameters.compatibleSchemaVersion,
spParameters.isCurrentVersion,
spParameters.removeIncompatibleSchema);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Roles_CreateRole(aspnet_Roles_CreateRole_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Roles_CreateRole",
    spParameters.applicationName,
spParameters.roleName);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Roles_DeleteRole(aspnet_Roles_DeleteRole_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Roles_DeleteRole",
    spParameters.applicationName,
spParameters.roleName,
spParameters.deleteOnlyIfRoleIsEmpty);
            spParameters.ReturnValue = returnValue;
        }
        public IEnumerable<aspnet_Roles_GetAllRoles_Result> aspnet_Roles_GetAllRoles(aspnet_Roles_GetAllRoles_InputParameter spParameters)
        {
            int returnValue = 0;
            IEnumerable<aspnet_Roles_GetAllRoles_Result> result;
            returnValue = this.ExecuteStoredProcedure(
     "aspnet_Roles_GetAllRoles",
     out result,
             spParameters.applicationName);
            spParameters.ReturnValue = returnValue;
            return result;
        }
        public void aspnet_Roles_RoleExists(aspnet_Roles_RoleExists_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Roles_RoleExists",
    spParameters.applicationName,
spParameters.roleName);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Setup_RemoveAllRoleMembers(aspnet_Setup_RemoveAllRoleMembers_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Setup_RemoveAllRoleMembers",
    spParameters.name);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_Setup_RestorePermissions(aspnet_Setup_RestorePermissions_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_Setup_RestorePermissions",
    spParameters.name);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_UnRegisterSchemaVersion(aspnet_UnRegisterSchemaVersion_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_UnRegisterSchemaVersion",
    spParameters.feature,
spParameters.compatibleSchemaVersion);
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
                spParameters.applicationId,
spParameters.userName,
spParameters.isUserAnonymous,
spParameters.lastActivityDate);
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
                spParameters.ApplicationName,
spParameters.UserName,
spParameters.TablesToDeleteFrom);
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
             spParameters.applicationName,
spParameters.userNames,
spParameters.roleNames,
spParameters.currentTimeUtc);
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
             spParameters.applicationName,
spParameters.roleName,
spParameters.userNameToMatch);
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
             spParameters.applicationName,
spParameters.userName);
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
             spParameters.applicationName,
spParameters.roleName);
            spParameters.ReturnValue = returnValue;
            return result;
        }
        public void aspnet_UsersInRoles_IsUserInRole(aspnet_UsersInRoles_IsUserInRole_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_UsersInRoles_IsUserInRole",
    spParameters.applicationName,
spParameters.userName,
spParameters.roleName);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_UsersInRoles_RemoveUsersFromRoles(aspnet_UsersInRoles_RemoveUsersFromRoles_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_UsersInRoles_RemoveUsersFromRoles",
    spParameters.applicationName,
spParameters.userNames,
spParameters.roleNames);
            spParameters.ReturnValue = returnValue;
        }
        public void aspnet_WebEvent_LogEvent(aspnet_WebEvent_LogEvent_InputParameter spParameters)
        {
            int returnValue = 0;
            returnValue = this.ExecuteStoredProcedure(
    "aspnet_WebEvent_LogEvent",
    spParameters.eventId,
spParameters.eventTimeUtc,
spParameters.eventTime,
spParameters.eventType,
spParameters.eventSequence,
spParameters.eventOccurrence,
spParameters.eventCode,
spParameters.eventDetailCode,
spParameters.message,
spParameters.applicationPath,
spParameters.applicationVirtualPath,
spParameters.machineName,
spParameters.requestUrl,
spParameters.exceptionType,
spParameters.details);
            spParameters.ReturnValue = returnValue;
        }
    }
}
