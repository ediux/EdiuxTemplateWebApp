using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
	public partial class aspnet_PersonalizationPerUserRepository : EFRepository<aspnet_PersonalizationPerUser>, Iaspnet_PersonalizationPerUserRepository
	{
		//public void Update(aspnet_PersonalizationPerUser entity)
		//{
		//	var found = Get(entity.PathId);
		//	found = CopyTo<aspnet_PersonalizationPerUser>(entity);
		//	UnitOfWork.Commit();
		//}
	}

	public partial interface Iaspnet_PersonalizationPerUserRepository : IRepositoryBase<aspnet_PersonalizationPerUser>
	{
		void Update(aspnet_PersonalizationPerUser entity);
	}
}
