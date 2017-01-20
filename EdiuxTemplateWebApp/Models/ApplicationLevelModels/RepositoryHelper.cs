using System;

namespace EdiuxTemplateWebApp.Models.ApplicationLevelModels
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