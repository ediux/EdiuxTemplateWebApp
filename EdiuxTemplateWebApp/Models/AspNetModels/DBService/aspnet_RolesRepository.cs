using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
	public partial class aspnet_RolesRepository : EFRepository<aspnet_Roles>, Iaspnet_RolesRepository
	{
		public override aspnet_Roles Add(aspnet_Roles entity)
		{
			try
			{
				var paramObject = new aspnet_Roles_CreateRole_InputParameter();

				paramObject.applicationName = entity?.aspnet_Applications.ApplicationName;
				paramObject.roleName = entity.Name;

				UnitOfWork.GetTypedContext<AspNetDbEntities2>().aspnet_Roles_CreateRole(paramObject);

				return Get(entity.Id);
			}
			catch (Exception ex)
			{
				WriteErrorLog(ex);
				throw ex;
			}
		}

		public override IQueryable<aspnet_Roles> All()
		{
			return base.All();
		}

		//public aspnet_Roles Add(aspnet_Applications application, string Name, string Desctiption = "")
		//{
		//	var _role = new aspnet_Roles();

		//	_role.ApplicationId = application.ApplicationId;
		//	_role.LoweredRoleName = Name.ToLowerInvariant();
		//	_role.Name = Name;
		//	_role.Description = Desctiption;

		//	return Add(_role);
		//}

		public override void Delete(aspnet_Roles entity)
		{
			try
			{
				var returnCode = UnitOfWork.GetTypedContext<AspNetDbEntities2>()
										   .aspnet_Roles_DeleteRole(
											   entity.aspnet_Applications.ApplicationName,
											   entity.Name,
											   true);

				if (returnCode != 0)
					throw new Exception("Has an error in database.");
			}
			catch (Exception ex)
			{
				WriteErrorLog(ex);
				throw;
			}
		}

		public IEnumerator<aspnet_Roles> FindById(Guid applicationId, Guid roleId)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<aspnet_Roles> FindByName(Guid applicationId, string Name)
		{
			throw new NotImplementedException();
		}



		//public aspnet_Roles FindById(Guid applicationId, Guid roleId)
		//{
		//	try
		//	{
		//		return All().SingleOrDefault(s => s.ApplicationId == applicationId && s.Id == roleId);
		//	}
		//	catch (Exception ex)
		//	{
		//		WriteErrorLog(ex);
		//		throw ex;
		//	}
		//}
		//public aspnet_Roles FindByName(Guid applicationId, string Name)
		//{
		//	try
		//	{
		//		string loweredName = Name.ToLowerInvariant();
		//		return All().SingleOrDefault(s => s.ApplicationId == applicationId && (s.Name == Name || s.LoweredRoleName == loweredName));
		//	}
		//	catch (Exception ex)
		//	{
		//		WriteErrorLog(ex);
		//		throw ex;
		//	}
		//}


	}

	public partial interface Iaspnet_RolesRepository : IRepositoryBase<aspnet_Roles>
	{
		/// <summary>
		/// Finds the by identifier.
		/// </summary>
		/// <returns>The by identifier.</returns>
		/// <param name="applicationId">Application identifier.</param>
		/// <param name="roleId">Role identifier.</param>
		IEnumerator<aspnet_Roles> FindById(Guid applicationId, Guid roleId);

		/// <summary>
		/// Finds the name of the by.
		/// </summary>
		/// <returns>The by name.</returns>
		/// <param name="applicationId">Application identifier.</param>
		/// <param name="Name">Name.</param>
		IEnumerator<aspnet_Roles> FindByName(Guid applicationId, string Name);
	}
}
