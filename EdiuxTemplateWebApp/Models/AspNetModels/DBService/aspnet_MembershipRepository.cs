using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Security;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
	public class aspnet_MembershipRepository : EFRepository<aspnet_Membership>, Iaspnet_MembershipRepository
	{
		/// <summary>
		/// All this instance.
		/// </summary>
		/// <returns>The all.</returns>
		public override IQueryable<aspnet_Membership> All()
		{
			UnitOfWork.LazyLoadingEnabled = false;

			var loadAllQueryable = (from m in ObjectSet
									join app in UnitOfWork.Set<aspnet_Applications>()
										on m.ApplicationId equals app.ApplicationId
									select m).AsQueryable();

			return loadAllQueryable;
		}

		/// <summary>
		/// Update the specified aspnet_Membership.
		/// </summary>
		/// <returns>The update.</returns>
		/// <param name="aspnet_Membership">Aspnet membership.</param>
		public void Update(aspnet_Membership aspnet_Membership)
		{
			var foundMembership = Get(aspnet_Membership.UserId);

			UnitOfWork.Repositories
						 .GetRepository<aspnet_ApplicationsRepository>()
						 .Update(aspnet_Membership.aspnet_Applications);

			foundMembership = CopyTo<aspnet_Membership>(aspnet_Membership);

			UnitOfWork.Entry(foundMembership).State = EntityState.Modified;
			UnitOfWork.Commit();
		}
	}

	public partial interface Iaspnet_MembershipRepository : IRepositoryBase<aspnet_Membership>
	{
		/// <summary>
		/// Update the specified aspnet_Membership.
		/// </summary>
		/// <returns>The update.</returns>
		/// <param name="aspnet_Membership">Aspnet membership.</param>
		void Update(aspnet_Membership aspnet_Membership);
	}
}
