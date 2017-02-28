using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    	public  partial class aspnet_PathsRepository : EFRepository<aspnet_Paths>, Iaspnet_PathsRepository
    	{
    
    	}
    
    	public  partial interface Iaspnet_PathsRepository : IRepositoryBase<aspnet_Paths>
    	{
    
    	}
}
