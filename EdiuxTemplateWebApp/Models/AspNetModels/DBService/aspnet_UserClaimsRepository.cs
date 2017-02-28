using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    	public  partial class aspnet_UserClaimsRepository : EFRepository<aspnet_UserClaims>, Iaspnet_UserClaimsRepository
    	{
    
    	}
    
    	public  partial interface Iaspnet_UserClaimsRepository : IRepositoryBase<aspnet_UserClaims>
    	{
    
    	}
}
