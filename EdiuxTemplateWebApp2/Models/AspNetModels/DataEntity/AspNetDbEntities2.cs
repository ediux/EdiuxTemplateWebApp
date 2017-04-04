﻿using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class AspNetDbEntities2 : DbContext
	{

		public AspNetDbEntities2()
			: base("name=AspNetDbEntities2")
		{
            Database.SetInitializer<AspNetDbEntities2>(
                new CreateDatabaseIfNotExists<AspNetDbEntities2>()
            );
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<aspnet_PersonalizationAllUsers>()
			            .HasRequired(r => r.aspnet_Paths)
						.WithRequiredPrincipal();

			modelBuilder.Entity<aspnet_Profile>()
						.HasRequired(r => r.aspnet_Users)
						.WithRequiredPrincipal();
			
			base.OnModelCreating(modelBuilder);
		}

		public virtual DbSet<aspnet_Applications> aspnet_Applications { get; set; }
		public virtual DbSet<aspnet_Membership> aspnet_Membership { get; set; }
		public virtual DbSet<aspnet_Paths> aspnet_Paths { get; set; }
		public virtual DbSet<aspnet_PersonalizationAllUsers> aspnet_PersonalizationAllUsers { get; set; }
		public virtual DbSet<aspnet_PersonalizationPerUser> aspnet_PersonalizationPerUser { get; set; }
		public virtual DbSet<aspnet_Profile> aspnet_Profile { get; set; }
		public virtual DbSet<aspnet_Roles> aspnet_Roles { get; set; }
		public virtual DbSet<aspnet_SchemaVersions> aspnet_SchemaVersions { get; set; }
		public virtual DbSet<aspnet_Users> aspnet_Users { get; set; }
		public virtual DbSet<aspnet_WebEvent_Events> aspnet_WebEvent_Events { get; set; }
		public virtual DbSet<aspnet_VoidUsers> aspnet_VoidUsers { get; set; }
		public virtual DbSet<aspnet_UserLogin> aspnet_UserLogin { get; set; }
		public virtual DbSet<aspnet_UserClaims> aspnet_UserClaims { get; set; }
		public virtual DbSet<Menus> Menus { get; set; }

		public virtual ObjectResult<string> aspnet_AnyDataInTables(Nullable<int> tablesToCheck)
		{
			var tablesToCheckParameter = tablesToCheck.HasValue ?
				new ObjectParameter("TablesToCheck", tablesToCheck) :
				new ObjectParameter("TablesToCheck", typeof(int));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("aspnet_AnyDataInTables", tablesToCheckParameter);
		}

		public virtual int aspnet_Applications_CreateApplication(string applicationName, ObjectParameter applicationId)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Applications_CreateApplication", applicationNameParameter, applicationId);
		}

		public virtual int aspnet_CheckSchemaVersion(string feature, string compatibleSchemaVersion)
		{
			var featureParameter = feature != null ?
				new ObjectParameter("Feature", feature) :
				new ObjectParameter("Feature", typeof(string));

			var compatibleSchemaVersionParameter = compatibleSchemaVersion != null ?
				new ObjectParameter("CompatibleSchemaVersion", compatibleSchemaVersion) :
				new ObjectParameter("CompatibleSchemaVersion", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_CheckSchemaVersion", featureParameter, compatibleSchemaVersionParameter);
		}

		public virtual int aspnet_Membership_ChangePasswordQuestionAndAnswer(string applicationName, string userName, string newPasswordQuestion, string newPasswordAnswer)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var userNameParameter = userName != null ?
				new ObjectParameter("UserName", userName) :
				new ObjectParameter("UserName", typeof(string));

			var newPasswordQuestionParameter = newPasswordQuestion != null ?
				new ObjectParameter("NewPasswordQuestion", newPasswordQuestion) :
				new ObjectParameter("NewPasswordQuestion", typeof(string));

			var newPasswordAnswerParameter = newPasswordAnswer != null ?
				new ObjectParameter("NewPasswordAnswer", newPasswordAnswer) :
				new ObjectParameter("NewPasswordAnswer", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Membership_ChangePasswordQuestionAndAnswer", applicationNameParameter, userNameParameter, newPasswordQuestionParameter, newPasswordAnswerParameter);
		}

		public virtual int aspnet_Membership_CreateUser(string applicationName, string userName, string password, string passwordSalt, string email, string passwordQuestion, string passwordAnswer, Nullable<bool> isApproved, Nullable<System.DateTime> currentTimeUtc, Nullable<System.DateTime> createDate, Nullable<int> uniqueEmail, Nullable<int> passwordFormat, ObjectParameter userId, ObjectParameter createStatus)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var userNameParameter = userName != null ?
				new ObjectParameter("UserName", userName) :
				new ObjectParameter("UserName", typeof(string));

			var passwordParameter = password != null ?
				new ObjectParameter("Password", password) :
				new ObjectParameter("Password", typeof(string));

			var passwordSaltParameter = passwordSalt != null ?
				new ObjectParameter("PasswordSalt", passwordSalt) :
				new ObjectParameter("PasswordSalt", typeof(string));

			var emailParameter = email != null ?
				new ObjectParameter("Email", email) :
				new ObjectParameter("Email", typeof(string));

			var passwordQuestionParameter = passwordQuestion != null ?
				new ObjectParameter("PasswordQuestion", passwordQuestion) :
				new ObjectParameter("PasswordQuestion", typeof(string));

			var passwordAnswerParameter = passwordAnswer != null ?
				new ObjectParameter("PasswordAnswer", passwordAnswer) :
				new ObjectParameter("PasswordAnswer", typeof(string));

			var isApprovedParameter = isApproved.HasValue ?
				new ObjectParameter("IsApproved", isApproved) :
				new ObjectParameter("IsApproved", typeof(bool));

			var currentTimeUtcParameter = currentTimeUtc.HasValue ?
				new ObjectParameter("CurrentTimeUtc", currentTimeUtc) :
				new ObjectParameter("CurrentTimeUtc", typeof(System.DateTime));

			var createDateParameter = createDate.HasValue ?
				new ObjectParameter("CreateDate", createDate) :
				new ObjectParameter("CreateDate", typeof(System.DateTime));

			var uniqueEmailParameter = uniqueEmail.HasValue ?
				new ObjectParameter("UniqueEmail", uniqueEmail) :
				new ObjectParameter("UniqueEmail", typeof(int));

			var passwordFormatParameter = passwordFormat.HasValue ?
				new ObjectParameter("PasswordFormat", passwordFormat) :
				new ObjectParameter("PasswordFormat", typeof(int));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Membership_CreateUser", applicationNameParameter, userNameParameter, passwordParameter, passwordSaltParameter, emailParameter, passwordQuestionParameter, passwordAnswerParameter, isApprovedParameter, currentTimeUtcParameter, createDateParameter, uniqueEmailParameter, passwordFormatParameter, userId, createStatus);
		}

		public virtual int aspnet_Membership_FindUsersByEmail(string applicationName, string emailToMatch, Nullable<int> pageIndex, Nullable<int> pageSize)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var emailToMatchParameter = emailToMatch != null ?
				new ObjectParameter("EmailToMatch", emailToMatch) :
				new ObjectParameter("EmailToMatch", typeof(string));

			var pageIndexParameter = pageIndex.HasValue ?
				new ObjectParameter("PageIndex", pageIndex) :
				new ObjectParameter("PageIndex", typeof(int));

			var pageSizeParameter = pageSize.HasValue ?
				new ObjectParameter("PageSize", pageSize) :
				new ObjectParameter("PageSize", typeof(int));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Membership_FindUsersByEmail", applicationNameParameter, emailToMatchParameter, pageIndexParameter, pageSizeParameter);
		}

		public virtual int aspnet_Membership_FindUsersByName(string applicationName, string userNameToMatch, Nullable<int> pageIndex, Nullable<int> pageSize)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var userNameToMatchParameter = userNameToMatch != null ?
				new ObjectParameter("UserNameToMatch", userNameToMatch) :
				new ObjectParameter("UserNameToMatch", typeof(string));

			var pageIndexParameter = pageIndex.HasValue ?
				new ObjectParameter("PageIndex", pageIndex) :
				new ObjectParameter("PageIndex", typeof(int));

			var pageSizeParameter = pageSize.HasValue ?
				new ObjectParameter("PageSize", pageSize) :
				new ObjectParameter("PageSize", typeof(int));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Membership_FindUsersByName", applicationNameParameter, userNameToMatchParameter, pageIndexParameter, pageSizeParameter);
		}

		public virtual int aspnet_Membership_GetAllUsers(string applicationName, Nullable<int> pageIndex, Nullable<int> pageSize, ObjectParameter totalRecords)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var pageIndexParameter = pageIndex.HasValue ?
				new ObjectParameter("PageIndex", pageIndex) :
				new ObjectParameter("PageIndex", typeof(int));

			var pageSizeParameter = pageSize.HasValue ?
				new ObjectParameter("PageSize", pageSize) :
				new ObjectParameter("PageSize", typeof(int));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Membership_GetAllUsers", applicationNameParameter, pageIndexParameter, pageSizeParameter, totalRecords);
		}

		public virtual int aspnet_Membership_GetNumberOfUsersOnline(string applicationName, Nullable<int> minutesSinceLastInActive, Nullable<System.DateTime> currentTimeUtc, ObjectParameter numOnline)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var minutesSinceLastInActiveParameter = minutesSinceLastInActive.HasValue ?
				new ObjectParameter("MinutesSinceLastInActive", minutesSinceLastInActive) :
				new ObjectParameter("MinutesSinceLastInActive", typeof(int));

			var currentTimeUtcParameter = currentTimeUtc.HasValue ?
				new ObjectParameter("CurrentTimeUtc", currentTimeUtc) :
				new ObjectParameter("CurrentTimeUtc", typeof(System.DateTime));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Membership_GetNumberOfUsersOnline", applicationNameParameter, minutesSinceLastInActiveParameter, currentTimeUtcParameter, numOnline);
		}

		public virtual ObjectResult<aspnet_Membership_GetPassword_Result> aspnet_Membership_GetPassword(string applicationName, string userName, Nullable<int> maxInvalidPasswordAttempts, Nullable<int> passwordAttemptWindow, Nullable<System.DateTime> currentTimeUtc, string passwordAnswer)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var userNameParameter = userName != null ?
				new ObjectParameter("UserName", userName) :
				new ObjectParameter("UserName", typeof(string));

			var maxInvalidPasswordAttemptsParameter = maxInvalidPasswordAttempts.HasValue ?
				new ObjectParameter("MaxInvalidPasswordAttempts", maxInvalidPasswordAttempts) :
				new ObjectParameter("MaxInvalidPasswordAttempts", typeof(int));

			var passwordAttemptWindowParameter = passwordAttemptWindow.HasValue ?
				new ObjectParameter("PasswordAttemptWindow", passwordAttemptWindow) :
				new ObjectParameter("PasswordAttemptWindow", typeof(int));

			var currentTimeUtcParameter = currentTimeUtc.HasValue ?
				new ObjectParameter("CurrentTimeUtc", currentTimeUtc) :
				new ObjectParameter("CurrentTimeUtc", typeof(System.DateTime));

			var passwordAnswerParameter = passwordAnswer != null ?
				new ObjectParameter("PasswordAnswer", passwordAnswer) :
				new ObjectParameter("PasswordAnswer", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<aspnet_Membership_GetPassword_Result>("aspnet_Membership_GetPassword", applicationNameParameter, userNameParameter, maxInvalidPasswordAttemptsParameter, passwordAttemptWindowParameter, currentTimeUtcParameter, passwordAnswerParameter);
		}

		public virtual int aspnet_Membership_GetPasswordWithFormat(string applicationName, string userName, Nullable<bool> updateLastLoginActivityDate, Nullable<System.DateTime> currentTimeUtc, ObjectParameter returnCode)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var userNameParameter = userName != null ?
				new ObjectParameter("UserName", userName) :
				new ObjectParameter("UserName", typeof(string));

			var updateLastLoginActivityDateParameter = updateLastLoginActivityDate.HasValue ?
				new ObjectParameter("UpdateLastLoginActivityDate", updateLastLoginActivityDate) :
				new ObjectParameter("UpdateLastLoginActivityDate", typeof(bool));

			var currentTimeUtcParameter = currentTimeUtc.HasValue ?
				new ObjectParameter("CurrentTimeUtc", currentTimeUtc) :
				new ObjectParameter("CurrentTimeUtc", typeof(System.DateTime));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Membership_GetPasswordWithFormat", applicationNameParameter, userNameParameter, updateLastLoginActivityDateParameter, currentTimeUtcParameter, returnCode);
		}

		public virtual ObjectResult<string> aspnet_Membership_GetUserByEmail(string applicationName, string email)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var emailParameter = email != null ?
				new ObjectParameter("Email", email) :
				new ObjectParameter("Email", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("aspnet_Membership_GetUserByEmail", applicationNameParameter, emailParameter);
		}

		public virtual int aspnet_Membership_GetUserByName(string applicationName, string userName, Nullable<System.DateTime> currentTimeUtc, Nullable<bool> updateLastActivity)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var userNameParameter = userName != null ?
				new ObjectParameter("UserName", userName) :
				new ObjectParameter("UserName", typeof(string));

			var currentTimeUtcParameter = currentTimeUtc.HasValue ?
				new ObjectParameter("CurrentTimeUtc", currentTimeUtc) :
				new ObjectParameter("CurrentTimeUtc", typeof(System.DateTime));

			var updateLastActivityParameter = updateLastActivity.HasValue ?
				new ObjectParameter("UpdateLastActivity", updateLastActivity) :
				new ObjectParameter("UpdateLastActivity", typeof(bool));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Membership_GetUserByName", applicationNameParameter, userNameParameter, currentTimeUtcParameter, updateLastActivityParameter);
		}

		public virtual int aspnet_Membership_GetUserByUserId(Nullable<System.Guid> userId, Nullable<System.DateTime> currentTimeUtc, Nullable<bool> updateLastActivity)
		{
			var userIdParameter = userId.HasValue ?
				new ObjectParameter("UserId", userId) :
				new ObjectParameter("UserId", typeof(System.Guid));

			var currentTimeUtcParameter = currentTimeUtc.HasValue ?
				new ObjectParameter("CurrentTimeUtc", currentTimeUtc) :
				new ObjectParameter("CurrentTimeUtc", typeof(System.DateTime));

			var updateLastActivityParameter = updateLastActivity.HasValue ?
				new ObjectParameter("UpdateLastActivity", updateLastActivity) :
				new ObjectParameter("UpdateLastActivity", typeof(bool));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Membership_GetUserByUserId", userIdParameter, currentTimeUtcParameter, updateLastActivityParameter);
		}

		public virtual int aspnet_Membership_ResetPassword(string applicationName, string userName, string newPassword, Nullable<int> maxInvalidPasswordAttempts, Nullable<int> passwordAttemptWindow, string passwordSalt, Nullable<System.DateTime> currentTimeUtc, Nullable<int> passwordFormat, string passwordAnswer)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var userNameParameter = userName != null ?
				new ObjectParameter("UserName", userName) :
				new ObjectParameter("UserName", typeof(string));

			var newPasswordParameter = newPassword != null ?
				new ObjectParameter("NewPassword", newPassword) :
				new ObjectParameter("NewPassword", typeof(string));

			var maxInvalidPasswordAttemptsParameter = maxInvalidPasswordAttempts.HasValue ?
				new ObjectParameter("MaxInvalidPasswordAttempts", maxInvalidPasswordAttempts) :
				new ObjectParameter("MaxInvalidPasswordAttempts", typeof(int));

			var passwordAttemptWindowParameter = passwordAttemptWindow.HasValue ?
				new ObjectParameter("PasswordAttemptWindow", passwordAttemptWindow) :
				new ObjectParameter("PasswordAttemptWindow", typeof(int));

			var passwordSaltParameter = passwordSalt != null ?
				new ObjectParameter("PasswordSalt", passwordSalt) :
				new ObjectParameter("PasswordSalt", typeof(string));

			var currentTimeUtcParameter = currentTimeUtc.HasValue ?
				new ObjectParameter("CurrentTimeUtc", currentTimeUtc) :
				new ObjectParameter("CurrentTimeUtc", typeof(System.DateTime));

			var passwordFormatParameter = passwordFormat.HasValue ?
				new ObjectParameter("PasswordFormat", passwordFormat) :
				new ObjectParameter("PasswordFormat", typeof(int));

			var passwordAnswerParameter = passwordAnswer != null ?
				new ObjectParameter("PasswordAnswer", passwordAnswer) :
				new ObjectParameter("PasswordAnswer", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Membership_ResetPassword", applicationNameParameter, userNameParameter, newPasswordParameter, maxInvalidPasswordAttemptsParameter, passwordAttemptWindowParameter, passwordSaltParameter, currentTimeUtcParameter, passwordFormatParameter, passwordAnswerParameter);
		}

		public virtual int aspnet_Membership_SetPassword(string applicationName, string userName, string newPassword, string passwordSalt, Nullable<System.DateTime> currentTimeUtc, Nullable<int> passwordFormat)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var userNameParameter = userName != null ?
				new ObjectParameter("UserName", userName) :
				new ObjectParameter("UserName", typeof(string));

			var newPasswordParameter = newPassword != null ?
				new ObjectParameter("NewPassword", newPassword) :
				new ObjectParameter("NewPassword", typeof(string));

			var passwordSaltParameter = passwordSalt != null ?
				new ObjectParameter("PasswordSalt", passwordSalt) :
				new ObjectParameter("PasswordSalt", typeof(string));

			var currentTimeUtcParameter = currentTimeUtc.HasValue ?
				new ObjectParameter("CurrentTimeUtc", currentTimeUtc) :
				new ObjectParameter("CurrentTimeUtc", typeof(System.DateTime));

			var passwordFormatParameter = passwordFormat.HasValue ?
				new ObjectParameter("PasswordFormat", passwordFormat) :
				new ObjectParameter("PasswordFormat", typeof(int));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Membership_SetPassword", applicationNameParameter, userNameParameter, newPasswordParameter, passwordSaltParameter, currentTimeUtcParameter, passwordFormatParameter);
		}

		public virtual int aspnet_Membership_UnlockUser(string applicationName, string userName)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var userNameParameter = userName != null ?
				new ObjectParameter("UserName", userName) :
				new ObjectParameter("UserName", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Membership_UnlockUser", applicationNameParameter, userNameParameter);
		}

		public virtual int aspnet_Membership_UpdateUser(string applicationName, string userName, string email, string comment, Nullable<bool> isApproved, Nullable<System.DateTime> lastLoginDate, Nullable<System.DateTime> lastActivityDate, Nullable<int> uniqueEmail, Nullable<System.DateTime> currentTimeUtc)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var userNameParameter = userName != null ?
				new ObjectParameter("UserName", userName) :
				new ObjectParameter("UserName", typeof(string));

			var emailParameter = email != null ?
				new ObjectParameter("Email", email) :
				new ObjectParameter("Email", typeof(string));

			var commentParameter = comment != null ?
				new ObjectParameter("Comment", comment) :
				new ObjectParameter("Comment", typeof(string));

			var isApprovedParameter = isApproved.HasValue ?
				new ObjectParameter("IsApproved", isApproved) :
				new ObjectParameter("IsApproved", typeof(bool));

			var lastLoginDateParameter = lastLoginDate.HasValue ?
				new ObjectParameter("LastLoginDate", lastLoginDate) :
				new ObjectParameter("LastLoginDate", typeof(System.DateTime));

			var lastActivityDateParameter = lastActivityDate.HasValue ?
				new ObjectParameter("LastActivityDate", lastActivityDate) :
				new ObjectParameter("LastActivityDate", typeof(System.DateTime));

			var uniqueEmailParameter = uniqueEmail.HasValue ?
				new ObjectParameter("UniqueEmail", uniqueEmail) :
				new ObjectParameter("UniqueEmail", typeof(int));

			var currentTimeUtcParameter = currentTimeUtc.HasValue ?
				new ObjectParameter("CurrentTimeUtc", currentTimeUtc) :
				new ObjectParameter("CurrentTimeUtc", typeof(System.DateTime));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Membership_UpdateUser", applicationNameParameter, userNameParameter, emailParameter, commentParameter, isApprovedParameter, lastLoginDateParameter, lastActivityDateParameter, uniqueEmailParameter, currentTimeUtcParameter);
		}

		public virtual int aspnet_Membership_UpdateUserInfo(string applicationName, string userName, Nullable<bool> isPasswordCorrect, Nullable<bool> updateLastLoginActivityDate, Nullable<int> maxInvalidPasswordAttempts, Nullable<int> passwordAttemptWindow, Nullable<System.DateTime> currentTimeUtc, Nullable<System.DateTime> lastLoginDate, Nullable<System.DateTime> lastActivityDate)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var userNameParameter = userName != null ?
				new ObjectParameter("UserName", userName) :
				new ObjectParameter("UserName", typeof(string));

			var isPasswordCorrectParameter = isPasswordCorrect.HasValue ?
				new ObjectParameter("IsPasswordCorrect", isPasswordCorrect) :
				new ObjectParameter("IsPasswordCorrect", typeof(bool));

			var updateLastLoginActivityDateParameter = updateLastLoginActivityDate.HasValue ?
				new ObjectParameter("UpdateLastLoginActivityDate", updateLastLoginActivityDate) :
				new ObjectParameter("UpdateLastLoginActivityDate", typeof(bool));

			var maxInvalidPasswordAttemptsParameter = maxInvalidPasswordAttempts.HasValue ?
				new ObjectParameter("MaxInvalidPasswordAttempts", maxInvalidPasswordAttempts) :
				new ObjectParameter("MaxInvalidPasswordAttempts", typeof(int));

			var passwordAttemptWindowParameter = passwordAttemptWindow.HasValue ?
				new ObjectParameter("PasswordAttemptWindow", passwordAttemptWindow) :
				new ObjectParameter("PasswordAttemptWindow", typeof(int));

			var currentTimeUtcParameter = currentTimeUtc.HasValue ?
				new ObjectParameter("CurrentTimeUtc", currentTimeUtc) :
				new ObjectParameter("CurrentTimeUtc", typeof(System.DateTime));

			var lastLoginDateParameter = lastLoginDate.HasValue ?
				new ObjectParameter("LastLoginDate", lastLoginDate) :
				new ObjectParameter("LastLoginDate", typeof(System.DateTime));

			var lastActivityDateParameter = lastActivityDate.HasValue ?
				new ObjectParameter("LastActivityDate", lastActivityDate) :
				new ObjectParameter("LastActivityDate", typeof(System.DateTime));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Membership_UpdateUserInfo", applicationNameParameter, userNameParameter, isPasswordCorrectParameter, updateLastLoginActivityDateParameter, maxInvalidPasswordAttemptsParameter, passwordAttemptWindowParameter, currentTimeUtcParameter, lastLoginDateParameter, lastActivityDateParameter);
		}

		public virtual int aspnet_Paths_CreatePath(Nullable<System.Guid> applicationId, string path, ObjectParameter pathId)
		{
			var applicationIdParameter = applicationId.HasValue ?
				new ObjectParameter("ApplicationId", applicationId) :
				new ObjectParameter("ApplicationId", typeof(System.Guid));

			var pathParameter = path != null ?
				new ObjectParameter("Path", path) :
				new ObjectParameter("Path", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Paths_CreatePath", applicationIdParameter, pathParameter, pathId);
		}

		public virtual int aspnet_Personalization_GetApplicationId(string applicationName, ObjectParameter applicationId)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Personalization_GetApplicationId", applicationNameParameter, applicationId);
		}

		public virtual int aspnet_PersonalizationAdministration_DeleteAllState(Nullable<bool> allUsersScope, string applicationName, ObjectParameter count)
		{
			var allUsersScopeParameter = allUsersScope.HasValue ?
				new ObjectParameter("AllUsersScope", allUsersScope) :
				new ObjectParameter("AllUsersScope", typeof(bool));

			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_PersonalizationAdministration_DeleteAllState", allUsersScopeParameter, applicationNameParameter, count);
		}

		public virtual int aspnet_PersonalizationAdministration_FindState(Nullable<bool> allUsersScope, string applicationName, Nullable<int> pageIndex, Nullable<int> pageSize, string path, string userName, Nullable<System.DateTime> inactiveSinceDate)
		{
			var allUsersScopeParameter = allUsersScope.HasValue ?
				new ObjectParameter("AllUsersScope", allUsersScope) :
				new ObjectParameter("AllUsersScope", typeof(bool));

			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var pageIndexParameter = pageIndex.HasValue ?
				new ObjectParameter("PageIndex", pageIndex) :
				new ObjectParameter("PageIndex", typeof(int));

			var pageSizeParameter = pageSize.HasValue ?
				new ObjectParameter("PageSize", pageSize) :
				new ObjectParameter("PageSize", typeof(int));

			var pathParameter = path != null ?
				new ObjectParameter("Path", path) :
				new ObjectParameter("Path", typeof(string));

			var userNameParameter = userName != null ?
				new ObjectParameter("UserName", userName) :
				new ObjectParameter("UserName", typeof(string));

			var inactiveSinceDateParameter = inactiveSinceDate.HasValue ?
				new ObjectParameter("InactiveSinceDate", inactiveSinceDate) :
				new ObjectParameter("InactiveSinceDate", typeof(System.DateTime));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_PersonalizationAdministration_FindState", allUsersScopeParameter, applicationNameParameter, pageIndexParameter, pageSizeParameter, pathParameter, userNameParameter, inactiveSinceDateParameter);
		}

		public virtual int aspnet_PersonalizationAdministration_GetCountOfState(ObjectParameter count, Nullable<bool> allUsersScope, string applicationName, string path, string userName, Nullable<System.DateTime> inactiveSinceDate)
		{
			var allUsersScopeParameter = allUsersScope.HasValue ?
				new ObjectParameter("AllUsersScope", allUsersScope) :
				new ObjectParameter("AllUsersScope", typeof(bool));

			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var pathParameter = path != null ?
				new ObjectParameter("Path", path) :
				new ObjectParameter("Path", typeof(string));

			var userNameParameter = userName != null ?
				new ObjectParameter("UserName", userName) :
				new ObjectParameter("UserName", typeof(string));

			var inactiveSinceDateParameter = inactiveSinceDate.HasValue ?
				new ObjectParameter("InactiveSinceDate", inactiveSinceDate) :
				new ObjectParameter("InactiveSinceDate", typeof(System.DateTime));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_PersonalizationAdministration_GetCountOfState", count, allUsersScopeParameter, applicationNameParameter, pathParameter, userNameParameter, inactiveSinceDateParameter);
		}

		public virtual int aspnet_PersonalizationAdministration_ResetSharedState(ObjectParameter count, string applicationName, string path)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var pathParameter = path != null ?
				new ObjectParameter("Path", path) :
				new ObjectParameter("Path", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_PersonalizationAdministration_ResetSharedState", count, applicationNameParameter, pathParameter);
		}

		public virtual int aspnet_PersonalizationAdministration_ResetUserState(ObjectParameter count, string applicationName, Nullable<System.DateTime> inactiveSinceDate, string userName, string path)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var inactiveSinceDateParameter = inactiveSinceDate.HasValue ?
				new ObjectParameter("InactiveSinceDate", inactiveSinceDate) :
				new ObjectParameter("InactiveSinceDate", typeof(System.DateTime));

			var userNameParameter = userName != null ?
				new ObjectParameter("UserName", userName) :
				new ObjectParameter("UserName", typeof(string));

			var pathParameter = path != null ?
				new ObjectParameter("Path", path) :
				new ObjectParameter("Path", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_PersonalizationAdministration_ResetUserState", count, applicationNameParameter, inactiveSinceDateParameter, userNameParameter, pathParameter);
		}

		public virtual ObjectResult<byte[]> aspnet_PersonalizationAllUsers_GetPageSettings(string applicationName, string path)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var pathParameter = path != null ?
				new ObjectParameter("Path", path) :
				new ObjectParameter("Path", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<byte[]>("aspnet_PersonalizationAllUsers_GetPageSettings", applicationNameParameter, pathParameter);
		}

		public virtual int aspnet_PersonalizationAllUsers_ResetPageSettings(string applicationName, string path)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var pathParameter = path != null ?
				new ObjectParameter("Path", path) :
				new ObjectParameter("Path", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_PersonalizationAllUsers_ResetPageSettings", applicationNameParameter, pathParameter);
		}

		public virtual int aspnet_PersonalizationAllUsers_SetPageSettings(string applicationName, string path, byte[] pageSettings, Nullable<System.DateTime> currentTimeUtc)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var pathParameter = path != null ?
				new ObjectParameter("Path", path) :
				new ObjectParameter("Path", typeof(string));

			var pageSettingsParameter = pageSettings != null ?
				new ObjectParameter("PageSettings", pageSettings) :
				new ObjectParameter("PageSettings", typeof(byte[]));

			var currentTimeUtcParameter = currentTimeUtc.HasValue ?
				new ObjectParameter("CurrentTimeUtc", currentTimeUtc) :
				new ObjectParameter("CurrentTimeUtc", typeof(System.DateTime));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_PersonalizationAllUsers_SetPageSettings", applicationNameParameter, pathParameter, pageSettingsParameter, currentTimeUtcParameter);
		}

		public virtual ObjectResult<byte[]> aspnet_PersonalizationPerUser_GetPageSettings(string applicationName, string userName, string path, Nullable<System.DateTime> currentTimeUtc)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var userNameParameter = userName != null ?
				new ObjectParameter("UserName", userName) :
				new ObjectParameter("UserName", typeof(string));

			var pathParameter = path != null ?
				new ObjectParameter("Path", path) :
				new ObjectParameter("Path", typeof(string));

			var currentTimeUtcParameter = currentTimeUtc.HasValue ?
				new ObjectParameter("CurrentTimeUtc", currentTimeUtc) :
				new ObjectParameter("CurrentTimeUtc", typeof(System.DateTime));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<byte[]>("aspnet_PersonalizationPerUser_GetPageSettings", applicationNameParameter, userNameParameter, pathParameter, currentTimeUtcParameter);
		}

		public virtual int aspnet_PersonalizationPerUser_ResetPageSettings(string applicationName, string userName, string path, Nullable<System.DateTime> currentTimeUtc)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var userNameParameter = userName != null ?
				new ObjectParameter("UserName", userName) :
				new ObjectParameter("UserName", typeof(string));

			var pathParameter = path != null ?
				new ObjectParameter("Path", path) :
				new ObjectParameter("Path", typeof(string));

			var currentTimeUtcParameter = currentTimeUtc.HasValue ?
				new ObjectParameter("CurrentTimeUtc", currentTimeUtc) :
				new ObjectParameter("CurrentTimeUtc", typeof(System.DateTime));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_PersonalizationPerUser_ResetPageSettings", applicationNameParameter, userNameParameter, pathParameter, currentTimeUtcParameter);
		}

		public virtual int aspnet_PersonalizationPerUser_SetPageSettings(string applicationName, string userName, string path, byte[] pageSettings, Nullable<System.DateTime> currentTimeUtc)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var userNameParameter = userName != null ?
				new ObjectParameter("UserName", userName) :
				new ObjectParameter("UserName", typeof(string));

			var pathParameter = path != null ?
				new ObjectParameter("Path", path) :
				new ObjectParameter("Path", typeof(string));

			var pageSettingsParameter = pageSettings != null ?
				new ObjectParameter("PageSettings", pageSettings) :
				new ObjectParameter("PageSettings", typeof(byte[]));

			var currentTimeUtcParameter = currentTimeUtc.HasValue ?
				new ObjectParameter("CurrentTimeUtc", currentTimeUtc) :
				new ObjectParameter("CurrentTimeUtc", typeof(System.DateTime));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_PersonalizationPerUser_SetPageSettings", applicationNameParameter, userNameParameter, pathParameter, pageSettingsParameter, currentTimeUtcParameter);
		}

		public virtual ObjectResult<Nullable<int>> aspnet_Profile_DeleteInactiveProfiles(string applicationName, Nullable<int> profileAuthOptions, Nullable<System.DateTime> inactiveSinceDate)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var profileAuthOptionsParameter = profileAuthOptions.HasValue ?
				new ObjectParameter("ProfileAuthOptions", profileAuthOptions) :
				new ObjectParameter("ProfileAuthOptions", typeof(int));

			var inactiveSinceDateParameter = inactiveSinceDate.HasValue ?
				new ObjectParameter("InactiveSinceDate", inactiveSinceDate) :
				new ObjectParameter("InactiveSinceDate", typeof(System.DateTime));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("aspnet_Profile_DeleteInactiveProfiles", applicationNameParameter, profileAuthOptionsParameter, inactiveSinceDateParameter);
		}

		public virtual ObjectResult<Nullable<int>> aspnet_Profile_DeleteProfiles(string applicationName, string userNames)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var userNamesParameter = userNames != null ?
				new ObjectParameter("UserNames", userNames) :
				new ObjectParameter("UserNames", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("aspnet_Profile_DeleteProfiles", applicationNameParameter, userNamesParameter);
		}

		public virtual ObjectResult<Nullable<int>> aspnet_Profile_GetNumberOfInactiveProfiles(string applicationName, Nullable<int> profileAuthOptions, Nullable<System.DateTime> inactiveSinceDate)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var profileAuthOptionsParameter = profileAuthOptions.HasValue ?
				new ObjectParameter("ProfileAuthOptions", profileAuthOptions) :
				new ObjectParameter("ProfileAuthOptions", typeof(int));

			var inactiveSinceDateParameter = inactiveSinceDate.HasValue ?
				new ObjectParameter("InactiveSinceDate", inactiveSinceDate) :
				new ObjectParameter("InactiveSinceDate", typeof(System.DateTime));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("aspnet_Profile_GetNumberOfInactiveProfiles", applicationNameParameter, profileAuthOptionsParameter, inactiveSinceDateParameter);
		}

		public virtual int aspnet_Profile_GetProfiles(string applicationName, Nullable<int> profileAuthOptions, Nullable<int> pageIndex, Nullable<int> pageSize, string userNameToMatch, Nullable<System.DateTime> inactiveSinceDate)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var profileAuthOptionsParameter = profileAuthOptions.HasValue ?
				new ObjectParameter("ProfileAuthOptions", profileAuthOptions) :
				new ObjectParameter("ProfileAuthOptions", typeof(int));

			var pageIndexParameter = pageIndex.HasValue ?
				new ObjectParameter("PageIndex", pageIndex) :
				new ObjectParameter("PageIndex", typeof(int));

			var pageSizeParameter = pageSize.HasValue ?
				new ObjectParameter("PageSize", pageSize) :
				new ObjectParameter("PageSize", typeof(int));

			var userNameToMatchParameter = userNameToMatch != null ?
				new ObjectParameter("UserNameToMatch", userNameToMatch) :
				new ObjectParameter("UserNameToMatch", typeof(string));

			var inactiveSinceDateParameter = inactiveSinceDate.HasValue ?
				new ObjectParameter("InactiveSinceDate", inactiveSinceDate) :
				new ObjectParameter("InactiveSinceDate", typeof(System.DateTime));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Profile_GetProfiles", applicationNameParameter, profileAuthOptionsParameter, pageIndexParameter, pageSizeParameter, userNameToMatchParameter, inactiveSinceDateParameter);
		}

		public virtual int aspnet_Profile_GetProperties(string applicationName, string userName, Nullable<System.DateTime> currentTimeUtc)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var userNameParameter = userName != null ?
				new ObjectParameter("UserName", userName) :
				new ObjectParameter("UserName", typeof(string));

			var currentTimeUtcParameter = currentTimeUtc.HasValue ?
				new ObjectParameter("CurrentTimeUtc", currentTimeUtc) :
				new ObjectParameter("CurrentTimeUtc", typeof(System.DateTime));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Profile_GetProperties", applicationNameParameter, userNameParameter, currentTimeUtcParameter);
		}

		public virtual int aspnet_Profile_SetProperties(string applicationName, string propertyNames, string propertyValuesString, byte[] propertyValuesBinary, string userName, Nullable<bool> isUserAnonymous, Nullable<System.DateTime> currentTimeUtc)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var propertyNamesParameter = propertyNames != null ?
				new ObjectParameter("PropertyNames", propertyNames) :
				new ObjectParameter("PropertyNames", typeof(string));

			var propertyValuesStringParameter = propertyValuesString != null ?
				new ObjectParameter("PropertyValuesString", propertyValuesString) :
				new ObjectParameter("PropertyValuesString", typeof(string));

			var propertyValuesBinaryParameter = propertyValuesBinary != null ?
				new ObjectParameter("PropertyValuesBinary", propertyValuesBinary) :
				new ObjectParameter("PropertyValuesBinary", typeof(byte[]));

			var userNameParameter = userName != null ?
				new ObjectParameter("UserName", userName) :
				new ObjectParameter("UserName", typeof(string));

			var isUserAnonymousParameter = isUserAnonymous.HasValue ?
				new ObjectParameter("IsUserAnonymous", isUserAnonymous) :
				new ObjectParameter("IsUserAnonymous", typeof(bool));

			var currentTimeUtcParameter = currentTimeUtc.HasValue ?
				new ObjectParameter("CurrentTimeUtc", currentTimeUtc) :
				new ObjectParameter("CurrentTimeUtc", typeof(System.DateTime));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Profile_SetProperties", applicationNameParameter, propertyNamesParameter, propertyValuesStringParameter, propertyValuesBinaryParameter, userNameParameter, isUserAnonymousParameter, currentTimeUtcParameter);
		}

		public virtual int aspnet_RegisterSchemaVersion(string feature, string compatibleSchemaVersion, Nullable<bool> isCurrentVersion, Nullable<bool> removeIncompatibleSchema)
		{
			var featureParameter = feature != null ?
				new ObjectParameter("Feature", feature) :
				new ObjectParameter("Feature", typeof(string));

			var compatibleSchemaVersionParameter = compatibleSchemaVersion != null ?
				new ObjectParameter("CompatibleSchemaVersion", compatibleSchemaVersion) :
				new ObjectParameter("CompatibleSchemaVersion", typeof(string));

			var isCurrentVersionParameter = isCurrentVersion.HasValue ?
				new ObjectParameter("IsCurrentVersion", isCurrentVersion) :
				new ObjectParameter("IsCurrentVersion", typeof(bool));

			var removeIncompatibleSchemaParameter = removeIncompatibleSchema.HasValue ?
				new ObjectParameter("RemoveIncompatibleSchema", removeIncompatibleSchema) :
				new ObjectParameter("RemoveIncompatibleSchema", typeof(bool));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_RegisterSchemaVersion", featureParameter, compatibleSchemaVersionParameter, isCurrentVersionParameter, removeIncompatibleSchemaParameter);
		}

		public virtual int aspnet_Roles_CreateRole(string applicationName, string roleName)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var roleNameParameter = roleName != null ?
				new ObjectParameter("RoleName", roleName) :
				new ObjectParameter("RoleName", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Roles_CreateRole", applicationNameParameter, roleNameParameter);
		}

		public virtual int aspnet_Roles_DeleteRole(string applicationName, string roleName, Nullable<bool> deleteOnlyIfRoleIsEmpty)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var roleNameParameter = roleName != null ?
				new ObjectParameter("RoleName", roleName) :
				new ObjectParameter("RoleName", typeof(string));

			var deleteOnlyIfRoleIsEmptyParameter = deleteOnlyIfRoleIsEmpty.HasValue ?
				new ObjectParameter("DeleteOnlyIfRoleIsEmpty", deleteOnlyIfRoleIsEmpty) :
				new ObjectParameter("DeleteOnlyIfRoleIsEmpty", typeof(bool));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Roles_DeleteRole", applicationNameParameter, roleNameParameter, deleteOnlyIfRoleIsEmptyParameter);
		}

		public virtual ObjectResult<string> aspnet_Roles_GetAllRoles(string applicationName)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("aspnet_Roles_GetAllRoles", applicationNameParameter);
		}

		public virtual int aspnet_Roles_RoleExists(string applicationName, string roleName)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var roleNameParameter = roleName != null ?
				new ObjectParameter("RoleName", roleName) :
				new ObjectParameter("RoleName", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Roles_RoleExists", applicationNameParameter, roleNameParameter);
		}

		public virtual int aspnet_Setup_RemoveAllRoleMembers(string name)
		{
			var nameParameter = name != null ?
				new ObjectParameter("name", name) :
				new ObjectParameter("name", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Setup_RemoveAllRoleMembers", nameParameter);
		}

		public virtual int aspnet_Setup_RestorePermissions(string name)
		{
			var nameParameter = name != null ?
				new ObjectParameter("name", name) :
				new ObjectParameter("name", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Setup_RestorePermissions", nameParameter);
		}

		public virtual int aspnet_UnRegisterSchemaVersion(string feature, string compatibleSchemaVersion)
		{
			var featureParameter = feature != null ?
				new ObjectParameter("Feature", feature) :
				new ObjectParameter("Feature", typeof(string));

			var compatibleSchemaVersionParameter = compatibleSchemaVersion != null ?
				new ObjectParameter("CompatibleSchemaVersion", compatibleSchemaVersion) :
				new ObjectParameter("CompatibleSchemaVersion", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_UnRegisterSchemaVersion", featureParameter, compatibleSchemaVersionParameter);
		}

		public virtual int aspnet_Users_CreateUser(Nullable<System.Guid> applicationId, string userName, Nullable<bool> isUserAnonymous, Nullable<System.DateTime> lastActivityDate, ObjectParameter userId)
		{
			var applicationIdParameter = applicationId.HasValue ?
				new ObjectParameter("ApplicationId", applicationId) :
				new ObjectParameter("ApplicationId", typeof(System.Guid));

			var userNameParameter = userName != null ?
				new ObjectParameter("UserName", userName) :
				new ObjectParameter("UserName", typeof(string));

			var isUserAnonymousParameter = isUserAnonymous.HasValue ?
				new ObjectParameter("IsUserAnonymous", isUserAnonymous) :
				new ObjectParameter("IsUserAnonymous", typeof(bool));

			var lastActivityDateParameter = lastActivityDate.HasValue ?
				new ObjectParameter("LastActivityDate", lastActivityDate) :
				new ObjectParameter("LastActivityDate", typeof(System.DateTime));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Users_CreateUser", applicationIdParameter, userNameParameter, isUserAnonymousParameter, lastActivityDateParameter, userId);
		}

		public virtual int aspnet_Users_DeleteUser(string applicationName, string userName, Nullable<int> tablesToDeleteFrom, ObjectParameter numTablesDeletedFrom)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var userNameParameter = userName != null ?
				new ObjectParameter("UserName", userName) :
				new ObjectParameter("UserName", typeof(string));

			var tablesToDeleteFromParameter = tablesToDeleteFrom.HasValue ?
				new ObjectParameter("TablesToDeleteFrom", tablesToDeleteFrom) :
				new ObjectParameter("TablesToDeleteFrom", typeof(int));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_Users_DeleteUser", applicationNameParameter, userNameParameter, tablesToDeleteFromParameter, numTablesDeletedFrom);
		}

		public virtual ObjectResult<string> aspnet_UsersInRoles_AddUsersToRoles(string applicationName, string userNames, string roleNames, Nullable<System.DateTime> currentTimeUtc)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var userNamesParameter = userNames != null ?
				new ObjectParameter("UserNames", userNames) :
				new ObjectParameter("UserNames", typeof(string));

			var roleNamesParameter = roleNames != null ?
				new ObjectParameter("RoleNames", roleNames) :
				new ObjectParameter("RoleNames", typeof(string));

			var currentTimeUtcParameter = currentTimeUtc.HasValue ?
				new ObjectParameter("CurrentTimeUtc", currentTimeUtc) :
				new ObjectParameter("CurrentTimeUtc", typeof(System.DateTime));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("aspnet_UsersInRoles_AddUsersToRoles", applicationNameParameter, userNamesParameter, roleNamesParameter, currentTimeUtcParameter);
		}

		public virtual ObjectResult<string> aspnet_UsersInRoles_FindUsersInRole(string applicationName, string roleName, string userNameToMatch)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var roleNameParameter = roleName != null ?
				new ObjectParameter("RoleName", roleName) :
				new ObjectParameter("RoleName", typeof(string));

			var userNameToMatchParameter = userNameToMatch != null ?
				new ObjectParameter("UserNameToMatch", userNameToMatch) :
				new ObjectParameter("UserNameToMatch", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("aspnet_UsersInRoles_FindUsersInRole", applicationNameParameter, roleNameParameter, userNameToMatchParameter);
		}

		public virtual ObjectResult<string> aspnet_UsersInRoles_GetRolesForUser(string applicationName, string userName)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var userNameParameter = userName != null ?
				new ObjectParameter("UserName", userName) :
				new ObjectParameter("UserName", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("aspnet_UsersInRoles_GetRolesForUser", applicationNameParameter, userNameParameter);
		}

		public virtual ObjectResult<string> aspnet_UsersInRoles_GetUsersInRoles(string applicationName, string roleName)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var roleNameParameter = roleName != null ?
				new ObjectParameter("RoleName", roleName) :
				new ObjectParameter("RoleName", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("aspnet_UsersInRoles_GetUsersInRoles", applicationNameParameter, roleNameParameter);
		}

		public virtual int aspnet_UsersInRoles_IsUserInRole(string applicationName, string userName, string roleName)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var userNameParameter = userName != null ?
				new ObjectParameter("UserName", userName) :
				new ObjectParameter("UserName", typeof(string));

			var roleNameParameter = roleName != null ?
				new ObjectParameter("RoleName", roleName) :
				new ObjectParameter("RoleName", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_UsersInRoles_IsUserInRole", applicationNameParameter, userNameParameter, roleNameParameter);
		}

		public virtual int aspnet_UsersInRoles_RemoveUsersFromRoles(string applicationName, string userNames, string roleNames)
		{
			var applicationNameParameter = applicationName != null ?
				new ObjectParameter("ApplicationName", applicationName) :
				new ObjectParameter("ApplicationName", typeof(string));

			var userNamesParameter = userNames != null ?
				new ObjectParameter("UserNames", userNames) :
				new ObjectParameter("UserNames", typeof(string));

			var roleNamesParameter = roleNames != null ?
				new ObjectParameter("RoleNames", roleNames) :
				new ObjectParameter("RoleNames", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_UsersInRoles_RemoveUsersFromRoles", applicationNameParameter, userNamesParameter, roleNamesParameter);
		}

		public virtual int aspnet_WebEvent_LogEvent(string eventId, Nullable<System.DateTime> eventTimeUtc, Nullable<System.DateTime> eventTime, string eventType, Nullable<decimal> eventSequence, Nullable<decimal> eventOccurrence, Nullable<int> eventCode, Nullable<int> eventDetailCode, string message, string applicationPath, string applicationVirtualPath, string machineName, string requestUrl, string exceptionType, string details)
		{
			var eventIdParameter = eventId != null ?
				new ObjectParameter("EventId", eventId) :
				new ObjectParameter("EventId", typeof(string));

			var eventTimeUtcParameter = eventTimeUtc.HasValue ?
				new ObjectParameter("EventTimeUtc", eventTimeUtc) :
				new ObjectParameter("EventTimeUtc", typeof(System.DateTime));

			var eventTimeParameter = eventTime.HasValue ?
				new ObjectParameter("EventTime", eventTime) :
				new ObjectParameter("EventTime", typeof(System.DateTime));

			var eventTypeParameter = eventType != null ?
				new ObjectParameter("EventType", eventType) :
				new ObjectParameter("EventType", typeof(string));

			var eventSequenceParameter = eventSequence.HasValue ?
				new ObjectParameter("EventSequence", eventSequence) :
				new ObjectParameter("EventSequence", typeof(decimal));

			var eventOccurrenceParameter = eventOccurrence.HasValue ?
				new ObjectParameter("EventOccurrence", eventOccurrence) :
				new ObjectParameter("EventOccurrence", typeof(decimal));

			var eventCodeParameter = eventCode.HasValue ?
				new ObjectParameter("EventCode", eventCode) :
				new ObjectParameter("EventCode", typeof(int));

			var eventDetailCodeParameter = eventDetailCode.HasValue ?
				new ObjectParameter("EventDetailCode", eventDetailCode) :
				new ObjectParameter("EventDetailCode", typeof(int));

			var messageParameter = message != null ?
				new ObjectParameter("Message", message) :
				new ObjectParameter("Message", typeof(string));

			var applicationPathParameter = applicationPath != null ?
				new ObjectParameter("ApplicationPath", applicationPath) :
				new ObjectParameter("ApplicationPath", typeof(string));

			var applicationVirtualPathParameter = applicationVirtualPath != null ?
				new ObjectParameter("ApplicationVirtualPath", applicationVirtualPath) :
				new ObjectParameter("ApplicationVirtualPath", typeof(string));

			var machineNameParameter = machineName != null ?
				new ObjectParameter("MachineName", machineName) :
				new ObjectParameter("MachineName", typeof(string));

			var requestUrlParameter = requestUrl != null ?
				new ObjectParameter("RequestUrl", requestUrl) :
				new ObjectParameter("RequestUrl", typeof(string));

			var exceptionTypeParameter = exceptionType != null ?
				new ObjectParameter("ExceptionType", exceptionType) :
				new ObjectParameter("ExceptionType", typeof(string));

			var detailsParameter = details != null ?
				new ObjectParameter("Details", details) :
				new ObjectParameter("Details", typeof(string));

			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("aspnet_WebEvent_LogEvent", eventIdParameter, eventTimeUtcParameter, eventTimeParameter, eventTypeParameter, eventSequenceParameter, eventOccurrenceParameter, eventCodeParameter, eventDetailCodeParameter, messageParameter, applicationPathParameter, applicationVirtualPathParameter, machineNameParameter, requestUrlParameter, exceptionTypeParameter, detailsParameter);
		}
	}
}
