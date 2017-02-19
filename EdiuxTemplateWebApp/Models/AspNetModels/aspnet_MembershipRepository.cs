using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_MembershipRepository : EFRepository<aspnet_Membership>, Iaspnet_MembershipRepository
    {
        public int GetNumberOfUsersOnline(string applicationName, int MinutesSinceLastInActive, DateTime CurrentTimeUtc)
        {
            throw new NotImplementedException();
        }
    }

    public  partial interface Iaspnet_MembershipRepository : IRepositoryBase<aspnet_Membership>
	{

	}
}