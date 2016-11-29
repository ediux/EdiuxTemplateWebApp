using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models
{   
	public  partial class System_ControllersRepository : EFRepository<System_Controllers>, ISystem_ControllersRepository
	{

	}

	public  partial interface ISystem_ControllersRepository : IRepositoryBase<System_Controllers>
	{

	}
}