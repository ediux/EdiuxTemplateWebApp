using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{   
	public  partial class aspnet_ProfileRepository : EFRepository<aspnet_Profile>, Iaspnet_ProfileRepository
	{

	}

	public  partial interface Iaspnet_ProfileRepository : IRepositoryBase<aspnet_Profile>
	{

	}
}