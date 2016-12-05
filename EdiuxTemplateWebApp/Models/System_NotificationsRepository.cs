using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models
{   
	public  partial class System_NotificationsRepository : EFRepository<System_Notifications>, ISystem_NotificationsRepository
	{

	}

	public  partial interface ISystem_NotificationsRepository : IRepositoryBase<System_Notifications>
	{

	}
}