using System;

namespace EdiuxTemplateWebApp.Models.AspNetModels.DataEntity
{
	public static class RepositoryHelper
	{
		public static IUnitOfWork GetUnitOfWork()
		{
			return new EFUnitOfWork();
		}

		public static TUnitOfWork GetUnitOfWork<TUnitOfWork>() where TUnitOfWork : IUnitOfWork
        {
            return Activator.CreateInstance<TUnitOfWork>();
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
	}
}