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

                if (paramObject.ReturnValue == 0)
                {
                    
                }
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

		public IEnumerable<aspnet_Roles> FindById(Guid applicationId, Guid roleId)
		{
            var roles = Where(s => s.ApplicationId == applicationId && s.Id == roleId);
            return roles.AsEnumerable();
		}

		public IEnumerable<aspnet_Roles> FindByName(Guid applicationId, string Name)
		{
            var roles = Where(s => s.ApplicationId == applicationId
                              && (s.Name == Name
                                  || s.LoweredRoleName == Name));
            
            return roles.AsEnumerable();
		}

        public bool IsExists(aspnet_Roles role)
        {
            throw new NotImplementedException();
        }

        public void Update(aspnet_Roles entity)
		{
			var foundPath = Get(entity.Id, entity.ApplicationId);
			foundPath = CopyTo<aspnet_Roles>(entity);
			UnitOfWork.Commit();
		}

	}

	public partial interface Iaspnet_RolesRepository : IRepositoryBase<aspnet_Roles>
	{
		/// <summary>
		/// Finds the by identifier.
		/// </summary>
		/// <returns>The by identifier.</returns>
		/// <param name="applicationId">Application identifier.</param>
		/// <param name="roleId">Role identifier.</param>
        IEnumerable<aspnet_Roles> FindById(Guid applicationId, Guid roleId);

		/// <summary>
		/// Finds the name of the by.
		/// </summary>
		/// <returns>The by name.</returns>
		/// <param name="applicationId">Application identifier.</param>
		/// <param name="Name">Name.</param>
		IEnumerable<aspnet_Roles> FindByName(Guid applicationId, string Name);

        /// <summary>
        /// 指出角色是否存在?
        /// </summary>
        /// <param name="role">查詢的角色</param>
        /// <returns>True:存在 Fale:不存在</returns>
        bool IsExists(aspnet_Roles role);

		/// <summary>
		/// Update the specified entity.
		/// </summary>
		/// <returns>The update.</returns>
		/// <param name="entity">Entity.</param>
		void Update(aspnet_Roles entity);
	}
}
