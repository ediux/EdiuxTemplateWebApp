using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{   
	public  partial class aspnet_PersonalizationPerUserRepository : EFRepository<aspnet_PersonalizationPerUser>, Iaspnet_PersonalizationPerUserRepository
	{

	}

	public  partial interface Iaspnet_PersonalizationPerUserRepository : IRepositoryBase<aspnet_PersonalizationPerUser>
	{

	}
}