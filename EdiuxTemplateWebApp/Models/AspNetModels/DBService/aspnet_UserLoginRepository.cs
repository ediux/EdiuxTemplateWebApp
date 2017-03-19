using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
	public partial class aspnet_UserLoginRepository : EFRepository<aspnet_UserLogin>, Iaspnet_UserLoginRepository
	{
		public void Update(aspnet_UserLogin entity)
		{
			var found = Get(entity.UserId);
			found = CopyTo<aspnet_UserLogin>(entity);
			UnitOfWork.Commit();
		}
	}

	public partial interface Iaspnet_UserLoginRepository : IRepositoryBase<aspnet_UserLogin>
	{
		void Update(aspnet_UserLogin entity);
	}
}
