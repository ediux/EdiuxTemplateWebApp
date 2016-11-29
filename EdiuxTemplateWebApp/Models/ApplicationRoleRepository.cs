using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models
{   
	public  partial class ApplicationRoleRepository : EFRepository<ApplicationRole>, IApplicationRoleRepository
	{

	}

	public  partial interface IApplicationRoleRepository : IRepositoryBase<ApplicationRole>
	{

	}
}