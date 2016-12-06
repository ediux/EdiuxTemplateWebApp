using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models
{   
	public  partial class System_ApplicationsRepository : EFRepository<System_Applications>, ISystem_ApplicationsRepository
	{

	}

	public  partial interface ISystem_ApplicationsRepository : IRepositoryBase<System_Applications>
	{

	}
}