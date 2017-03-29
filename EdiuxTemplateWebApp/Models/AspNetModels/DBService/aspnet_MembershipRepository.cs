﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Security;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
	public class aspnet_MembershipRepository : EFRepository<aspnet_Membership>, Iaspnet_MembershipRepository
	{
		/// <summary>
		/// Update the specified aspnet_Membership.
		/// </summary>
		/// <returns>The update.</returns>
		/// <param name="aspnet_Membership">Aspnet membership.</param>
		public void Update(aspnet_Membership aspnet_Membership)
		{
			var foundMembership = Get(aspnet_Membership.UserId);

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
