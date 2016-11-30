using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity;
using Microsoft.AspNet.Identity;

namespace EdiuxTemplateWebApp.Models
{
    public partial class ApplicationUserRepository : EFRepository<ApplicationUser>, IApplicationUserRepository

    {
#if CS6
        private const string KeyName = nameof(ApplicationUser);
#else
        private static string KeyName = typeof(ApplicationUser).Name;
#endif

        public override IQueryable<ApplicationUser> All()
        {
            //檢查是否有快取，有就快取先行否則從資料庫載入
            try
            {
                return GetCache().Where(p => p.Void == false);
            }
            catch (Exception ex)
            {
#if !DEBUG
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
#endif
                throw ex;
            }

        }

        public override ApplicationUser Add(ApplicationUser entity)
        {
            try
            {

                List<ApplicationUser> _cache = GetFromCache();

                if (_cache != null)
                {
                    _cache.Add(entity);
                    UnitOfWork.Set(KeyName, _cache, 30);
                }

                return base.Add(entity);
            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif

                throw ex;
            }

        }

        public override void Delete(ApplicationUser entity)
        {
            try
            {
                List<ApplicationUser> _cache = GetFromCache();

                if (_cache != null)
                {
                    List<ApplicationUser> _cacheList = _cache;

                    _cache.Remove(entity);
                    UnitOfWork.Set(KeyName, _cache, 30);
                }


            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                throw ex;
            }

        }

        public void ClearCache(string key)
        {
            try
            {
                UnitOfWork.Invalidate(key);
            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                throw ex;
            }

        }

        public Task CreateAsync(ApplicationUser user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));  //C# 6.0 新語法

                if (IsUserExists(user.UserName))
                    throw new Exception(string.Format("User '{0}' is existed.", user.UserName));

                Add(user);
                UnitOfWork.Commit();

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

        public Task DeleteAsync(ApplicationUser user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));  //C# 6.0 新語法

                if (IsUserExists(user.UserName) == false)
                    throw new Exception(string.Format("User '{0}' is not existed.", user.UserName));

                var dbUser = Get(user.Id);

                dbUser.Void = true;
                dbUser.LockoutEnabled = true;
                dbUser.LockoutEndDate = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                dbUser.LastActivityTime = dbUser.LastUpdateTime = DateTime.UtcNow;

                if (System.Web.HttpContext.Current != null)
                {
                    dbUser.LastUpdateUserId = System.Web.HttpContext.Current.User.Identity.GetUserId<int>();
                }
                else
                {
                    ApplicationUser sysUser = FindByNameAsync("root").Result;

                    if (sysUser == null)
                    {
                        dbUser.LastUpdateUserId = 0;
                    }
                    else
                    {
                        dbUser.LastUpdateUserId = sysUser.Id;
                    }
                }

                UnitOfWork.Context.Entry(dbUser).State = EntityState.Modified;

                return UnitOfWork.CommitAsync();
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

        public IQueryable<ApplicationUser> GetCache()
        {
            try
            {
                if (UnitOfWork.IsSet(KeyName) == false)
                {
                    IQueryable<ApplicationUser> asyncResult = ObjectSet;
                    Task<List<ApplicationUser>> _cacheList = asyncResult.ToListAsync();
                    _cacheList.Wait();
                    UnitOfWork.Set(KeyName, _cacheList.Result, 30); //快取保留30分鐘
                    return asyncResult;
                }
                else
                {
                    List<ApplicationUser> _cache =
                        UnitOfWork.Get(KeyName) as List<ApplicationUser>;
                    return _cache.AsQueryable();
                }
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

                return Task.FromResult(
                   (IList<string>)user.ApplicationRole.SelectMany(s => s.Name).ToList()
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

                if (!string.IsNullOrEmpty(roleName))
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

                if (!string.IsNullOrEmpty(roleName))
                {
                    throw new ArgumentNullException(nameof(roleName));
                }

                ApplicationUser userfromcache =
                        All().FirstOrDefault(s => s.Id == user.Id);

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

                    ApplicationRole rolefromcache =
                        userfromcache.ApplicationRole.
                        FirstOrDefault(w => w.Name.Equals(roleName,
                        StringComparison.InvariantCultureIgnoreCase));

                    if (rolefromcache != null)
                    {
                        user.ApplicationRole.Remove(rolefromcache);
                        UnitOfWork.Context.Entry(user).State = EntityState.Modified;
                        UnitOfWork.Commit();
                        return Task.CompletedTask;
                    }

                    throw new Exception(string.Format("Role '{0}' is not existed.", roleName));
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

                List<ApplicationUser> _MemoryCache = GetFromCache();

                ApplicationUser dbUser = Get(user.Id);
                _MemoryCache.Remove(dbUser);

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

                _MemoryCache.Add(dbUser);
                UnitOfWork.Set(KeyName, _MemoryCache, 30);
                UnitOfWork.Context.Entry(dbUser).State = EntityState.Modified;
                return UnitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                throw ex;
            }
        }

        #region Helper Function
        private bool IsUserExists(string userName)
        {
            try
            {
                return GetCache().Any(p => p.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase));
            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                throw ex;
            }
        }
        private List<ApplicationUser> GetFromCache()
        {
            try
            {
                List<ApplicationUser> _cache = GetCache().ToList();
                return _cache;
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
        Task CreateAsync(ApplicationUser user);
        Task DeleteAsync(ApplicationUser user);
        Task<ApplicationUser> FindByIdAsync(int userId);
        Task<ApplicationUser> FindByNameAsync(string userName);
        Task<IList<string>> GetRolesAsync(ApplicationUser user);
        Task<bool> IsInRoleAsync(ApplicationUser user, string roleName);
        Task RemoveFromRoleAsync(ApplicationUser user, string roleName);
        Task UpdateAsync(ApplicationUser user);
    }
}