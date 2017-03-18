using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
	public partial class aspnet_UsersRepository : EFRepository<aspnet_Users>, Iaspnet_UsersRepository
	{
		public IQueryable<aspnet_Users> All(aspnet_Applications application)
		{
			try
			{

				IQueryable<aspnet_Users> query =
					from u in ObjectSet
					from ub in (from r in UnitOfWork.Set<aspnet_Roles>()
								from ruu in r.aspnet_Users
								where r.ApplicationId == u.ApplicationId
								select ruu).Distinct()
					join m in UnitOfWork.Set<aspnet_Membership>() on u.Id equals m.UserId
					join ppu in UnitOfWork.Set<aspnet_PersonalizationPerUser>() on u.Id equals ppu.UserId
					join p in UnitOfWork.Set<aspnet_Profile>() on u.Id equals p.UserId
					where ub.Id == u.Id && u.ApplicationId == application.ApplicationId
					select u;


				query.Load();

				return query;
			}
			catch (Exception ex)
			{
				WriteErrorLog(ex);
				throw ex;
			}

		}

		public override IQueryable<aspnet_Users> All()
		{
			try
			{
				UnitOfWork.LazyLoadingEnabled = false;

				var apps = UnitOfWork.Set<aspnet_Applications>();
				var roles = UnitOfWork.Set<aspnet_Roles>();
				var memberships = UnitOfWork.Set<aspnet_Membership>();
				var profiles = UnitOfWork.Set<aspnet_Profile>();
				var ppu = UnitOfWork.Set<aspnet_PersonalizationPerUser>();
				var externLogins = UnitOfWork.Set<aspnet_UserLogin>();
				var externClaims = UnitOfWork.Set<aspnet_UserClaims>();

				apps.Load();
				roles.Load();
				memberships.Load();
				profiles.Load();
				ppu.Load();
				externLogins.Load();
				externClaims.Load();

				var rolequery = (from r in roles
								 from uir in r.aspnet_Users
								 where ObjectSet.Contains(uir)
								 select new { r.ApplicationId, RoleId = r.Id, r.Name, r.LoweredRoleName, UserId = uir.Id })
					.Distinct();

				IQueryable<aspnet_Users> query =
					from u in ObjectSet
					join app in apps on u.ApplicationId equals app.ApplicationId
					join m in memberships on u.Id equals m.UserId
					join p in profiles on u.Id equals p.UserId
					join userpp in ppu on u.Id equals userpp.UserId
					join ul in externLogins on u.Id equals ul.UserId
					join uc in externClaims on u.Id equals uc.UserId
					join r in rolequery on u.Id equals r.UserId
					select u;


				query.Load();

				return query;
			}
			catch (Exception ex)
			{
				WriteErrorLog(ex);
				throw ex;
			}

		}

		public override void Delete(aspnet_Users entity)
		{
			try
			{
				var inputParam = new aspnet_Users_DeleteUser_InputParameter();

				inputParam.ApplicationName = entity.aspnet_Applications.ApplicationName;
				inputParam.TablesToDeleteFrom = (int)(TablesToCheck.aspnet_Membership | TablesToCheck.aspnet_Profile | TablesToCheck.aspnet_Roles);
				inputParam.UserName = entity.UserName;

				UnitOfWork.GetTypedContext<AspNetDbEntities2>().aspnet_Users_DeleteUser(inputParam);
			}
			catch (Exception ex)
			{
				WriteErrorLog(ex);
				throw ex;
			}
		}

		public override aspnet_Users Add(aspnet_Users entity)
		{

			try
			{
				var inputParam = new aspnet_Users_CreateUser_InputParameter();

				inputParam.applicationId = entity.ApplicationId;
				inputParam.isUserAnonymous = entity.IsAnonymous;
				inputParam.lastActivityDate = entity.LastActivityDate;
				inputParam.userName = entity.UserName;

				UnitOfWork.GetTypedContext<AspNetDbEntities2>().aspnet_Users_CreateUser(inputParam);

				if (inputParam.ReturnValue == (int)System.Web.Security.MembershipCreateStatus.Success)
				{
					return Get(inputParam.OutputParameter.UserId);
				}

				throw new Exception(string.Format("Provider Error.(ErrorCode:{0})", inputParam.ReturnValue));

			}
			catch (Exception ex)
			{
				WriteErrorLog(ex);
				throw ex;
			}

		}



		public IEnumerable<aspnet_Membership_FindUsersByName_Result> FindByName(string applicationName, string userNameToMatch, int pageIndex, int pageSize)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<aspnet_Membership_FindUsersByName_Result> FindByEmail(string applicationName, string EmailToMatch, int pageIndex, int pageSize)
		{
			throw new NotImplementedException();
		}

		public aspnet_Membership_GetUserByName_Result GetUserByName(string applicationName, string userName, DateTime currentTimeUtc, bool updateLastActivity)
		{
			throw new NotImplementedException();
		}

		public aspnet_Membership_GetUserByEmail_Result GetUserByEmail(string applicationName, string eMail)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<aspnet_Membership_GetAllUsers_Result> GetAllUsers(string applicationName, int pageIndex, int pageSize, out int totalRecords)
		{
			throw new NotImplementedException();
		}

		public aspnet_Membership_GetUserByUserId_Result GetUserByUserId(Guid userId, DateTime currentTimeUtc, bool updateLastActivity)
		{
			throw new NotImplementedException();
		}

		public int getNumberOfUsersOnline(string applicationName, int minutesSinceLastInActive, DateTime currentTimeUtc)
		{
			throw new NotImplementedException();
		}

		public void AddToRole(aspnet_Users user, string roleName)
		{
			var roleRepo = UnitOfWork.Repositories.GetRepository<Iaspnet_RolesRepository>();
			var loweredRoleName = roleName.ToLowerInvariant();

			var roles = roleRepo
				.Where(s => s.ApplicationId == user.ApplicationId &&
					   (s.Name == roleName
						|| s.LoweredRoleName == loweredRoleName));

			if (!roles.SelectMany(s => s.aspnet_Users).Where(w => w.Id == user.Id).Any())
			{
				var existedUser = Get(user.Id);
				var loweredUserName = user.UserName.ToLowerInvariant();

				var foundrole = roleRepo.Where(w => w.ApplicationId == user.ApplicationId
											   && (w.Name == roleName
												   || w.LoweredRoleName == loweredRoleName))
										.SingleOrDefault();

				if (foundrole != null)
				{
					existedUser.aspnet_Roles.Add(foundrole);
					UnitOfWork.Commit();
				}
			}
		}

		public bool IsInRole(aspnet_Users user, string roleName)
		{
			var paramObject = new aspnet_UsersInRoles_IsUserInRole_InputParameter();

			paramObject.applicationName = user.aspnet_Applications.ApplicationName;
			paramObject.roleName = roleName;
			paramObject.userName = user.UserName;

			if (paramObject.ReturnValue == 1)
			{
				return true;
			}

			return false;
		}


		//public void AddToRole(string applicationName, string userName, string roleName)
		//{
		//	InternalDatabaseAlias.aspnet_UsersInRoles_AddUsersToRoles(applicationName, userName, roleName, DateTime.UtcNow);
		//}

		//public IList<string> FindUsersInRole(string applicationName, string UserNameToMatch, string roleName)
		//{
		//	return InternalDatabaseAlias.aspnet_UsersInRoles_FindUsersInRole(applicationName, UserNameToMatch, roleName)
		//		.Select(s => s.UserName).ToList();
		//}

		//public bool IsInRole(string applicationName, string userName, string roleName)
		//{

		//	try
		//	{
		//		var applicationNameParameter = applicationName != null ?
		//		new SqlParameter("@ApplicationName", applicationName) :
		//		new SqlParameter("@ApplicationName", SqlDbType.NVarChar, 256);

		//		var userNameParameter = userName != null ?
		//			new SqlParameter("@UserName", userName) :
		//			new SqlParameter("@UserName", SqlDbType.NVarChar, 256);

		//		var roleNameParameter = roleName != null ?
		//			new SqlParameter("@RoleName", roleName) :
		//			new SqlParameter("@RoleName", SqlDbType.NVarChar, 256);

		//		var returnCode = new SqlParameter();
		//		returnCode.ParameterName = "@return_value";
		//		returnCode.SqlDbType = SqlDbType.Int;
		//		returnCode.Direction = ParameterDirection.Output;

		//		int code = 0;

		//		var result = UnitOfWork.Context.Database.ExecuteSqlCommand("EXEC @return_value = [dbo].[aspnet_UsersInRoles_IsUserInRole] @ApplicationName, @UserName, @RoleName", applicationNameParameter, userNameParameter, roleNameParameter, returnCode);
		//		// ((IObjectContextAdapter)UnitOfWork.Context).ObjectContext.ExecuteFunction("aspnet_UsersInRoles_IsUserInRole", applicationNameParameter, userNameParameter, roleNameParameter,returnCode);

		//		code = (int)returnCode.Value;
		//		return (code == 1);
		//	}
		//	catch (Exception ex)
		//	{
		//		WriteErrorLog(ex);
		//		throw ex;
		//	}

		//}

		//public IList<string> GetRolesForUser(string applicationName, string userName)
		//{

		//	try
		//	{
		//		return InternalDatabaseAlias.aspnet_UsersInRoles_GetRolesForUser(applicationName, userName)
		//			.Select(s => s.RoleName).ToList();
		//	}
		//	catch (Exception ex)
		//	{
		//		WriteErrorLog(ex);
		//		throw ex;
		//	}

		//}

		//private static void updateCache(aspnet_Applications applicationObject)
		//{
		//	try
		//	{
		//		MemoryCache.Default.Set("ApplicationInfo", applicationObject, DateTime.UtcNow.AddMinutes(38400));
		//	}
		//	catch (Exception ex)
		//	{
		//		throw ex;
		//	}

		//}

		//public aspnet_Users FindByName(string applicationName, string userName)
		//{
		//	try
		//	{
		//		Iaspnet_ApplicationsRepository appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository(UnitOfWork);
		//		var app = appRepo.FindByName(applicationName);
		//		var loweredUserName = userName.ToLowerInvariant();
		//		if (app == null)
		//		{
		//			return null;
		//		}
		//		var users = IRepositoryBase.Where(this, s => s.ApplicationId == app.ApplicationId
		//		 && (s.UserName == userName || s.LoweredUserName == loweredUserName)).OrderByDescending(o => o.LastActivityDate);

		//		return users.FirstOrDefault();
		//	}
		//	catch (Exception ex)
		//	{
		//		WriteErrorLog(ex);
		//		throw;
		//	}

		//}


	}

	public partial interface Iaspnet_UsersRepository : IRepositoryBase<aspnet_Users>
	{
		/// <summary>
		/// Finds the name of the by.
		/// </summary>
		/// <returns>The by name.</returns>
		/// <param name="applicationName">Application name.</param>
		/// <param name="userNameToMatch">User name to match.</param>
		/// <param name="pageIndex">Page index.</param>
		/// <param name="pageSize">Page size.</param>
		IEnumerable<aspnet_Membership_FindUsersByName_Result> FindByName(string applicationName, string userNameToMatch, int pageIndex, int pageSize);

		/// <summary>
		/// Finds the by email.
		/// </summary>
		/// <returns>The by email.</returns>
		/// <param name="applicationName">Application name.</param>
		/// <param name="EmailToMatch">Email to match.</param>
		/// <param name="pageIndex">Page index.</param>
		/// <param name="pageSize">Page size.</param>
		IEnumerable<aspnet_Membership_FindUsersByName_Result> FindByEmail(string applicationName, string EmailToMatch, int pageIndex, int pageSize);

		/// <summary>
		/// Gets the name of the user by.
		/// </summary>
		/// <returns>The user by name.</returns>
		/// <param name="applicationName">Application name.</param>
		/// <param name="userName">User name.</param>
		/// <param name="currentTimeUtc">Current time UTC.</param>
		/// <param name="updateLastActivity">If set to <c>true</c> update last activity.</param>
		aspnet_Membership_GetUserByName_Result GetUserByName(string applicationName, string userName, DateTime currentTimeUtc, bool updateLastActivity);

		/// <summary>
		/// Gets the user by email.
		/// </summary>
		/// <returns>The user by email.</returns>
		/// <param name="applicationName">Application name.</param>
		/// <param name="eMail">E mail.</param>
		aspnet_Membership_GetUserByEmail_Result GetUserByEmail(string applicationName, string eMail);


		/// <summary>
		/// Gets all users.
		/// </summary>
		/// <returns>The all users.</returns>
		/// <param name="applicationName">Application name.</param>
		/// <param name="pageIndex">Page index.</param>
		/// <param name="pageSize">Page size.</param>
		/// <param name="totalRecords">Total records.</param>
		IEnumerator<aspnet_Membership_GetAllUsers_Result> GetAllUsers(string applicationName, int pageIndex, int pageSize, out int totalRecords);

		/// <summary>
		/// Gets the user by user identifier.
		/// </summary>
		/// <returns>The user by user identifier.</returns>
		/// <param name="userId">User identifier.</param>
		/// <param name="currentTimeUtc">Current time UTC.</param>
		/// <param name="updateLastActivity">If set to <c>true</c> update last activity.</param>
		aspnet_Membership_GetUserByUserId_Result GetUserByUserId(Guid userId, DateTime currentTimeUtc, bool updateLastActivity);

		/// <summary>
		/// Gets the number of users online.
		/// </summary>
		/// <returns>The number of users online.</returns>
		/// <param name="applicationName">Application name.</param>
		/// <param name="minutesSinceLastInActive">Minutes since last in active.</param>
		/// <param name="currentTimeUtc">Current time UTC.</param>
		int getNumberOfUsersOnline(string applicationName, int minutesSinceLastInActive, DateTime currentTimeUtc);

		/// <summary>
		/// Adds to role.
		/// </summary>
		/// <param name="user">User.</param>
		/// <param name="roleName">Role name.</param>
		void AddToRole(aspnet_Users user, string roleName);

		/// <summary>
		/// Ises the in role.
		/// </summary>
		/// <returns><c>true</c>, if in role was ised, <c>false</c> otherwise.</returns>
		/// <param name="user">User.</param>
		/// <param name="roleName">Role name.</param>
		bool IsInRole(aspnet_Users user, string roleName);
		//IQueryable<aspnet_Users> All(aspnet_Applications application);
		//void AddToRole(string applicationName, string userName, string roleName);
		//bool IsInRole(string applicationName, string userName, string roleName);
		//IList<string> FindUsersInRole(string applicationName, string UserNameToMatch, string roleName);
		//IList<string> GetRolesForUser(string applicationName, string userName);
		//aspnet_Users FindByName(string applicationName, string userName);
	}
}
