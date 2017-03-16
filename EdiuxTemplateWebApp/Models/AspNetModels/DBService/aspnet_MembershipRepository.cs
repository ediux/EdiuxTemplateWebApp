using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Security;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_MembershipRepository : EFRepository<aspnet_Membership>, Iaspnet_MembershipRepository
    {
       
    }

    public partial interface Iaspnet_MembershipRepository : IRepositoryBase<aspnet_Membership>
    {
       
    }
}
