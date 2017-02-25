using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_ApplicationsRepository : EFRepository<aspnet_Applications>, Iaspnet_ApplicationsRepository
    {

    }

    public  partial interface Iaspnet_ApplicationsRepository : IRepositoryBase<aspnet_Applications>
	{

    }
}