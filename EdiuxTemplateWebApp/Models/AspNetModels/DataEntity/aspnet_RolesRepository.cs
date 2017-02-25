using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_RolesRepository : EFRepository<aspnet_Roles>, Iaspnet_RolesRepository
    {
        
    }

    public  partial interface Iaspnet_RolesRepository : IRepositoryBase<aspnet_Roles>
	{

	}
}