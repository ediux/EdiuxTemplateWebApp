using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using System.Linq.Expressions;

namespace EdiuxTemplateWebApp.Models
{
    public partial class ApplicationUserRepository : EFRepository<ApplicationUser>, IApplicationUserRepository

    {
#if CS6
        private const string KeyName = nameof(ApplicationUser);
#else
        private static string KeyName = typeof(ApplicationUser).Name;
#endif

        #region Override of Base

        protected ApplicationUser ChangeBeforeReactive(ApplicationUser entity)
        {
            entity.Void = false;
            entity.LockoutEnabled = false;
            entity.LockoutEndDate = DateTime.UtcNow;

            Task<ApplicationUser> getRootUserTask = FindByNameAsync("root");
            getRootUserTask.Wait();

            if (getRootUserTask.Result != null)
            {
                entity.LastUpdateUserId = getRootUserTask.Result.Id;
            }
            else
            {
                entity.LastUpdateUserId = 0;
            }

            entity.LastUpdateTime = DateTime.UtcNow;

            //更新資料庫
            Task updateTask = UpdateAsync(entity);
            updateTask.Wait();

            //從資料庫重新載入
            entity = Reload(entity);

            return entity;
        }
        public override ApplicationUser Add(ApplicationUser entity)
        {


        }

        public override void Delete(ApplicationUser entity)
        {


        }
        #endregion

        #region User Store
        public async Task CreateAsync(ApplicationUser user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));  //C# 6.0 新語法

                ApplicationUser newUser = Add(user);

                IApplicationRoleRepository roleRepo = RepositoryHelper.GetApplicationRoleRepository(UnitOfWork);

