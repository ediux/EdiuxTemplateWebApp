using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
	public partial class aspnet_VoidUsersRepository : EFRepository<aspnet_VoidUsers>, Iaspnet_VoidUsersRepository
	{
		public void Update(aspnet_VoidUsers entity)
		{
			var foundPath = Get(entity.UserId, entity.UserName, entity.LoweredUserName);
			foundPath = CopyTo<aspnet_VoidUsers>(entity);
			UnitOfWork.Commit();
		}
	}

	public partial interface Iaspnet_VoidUsersRepository : IRepositoryBase<aspnet_VoidUsers>
	{
		void Update(aspnet_VoidUsers entity);
	}
}
