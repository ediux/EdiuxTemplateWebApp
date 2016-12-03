using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace EdiuxTemplateWebApp.Models
{
    public partial class ApplicationUserLoginRepository : EFRepository<ApplicationUserLogin>, IApplicationUserLoginRepository
    {
        #region Login Store
        public Task AddLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                if (login == null)
                    throw new ArgumentNullException(nameof(login));

                IApplicationUserRepository userRepo = RepositoryHelper.GetApplicationUserRepository(UnitOfWork);

                ApplicationUser userInDb = userRepo.Get(user.Id);

                ApplicationUserLogin externloginInfo = new ApplicationUserLogin()
                {
                    LoginProvider = login.LoginProvider,
                    ProviderKey = login.ProviderKey,
                    UserId = user.Id
                };

                Add(externloginInfo);
                UnitOfWork.Commit();
                externloginInfo = Reload(externloginInfo);

                userInDb.ApplicationUserLogin.Add(externloginInfo);
                UnitOfWork.Context.Entry(userInDb).State = EntityState.Modified;
                UnitOfWork.Commit();

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task<ApplicationUser> FindAsync(UserLoginInfo login)
        {
            try
            {
                if (login == null)
                    throw new ArgumentNullException(nameof(login));

                ApplicationUserLogin externLogin = Get(login.LoginProvider, login.ProviderKey);
                if (externLogin != null)
                {
                    return Task.FromResult(externLogin.ApplicationUser);
                }

                return Task.FromResult(default(ApplicationUser));
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }

        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                List<ApplicationUserLogin> externLogins = All()
                     .Where(f => f.UserId == user.Id)
                     .ToList();

                if (externLogins != null)
                {
                    return Task.FromResult(externLogins
                        .ConvertAll(c => new UserLoginInfo(c.LoginProvider, c.ProviderKey))
                        as IList<UserLoginInfo>);
                }

                return Task.FromResult(
                    default(IList<UserLoginInfo>)
                    );
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }   
        }

        public async Task RemoveLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            var foundlogininfo = (from q in user.ApplicationUserLogin
                                  where q.LoginProvider == login.LoginProvider
                                  && q.ProviderKey == login.ProviderKey
                                  select q).Single();

            user.ApplicationUserLogin.Remove(foundlogininfo);
            UnitOfWork.Context.Entry(user).State = EntityState.Modified;
            await UnitOfWork.CommitAsync();

        }
        #endregion

    }

    public partial interface IApplicationUserLoginRepository : IRepositoryBase<ApplicationUserLogin>
    {
        #region Login Store
        Task AddLoginAsync(ApplicationUser user, UserLoginInfo login);

        Task<ApplicationUser> FindAsync(UserLoginInfo login);

        Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user);

        Task RemoveLoginAsync(ApplicationUser user, UserLoginInfo login);
        #endregion
    }
}