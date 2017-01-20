using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{   
	public  partial class aspnet_VoidUsersRepository : EFRepository<aspnet_VoidUsers>, Iaspnet_VoidUsersRepository
	{

	}

	public  partial interface Iaspnet_VoidUsersRepository : IRepositoryBase<aspnet_VoidUsers>
	{

	}
}