using System;
using System.Collections.Generic;
using System.Linq;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{

	public class aspnet_ApplicationsRepository : EFRepository<aspnet_Applications>, Iaspnet_ApplicationsRepository
	{
		/// <summary>
		/// Finds the by identifier.
		/// </summary>
		/// <returns>The by identifier.</returns>
		/// <param name="applicationId">Application identifier.</param>
		public IEnumerable<aspnet_Applications> FindById(Guid applicationId)
		{
			try
			{
				IEnumerable<aspnet_Applications> appInfos = All().Where(s => s.ApplicationId == applicationId);
				return appInfos;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Finds the name of the by.
		/// </summary>
		/// <returns>The by name.</returns>
		/// <param name="applicationName">Application name.</param>
		public IEnumerable<aspnet_Applications> FindByName(string applicationName)
		{
			try
			{
				string loweredApplicationName = applicationName.ToLowerInvariant();
				IEnumerable<aspnet_Applications> appInfo = All().Where(s => s.ApplicationName == applicationName
					|| s.LoweredApplicationName == loweredApplicationName);
				return appInfo;
			}
			catch
			{
				throw;
			}
		}

		//public void Update(aspnet_Applications entity)
		//{
		//	var foundApp = Get(entity.ApplicationId,entity.ApplicationName,entity.LoweredApplicationName);
		//	foundApp = CopyTo<aspnet_Applications>(entity);
		//	UnitOfWork.Commit();
		//}
	}

	public partial interface Iaspnet_ApplicationsRepository : IRepositoryBase<aspnet_Applications>
	{
		/// <summary>
		/// Finds the name of the by.
		/// </summary>
		/// <returns>The by name.</returns>
		/// <param name="applicationName">Application name.</param>
		IEnumerable<aspnet_Applications> FindByName(string applicationName);

		/// <summary>
		/// Finds the by identifier.
		/// </summary>
		/// <returns>The by identifier.</returns>
		/// <param name="applicationId">Application identifier.</param>
		IEnumerable<aspnet_Applications> FindById(Guid applicationId);

		/// <summary>
		/// Update the specified entity.
		/// </summary>
		/// <returns>The update.</returns>
		/// <param name="entity">Entity.</param>
		void Update(aspnet_Applications entity);
	}
}
