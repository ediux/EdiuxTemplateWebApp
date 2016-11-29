using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models
{   
	public  partial class ApplicationUserRepository : EFRepository<ApplicationUser>, IApplicationUserRepository
	{

	}

	public  partial interface IApplicationUserRepository : IRepositoryBase<ApplicationUser>
	{

	}
}