using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models
{   
	public  partial class MenusRepository : EFRepository<Menus>, IMenusRepository
	{

	}

	public  partial interface IMenusRepository : IRepositoryBase<Menus>
	{

	}
}