using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{   
	public  partial class aspnet_WebEvent_EventsRepository : EFRepository<aspnet_WebEvent_Events>, Iaspnet_WebEvent_EventsRepository
	{

	}

	public  partial interface Iaspnet_WebEvent_EventsRepository : IRepositoryBase<aspnet_WebEvent_Events>
	{

	}
}