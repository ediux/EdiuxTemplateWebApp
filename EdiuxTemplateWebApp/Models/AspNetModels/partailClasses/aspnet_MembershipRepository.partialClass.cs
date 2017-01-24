using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_MembershipRepository : EFRepository<aspnet_Membership>, Iaspnet_MembershipRepository
    {
        public Task<aspnet_Membership> FindByEmailAsync(string email)
        {
            return Task.FromResult(Where(s => s.Email == email).SingleOrDefault());
        }
    }

    public  partial interface Iaspnet_MembershipRepository : IRepositoryBase<aspnet_Membership>
	{




    }
}