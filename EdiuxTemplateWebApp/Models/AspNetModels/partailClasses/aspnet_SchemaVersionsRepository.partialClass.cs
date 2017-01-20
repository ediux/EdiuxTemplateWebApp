using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{   
	public  partial class aspnet_SchemaVersionsRepository : EFRepository<aspnet_SchemaVersions>, Iaspnet_SchemaVersionsRepository
	{

	}

	public  partial interface Iaspnet_SchemaVersionsRepository : IRepositoryBase<aspnet_SchemaVersions>
	{

	}
}