                if (await IsInRoleAsync(user, "Users") == false)
                {
                    ApplicationRole role = roleRepo
                        .All()
                        .FirstOrDefault(p => p.Name.Equals("Users", StringComparison.InvariantCultureIgnoreCase));
                    newUser.ApplicationRole.Add(role);
                    //UnitOfWork.Context.Entry(role).State = EntityState.Modified;
                    UnitOfWork.Context.Entry(newUser).State = EntityState.Modified;
                    UnitOfWork.Commit();
                    UpdateCache(newUser);

                }
                user = newUser;
            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                throw ex;
            }
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            try
            {
                Delete(user);
                UnitOfWork.CommitAsync().Wait();
                UpdateCache(user);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                throw ex;
            }
        }

        public Task<ApplicationUser> FindByIdAsync(int userId)
        {
            try
            {
                ApplicationUser _user = Get(userId);

                return Task.FromResult(_user);
            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                throw ex;
            }
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            try
            {
                ApplicationUser _user = GetCache()
                    .FirstOrDefault(w => w.UserName == userName);

                if (_user == null)
                {
                    //Get From DB
                    _user = ObjectSet
                        .Include(p => p.ApplicationRole)
                        .Include(p => p.ApplicationUserClaim)
                        .Include(p => p.ApplicationUserLogin)
                        .Where(w => w.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase))
                        .FirstOrDefault();

                    //加入到記憶體快取
                    if (_user != null)
                        AddToCache(_user);

                }

                return Task.FromResult(_user);
            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                throw ex;
            }
        }



        public Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                ApplicationUser cacheUser = Get(user.Id);

                return Task.FromResult(
                   (IList<string>)cacheUser
                   .ApplicationRole.Select(s => s.Name).ToList()
                    );
            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                throw ex;
            }
        }

        public Task<bool> IsInRoleAsync(ApplicationUser user, string roleName)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                if (string.IsNullOrEmpty(roleName))
                {
                    throw new ArgumentNullException(nameof(roleName));
                }

                return Task.FromResult(
                    user.ApplicationRole.Any(s => s.Name.Equals(roleName,
                     StringComparison.InvariantCultureIgnoreCase))
                    );
            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                throw ex;
            }
        }

        public Task RemoveFromRoleAsync(ApplicationUser user, string roleName)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                if (string.IsNullOrEmpty(roleName))
                {
                    throw new ArgumentNullException(nameof(roleName));
                }

                ApplicationUser userfromcache = Get(user.Id);

                if (userfromcache == null)
                {
                    //load from db
                    userfromcache =
                        ObjectSet.FirstOrDefault(s => s.Id == user.Id);

                    if (userfromcache == null)
                        throw new NullReferenceException(string.Format("User '{0}' is not existed.", user.UserName));
                }

                Task<bool> isInRole = IsInRoleAsync(user, roleName);
                isInRole.Wait();

                if (isInRole.Result)
                {
                    user.ApplicationRole.Remove(userfromcache.
                        ApplicationRole.Single(w => w.Name.Equals(roleName,
                        StringComparison.InvariantCultureIgnoreCase)));

                    UnitOfWork.Context.Entry(user).State = EntityState.Modified;
                    UnitOfWork.Commit();
                    return Task.CompletedTask;
                }

                throw new Exception(string.Format("User '{0}' is not in roles of {1}.", user.UserName, roleName));

            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                throw ex;
            }
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                ApplicationUser dbUser = base.Get(user.Id);  //從資料庫讀取

                RemoveFromCache(user); //先從記憶體快取中移除

                dbUser.LastActivityTime = user.LastActivityTime;
                dbUser.LastLoginFailTime = user.LastLoginFailTime;
                dbUser.LastUnlockedTime = user.LastUnlockedTime;
                dbUser.LastUpdateTime = DateTime.UtcNow;
                dbUser.LastUpdateUserId = user.LastUpdateUserId;
                dbUser.LockoutEnabled = user.LockoutEnabled;
                dbUser.LockoutEndDate = user.LockoutEndDate;
                dbUser.Password = user.Password;
                dbUser.PasswordHash = user.PasswordHash;
                dbUser.PhoneConfirmed = user.PhoneConfirmed;
                dbUser.PhoneNumber = user.PhoneNumber;
                dbUser.ResetPasswordToken = user.ResetPasswordToken;
                dbUser.SecurityStamp = user.SecurityStamp;
                dbUser.TwoFactorEnabled = user.TwoFactorEnabled;
                dbUser.Void = user.Void;

                UnitOfWork.Context.Entry(dbUser).State = EntityState.Modified;
                Task commitTask = UnitOfWork.CommitAsync();
                commitTask.Wait();

                //重新加入記憶體快取
                AddToCache(dbUser);

                return commitTask;
            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                throw ex;
            }
        }
        #endregion

        #region User Role Store
        public Task AddToRoleAsync(ApplicationUser user, string roleName)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                if (string.IsNullOrEmpty(roleName))
                {
                    throw new ArgumentException(nameof(roleName));
                }

                IApplicationRoleRepository roleRepo = RepositoryHelper.GetApplicationRoleRepository();

                Task<ApplicationRole> roleTask = roleRepo.FindByNameAsync(roleName);
                roleTask.Wait();

                ApplicationRole role = roleTask.Result;
                user.ApplicationRole.Clear();
                user.ApplicationRole.Add(role);

                UpdateAsync(user).Wait();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                throw ex;
            }
        }
        #endregion

        #region Email Store

        public Task SetEmailAsync(ApplicationUser user, string email)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetEmailAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Helper Function
        private bool IsUserExists(string userName)
        {
            try
            {
                return GetCache()
                    .Any(p => p.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase));
            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                throw ex;
            }
        }




        #endregion
    }

    public partial interface IApplicationUserRepository : IRepositoryBase<ApplicationUser>, Interfaces.IDataRepository<ApplicationUser>
    {
        #region For User Store
        Task CreateAsync(ApplicationUser user);
        Task DeleteAsync(ApplicationUser user);
        Task<ApplicationUser> FindByIdAsync(int userId);
        Task<ApplicationUser> FindByNameAsync(string userName);
        Task<IList<string>> GetRolesAsync(ApplicationUser user);
        Task<bool> IsInRoleAsync(ApplicationUser user, string roleName);
        Task RemoveFromRoleAsync(ApplicationUser user, string roleName);
        Task UpdateAsync(ApplicationUser user);
        #endregion

        #region For User Role Store
        Task AddToRoleAsync(ApplicationUser user, string roleName);
        #endregion

        #region Email Store
        Task SetEmailAsync(ApplicationUser user, string email);
        Task<string> GetEmailAsync(ApplicationUser user);
        #endregion
    }
}