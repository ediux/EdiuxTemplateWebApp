using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{   
	public  partial class aspnet_PersonalizationAllUsersRepository : EFRepository<aspnet_PersonalizationAllUsers>, Iaspnet_PersonalizationAllUsersRepository
	{

	}

	public  partial interface Iaspnet_PersonalizationAllUsersRepository : IRepositoryBase<aspnet_PersonalizationAllUsers>
	{

	}
}