using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_UserLoginRepository : EFRepository<aspnet_UserLogin>, Iaspnet_UserLoginRepository
    {
       
    }

    public partial interface Iaspnet_UserLoginRepository : IRepositoryBase<aspnet_UserLogin>
    {
        //Task AddLoginAsync(aspnet_Users user, UserLoginInfo login);
        //void RemoveLoginAsync(aspnet_Users user, UserLoginInfo login);
        //Task<aspnet_Users> FindAsync(UserLoginInfo login);
        //Task<IList<UserLoginInfo>> GetLoginsAsync(aspnet_Users user);
        //aspnet_Users Find(UserLoginInfo login);
    }
}