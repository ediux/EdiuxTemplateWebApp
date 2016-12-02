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
        public override IQueryable<ApplicationUser> Where(Expression<Func<ApplicationUser, bool>> expression)
        {
            try
            {
                //記憶體快取先行的讀取
                IQueryable<ApplicationUser> cacheResult = GetCache().Where(expression);
                //IQueryable<ApplicationUser> mergedSet = cacheResult.Union(base.Where(expression));  //
                //IQueryable<ApplicationUser> addtoCacheResult = mergedSet.Except(GetCache());
                //List<ApplicationUser> newcacheResult = GetFromCache();
                //newcacheResult.AddRange(addtoCacheResult.ToList());
                //return mergedSet;
                return cacheResult;
            }
            catch (Exception ex)
            {
#if !DEBUG
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
#endif
                throw ex;
            }

        }

        public override IList<ApplicationUser> BatchAdd(IEnumerable<ApplicationUser> entities)
        {
            List<ApplicationUser> cacheUsers = FromCache;

            var addlist = entities.Except(cacheUsers);

            if (addlist != null && addlist.Count() > 0)
            {
                cacheUsers.AddRange(addlist);
                UnitOfWork.Set(KeyName, cacheUsers, 30);
            }

            return base.BatchAdd(entities);
        }

        public override ApplicationUser Get(params object[] values)
        {
            try
            {
                if (values == null)
                    throw new ArgumentNullException(nameof(values));

                ApplicationUser cacheUser = null;

                if (values.Length == 1)
                {
                    cacheUser = GetCache().FirstOrDefault(p => p.Id == (int)values[0]);
                }

                if (cacheUser == null)
                {
                    cacheUser = base.Get(values);   //從資料庫讀取

                    //加入快取
                    AddToCache(cacheUser);
                }

                return cacheUser;
            }
            catch (Exception ex)
            {
#if !DEBUG
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
#endif
                throw ex;
            }
        }

        public override IQueryable<ApplicationUser> All()
        {

            try
            {
                //取得記憶體快取中的資料
                //只傳回未被標記為刪除的資料集合
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
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));  //C# 6.0 新語法

                //先取得目前快取
                Task<ApplicationUser> userInMemoryOrDbTask = FindByNameAsync(entity.UserName);
                userInMemoryOrDbTask.Wait();

                if (userInMemoryOrDbTask.Result == null)
                {
                    //加入快取
                    AddToCache(entity);

                    //加入資料庫
                    return base.Add(entity);
                }
                else
                {

                    ApplicationUser existedUser = ObjectSet.Find(userInMemoryOrDbTask.Result.Id);  //取得已存在的使用者

                    if (existedUser == null)
                    {
                        existedUser = entity;
                        existedUser = base.Add(entity);
                        UpdateCache(existedUser.Id, existedUser);
                        return existedUser;
                    }

                    existedUser.Void = false;

                    Task<ApplicationUser> getRootUserTask = FindByNameAsync("root");
                    getRootUserTask.Wait();

                    if (getRootUserTask.Result != null)
                    {
                        existedUser.LastUpdateUserId = getRootUserTask.Result.Id;
                    }
                    else
                    {
                        existedUser.LastUpdateUserId = 0;
                    }

                    existedUser.LastUpdateTime = DateTime.UtcNow;

                    //更新資料庫
                    Task updateTask = UpdateAsync(existedUser);
                    updateTask.Wait();

                    //從資料庫重新載入
                    existedUser = Reload(existedUser);

                    //更新快取
                    UpdateCache(existedUser.Id, existedUser);

                    return existedUser;
                }
            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#else
                System.Diagnostics.Debug.WriteLine(ex.Message);
#endif

                throw ex;
            }

        }

        public override void Delete(ApplicationUser entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));  //C# 6.0 新語法

                if (IsUserExists(entity.UserName) == false)
                    throw new Exception(string.Format("User '{0}' is not existed.", entity.UserName));

                var dbUser = ObjectSet.Find(entity.Id);

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

        #region User Store
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
                    UpdateCache(user.Id, newUser);

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
                UpdateCache(user.Id, user);
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

        public IQueryable<ApplicationUser> GetCache()
        {
            try
            {
                if (UnitOfWork.IsSet(KeyName) == false)
                {
                    IQueryable<ApplicationUser> asyncResult = ObjectSet
                        .Include(p => p.ApplicationRole)
                        .Include(p => p.ApplicationUserClaim)
                        .Include(p => p.ApplicationUserLogin);

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

        private void RemoveFromCache(ApplicationUser entity)
        {
            try
            {
                if (FromCache.Any(p => p.Id == entity.Id))
                {
                    List<ApplicationUser> cacheSet = FromCache;
                    cacheSet.Remove(FromCache.Find(p => p.Id == entity.Id));
                    UnitOfWork.Set(KeyName, cacheSet, 30);
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

        private void AddToCache(ApplicationUser entity)
        {
            try
            {
                //加入到記憶體快取
                List<ApplicationUser> memoryUsersCache = FromCache;
                if (memoryUsersCache.Any(w => w.Id == entity.Id))   //確認快取有該筆資料
                {
                    RemoveFromCache(entity);
                }
                memoryUsersCache.Add(entity);
                UnitOfWork.Set(KeyName, memoryUsersCache, 30);
            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                throw ex;
            }
        }

        private void UpdateCache(int id, ApplicationUser updated)
        {
            try
            {
                //加入到記憶體快取
                List<ApplicationUser> memoryUsersCache = FromCache;
                if (memoryUsersCache.Any(w => w.Id == id))   //確認快取有該筆資料
                {
                    RemoveFromCache(memoryUsersCache.Find(p => p.Id == id));    //移除快取資料
                }
                memoryUsersCache.Add(updated);
                UnitOfWork.Set(KeyName, memoryUsersCache, 30);
            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                throw ex;
            }
        }


        private List<ApplicationUser> FromCache
        {
            get
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