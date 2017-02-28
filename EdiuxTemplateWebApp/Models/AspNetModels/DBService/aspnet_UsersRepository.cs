using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    	public  partial class aspnet_UsersRepository : EFRepository<aspnet_Users>, Iaspnet_UsersRepository
    	{
    
    	}
    
    	public  partial interface Iaspnet_UsersRepository : IRepositoryBase<aspnet_Users>
    	{
    
    	}
}
