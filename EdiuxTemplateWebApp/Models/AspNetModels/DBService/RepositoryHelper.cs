using System;
using System.Linq;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
	public static class RepositoryHelper
	{
		public static IUnitOfWork GetUnitOfWork()
		{
			return new EFUnitOfWork();
		}

		public static aspnet_ApplicationsRepository Getaspnet_ApplicationsRepository()
		{
			var repository = new aspnet_ApplicationsRepository();
			repository.UnitOfWork = GetUnitOfWork();

			return repository;
		}

		public static aspnet_ApplicationsRepository Getaspnet_ApplicationsRepository(IUnitOfWork unitOfWork)
		{
			var repository = new aspnet_ApplicationsRepository();
			repository.UnitOfWork = unitOfWork;
			return repository;
		}

		public static aspnet_MembershipRepository Getaspnet_MembershipRepository()
		{
			var repository = new aspnet_MembershipRepository();
			repository.UnitOfWork = GetUnitOfWork();
			return repository;
		}

		public static aspnet_MembershipRepository Getaspnet_MembershipRepository(IUnitOfWork unitOfWork)
		{
			var repository = new aspnet_MembershipRepository();
			repository.UnitOfWork = unitOfWork;
			return repository;
		}

		public static aspnet_PathsRepository Getaspnet_PathsRepository()
		{
			var repository = new aspnet_PathsRepository();
			repository.UnitOfWork = GetUnitOfWork();
			return repository;
		}

		public static aspnet_PathsRepository Getaspnet_PathsRepository(IUnitOfWork unitOfWork)
		{
			var repository = new aspnet_PathsRepository();
			repository.UnitOfWork = unitOfWork;
			return repository;
		}

		public static aspnet_PersonalizationAllUsersRepository Getaspnet_PersonalizationAllUsersRepository()
		{
			var repository = new aspnet_PersonalizationAllUsersRepository();
			repository.UnitOfWork = GetUnitOfWork();
			return repository;
		}

		public static aspnet_PersonalizationAllUsersRepository Getaspnet_PersonalizationAllUsersRepository(IUnitOfWork unitOfWork)
		{
			var repository = new aspnet_PersonalizationAllUsersRepository();
			repository.UnitOfWork = unitOfWork;
			return repository;
		}

		public static aspnet_PersonalizationPerUserRepository Getaspnet_PersonalizationPerUserRepository()
		{
			var repository = new aspnet_PersonalizationPerUserRepository();
			repository.UnitOfWork = GetUnitOfWork();
			return repository;
		}

		public static aspnet_PersonalizationPerUserRepository Getaspnet_PersonalizationPerUserRepository(IUnitOfWork unitOfWork)
		{
			var repository = new aspnet_PersonalizationPerUserRepository();
			repository.UnitOfWork = unitOfWork;
			return repository;
		}

		public static aspnet_ProfileRepository Getaspnet_ProfileRepository()
		{
			var repository = new aspnet_ProfileRepository();
			repository.UnitOfWork = GetUnitOfWork();
			return repository;
		}

		public static aspnet_ProfileRepository Getaspnet_ProfileRepository(IUnitOfWork unitOfWork)
		{
			var repository = new aspnet_ProfileRepository();
			repository.UnitOfWork = unitOfWork;
			return repository;
		}

		public static aspnet_RolesRepository Getaspnet_RolesRepository()
		{
			var repository = new aspnet_RolesRepository();
			repository.UnitOfWork = GetUnitOfWork();
			return repository;
		}

		public static aspnet_RolesRepository Getaspnet_RolesRepository(IUnitOfWork unitOfWork)
		{
			var repository = new aspnet_RolesRepository();
			repository.UnitOfWork = unitOfWork;
			return repository;
		}

		public static aspnet_SchemaVersionsRepository Getaspnet_SchemaVersionsRepository()
		{
			var repository = new aspnet_SchemaVersionsRepository();
			repository.UnitOfWork = GetUnitOfWork();
			return repository;
		}

		public static aspnet_SchemaVersionsRepository Getaspnet_SchemaVersionsRepository(IUnitOfWork unitOfWork)
		{
			var repository = new aspnet_SchemaVersionsRepository();
			repository.UnitOfWork = unitOfWork;
			return repository;
		}

		public static aspnet_UserClaimsRepository Getaspnet_UserClaimsRepository()
		{
			var repository = new aspnet_UserClaimsRepository();
			repository.UnitOfWork = GetUnitOfWork();
			return repository;
		}

		public static aspnet_UserClaimsRepository Getaspnet_UserClaimsRepository(IUnitOfWork unitOfWork)
		{
			var repository = new aspnet_UserClaimsRepository();
			repository.UnitOfWork = unitOfWork;
			return repository;
		}

		public static aspnet_UserLoginRepository Getaspnet_UserLoginRepository()
		{
			var repository = new aspnet_UserLoginRepository();
			repository.UnitOfWork = GetUnitOfWork();
			return repository;
		}

		public static aspnet_UserLoginRepository Getaspnet_UserLoginRepository(IUnitOfWork unitOfWork)
		{
			var repository = new aspnet_UserLoginRepository();
			repository.UnitOfWork = unitOfWork;
			return repository;
		}

		public static aspnet_UsersRepository Getaspnet_UsersRepository()
		{
			var repository = new aspnet_UsersRepository();
			repository.UnitOfWork = GetUnitOfWork();
			return repository;
		}

		public static aspnet_UsersRepository Getaspnet_UsersRepository(IUnitOfWork unitOfWork)
		{
			var repository = new aspnet_UsersRepository();
			repository.UnitOfWork = unitOfWork;
			return repository;
		}

		public static aspnet_VoidUsersRepository Getaspnet_VoidUsersRepository()
		{
			var repository = new aspnet_VoidUsersRepository();
			repository.UnitOfWork = GetUnitOfWork();
			return repository;
		}

		public static aspnet_VoidUsersRepository Getaspnet_VoidUsersRepository(IUnitOfWork unitOfWork)
		{
			var repository = new aspnet_VoidUsersRepository();
			repository.UnitOfWork = unitOfWork;
			return repository;
		}

		public static aspnet_WebEvent_EventsRepository Getaspnet_WebEvent_EventsRepository()
		{
			var repository = new aspnet_WebEvent_EventsRepository();
			repository.UnitOfWork = GetUnitOfWork();
			return repository;
		}

		public static aspnet_WebEvent_EventsRepository Getaspnet_WebEvent_EventsRepository(IUnitOfWork unitOfWork)
		{
			var repository = new aspnet_WebEvent_EventsRepository();
			repository.UnitOfWork = unitOfWork;
			return repository;
		}

		public static MenusRepository GetMenusRepository()
		{
			var repository = new MenusRepository();
			repository.UnitOfWork = GetUnitOfWork();
			return repository;
		}

		public static MenusRepository GetMenusRepository(IUnitOfWork unitOfWork)
		{
			var repository = new MenusRepository();
			repository.UnitOfWork = unitOfWork;
			return repository;
		}

		public static aspnet_Applications FindByName(this Iaspnet_ApplicationsRepository appRepo, string applicationName)
		{
			try
			{
				return appRepo.FindByName(applicationName).SingleOrDefault();
			}
			catch
			{
				throw;
			}
		}

		public static aspnet_Users GetUserById(this aspnet_Membership membership)
		{
			try
			{
				Iaspnet_UsersRepository userRepo = Getaspnet_UsersRepository();
				return userRepo.Where(s => s.ApplicationId == membership.ApplicationId &&
									  s.Id == membership.UserId).SingleOrDefault();
			}
			catch
			{
				throw;
			}
		}

		public static aspnet_Users Add(this Iaspnet_UsersRepository userRepo, string userName, string password, aspnet_Applications applicationObject, string eMail = "@abc.com", bool IsUserAnonymous = false)
		{
			try
			{
				//string passwordSalt = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
				//PasswordHasher hasher = new PasswordHasher();
				//password = hasher.HashPassword(password + passwordSalt);
				//Guid userId = Guid.Empty;

				var user = new aspnet_Users();

				user.ApplicationId = applicationObject.ApplicationId;
				user.IsAnonymous = IsUserAnonymous;
				user.LoweredUserName = userName.ToLowerInvariant();
				user.UserName = userName;

				return userRepo.Add(user);

			}
			catch
			{
				throw;
			}
		}

		public static aspnet_Users GetUserByName(this Iaspnet_UsersRepository repo, string AppName, string userName)
		{
			aspnet_Membership_GetUserByName_InputParameter inputParam = new aspnet_Membership_GetUserByName_InputParameter();

			inputParam.ApplicationName = AppName;
			inputParam.CurrentTimeUtc = DateTime.UtcNow;
			inputParam.UpdateLastActivity = true;
			inputParam.UserName = userName;

			var rootUser = repo.UnitOfWork.GetTypedContext<AspNetDbEntities2>().aspnet_Membership_GetUserByName(inputParam).SingleOrDefault();

			aspnet_Users user = repo.ConvertFrom(rootUser);

			return user;
		}

		public static aspnet_Users GetUserByName(this aspnet_Applications AppInfo, string userName, Iaspnet_ApplicationsRepository appRepository = null)
		{
			Iaspnet_UsersRepository repo = null;

			if (appRepository == null)
				repo = Getaspnet_UsersRepository();

			var rootUser = GetUserByName(repo, AppInfo.ApplicationName, userName);

			aspnet_Users user = repo.ConvertFrom(rootUser);

			return user;
		}

		public static bool IsInRole(this Iaspnet_RolesRepository roleRpoe, string AppName, string userName, string roleName)
		{
			aspnet_UsersInRoles_IsUserInRole_InputParameter inputParam = new aspnet_UsersInRoles_IsUserInRole_InputParameter();
			inputParam.applicationName = AppName;
			inputParam.roleName = roleName;
			inputParam.userName = userName;
			roleRpoe.UnitOfWork.GetTypedContext<AspNetDbEntities2>().aspnet_UsersInRoles_IsUserInRole(inputParam);
			return inputParam.ReturnValue == 1;
		}

		public static bool IsInRole(this aspnet_Users user, string roleName, Iaspnet_RolesRepository repo = null)
		{
			if (repo == null)
				repo = Getaspnet_RolesRepository();

			return IsInRole(repo, user.aspnet_Applications.ApplicationName, user.UserName, roleName);
		}

		public static void AddToRole(this Iaspnet_RolesRepository roleRepo, string appName, string userName, string RoleName)
		{
			aspnet_UsersInRoles_AddUsersToRoles_InputParameter inputParam = new AspNetModels.aspnet_UsersInRoles_AddUsersToRoles_InputParameter();

			inputParam.applicationName = appName;
			inputParam.currentTimeUtc = DateTime.UtcNow;
			inputParam.roleNames = RoleName;
			inputParam.userNames = userName;

			roleRepo.UnitOfWork.GetTypedContext<AspNetDbEntities2>().aspnet_UsersInRoles_AddUsersToRoles(inputParam);

		}

		public static void AddToRole(this aspnet_Users user, string RoleName, Iaspnet_RolesRepository roleRepo = null)
		{
			if (roleRepo == null)
				roleRepo = Getaspnet_RolesRepository();

			AddToRole(roleRepo, user.aspnet_Applications.ApplicationName, user.UserName, RoleName);
		}
	}
}
