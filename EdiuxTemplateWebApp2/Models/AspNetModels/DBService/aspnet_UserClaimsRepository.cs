using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
	public partial class aspnet_UserClaimsRepository : EFRepository<aspnet_UserClaims>, Iaspnet_UserClaimsRepository
	{
		public void Update(aspnet_UserClaims entity)
		{
			var found = Get(entity.UserId);
			found = CopyTo<aspnet_UserClaims>(entity);
			UnitOfWork.Commit();
		}
	}

	public partial interface Iaspnet_UserClaimsRepository : IRepositoryBase<aspnet_UserClaims>
	{
		void Update(aspnet_UserClaims entity);
	}
}
