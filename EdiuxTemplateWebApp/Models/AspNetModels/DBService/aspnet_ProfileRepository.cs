using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
	public partial class aspnet_ProfileRepository : EFRepository<aspnet_Profile>, Iaspnet_ProfileRepository
	{
		//public void Update(aspnet_Profile entity)
		//{
		//	var found = Get(entity.UserId);
		//	found = CopyTo<aspnet_Profile>(entity);
		//	UnitOfWork.Commit();
		//}
	}

	public partial interface Iaspnet_ProfileRepository : IRepositoryBase<aspnet_Profile>
	{
		void Update(aspnet_Profile entity);
	}
}
