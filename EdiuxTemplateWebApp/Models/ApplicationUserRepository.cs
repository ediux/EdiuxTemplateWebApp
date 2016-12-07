using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using System.Linq.Expressions;
using System.Security.Claims;

namespace EdiuxTemplateWebApp.Models
{
    public partial class ApplicationUserRepository : EFRepository<ApplicationUser>, IApplicationUserRepository

    {
#if CS6
        private const string KeyName = nameof(ApplicationUser);
#else
        private static string KeyName = typeof(ApplicationUser).Name;
#endif
        private bool _IsOnline;
        public bool IsOnline
        {
            get
            {
                return _IsOnline;
            }

            set
            {
                _IsOnline = value;
            }
        }

        public ApplicationUserRepository()
        {
            _IsOnline = false;
        }

        #region Override of Base

        protected ApplicationUser ChangeBeforeReactive(ApplicationUser existedUser)
        {
            try
            {


                if (existedUser != null)
                {
                    existedUser.Void = false;
                    existedUser.LockoutEnabled = false;

                    if (System.Web.HttpContext.Current != null)
                    {
                        existedUser.LastUpdateUserId
                            = System.Web.HttpContext.Current.User.Identity.GetUserId<int>();
                    }
                    else
                    {
                        Task<ApplicationUser> findbyNameTask
                            = FindByNameAsync("root");
                        findbyNameTask.Wait();

                        ApplicationUser updatedUser
                            = findbyNameTask.Result;

                        if (updatedUser != null)
                        {
                            existedUser.LastUpdateUserId = updatedUser.Id;
                        }
                        else
                        {
                            existedUser.LastUpdateUserId = 0;
                        }
                    }

                    existedUser.LastActivityTime
                        = existedUser.LastUnlockedTime
                        = existedUser.LockoutEndDate
                        = existedUser.LastUpdateTime
                        = DateTime.UtcNow;

                    existedUser.ApplicationRole.Clear();    //移除角色
                    existedUser.ApplicationUserClaim.Clear();   //移除宣告式身分識別
                    existedUser.ApplicationUserLogin.Clear();   //移除外部登入識別                        

                }

                return existedUser;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }

        }

        public override ApplicationUser Add(ApplicationUser entity)
        {
            ApplicationUser existedUser = null;
            ApplicationUser newUser = null;
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                try
                {
                    existedUser =
                        base.All()
                        .SingleOrDefault(w => w.UserName.Equals(entity.UserName, StringComparison.InvariantCultureIgnoreCase));
                }
                catch (InvalidOperationException)
                {
                    //找不到或是有一筆以上紀錄
                    //以建立日期找出最後建立那一次的紀錄
                    existedUser = base.All()
                        .OrderByDescending(o => o.CreateTime)
                        .First(w => w.UserName.Equals(entity.UserName, StringComparison.InvariantCultureIgnoreCase));

                    //找出重複的使用者並永久刪除
                    List<ApplicationUser> existedUsers =
                        base.All()
                        .Where(w => w.UserName.Equals(entity.UserName, StringComparison.InvariantCultureIgnoreCase)
                        && w.Id != existedUser.Id).ToList();

                    foreach (var user in existedUsers)
                    {
                        ObjectSet.Remove(user);
                    }

                    UnitOfWork.Commit();
                    existedUser = Reload(existedUser);
                }

                if (existedUser == null)
                {
                    newUser = ApplicationUser.Create();
                    newUser.CloneFrom(entity);
                    ObjectSet.Add(newUser);
                }
                else
                {
                    if (existedUser.Void)
                    {
                        //檢查停用後最後更新時間與當下時間距離多久，如果超過六個月將該帳號直接刪除。
                        TimeSpan accountDisableDurtion = (TimeSpan)(DateTime.Now - existedUser.LastUpdateTime);

                        if (accountDisableDurtion.Days > (30 * 6))
                        {
                            ObjectSet.Remove(existedUser);
                            newUser = ApplicationUser.Create();
                            newUser.CloneFrom(entity);
                            ObjectSet.Add(newUser);
                        }
                        else
                        {
                            newUser = existedUser;
                            newUser.CloneFrom(entity);
                            newUser.Void = false;
                            newUser.LockoutEnabled = false;
                            newUser.LockoutEndDate = DateTime.UtcNow;
                            newUser.ApplicationRole.Clear();
                            newUser.ApplicationUserClaim.Clear();
                            newUser.ApplicationUserLogin.Clear();
                            UnitOfWork.Context.Entry(newUser).State = EntityState.Modified;
                        }
                    }
                    else
                    {
                        throw new Exception(string.Format("User '{0}' is already existed.", entity.UserName));
                    }

                }

                return newUser;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }

        }

        public override void Delete(ApplicationUser entity)
        {
            ApplicationUser existedUser = null;

            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                Task<ApplicationUser> findUserByIdTask = FindByIdAsync(entity.Id);
                findUserByIdTask.Wait();

                existedUser = findUserByIdTask.Result;

                if (existedUser == null)
                {
                    throw new Exception(string.Format("User '{0}' is not existed.", entity.UserName));
                }
                else
                {
                    //作廢並鎖定帳號
                    existedUser.Void = true;
                    existedUser.LockoutEnabled = true;
                    existedUser.LockoutEndDate = null;
                    existedUser.LastUpdateUserId
                          = getCurrentLoginedUserId();
                    existedUser.ApplicationRole.Clear();
                    UnitOfWork.Context.Entry(existedUser).State = EntityState.Modified;
                }

            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }

        }

        public override IQueryable<ApplicationUser> All()
        {
            try
            {
                return base.All().Where(w => w.Void == false);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public override IList<ApplicationUser> BatchAdd(IEnumerable<ApplicationUser> entities)
        {
            try
            {
                IList<ApplicationUser> newUsers = new List<ApplicationUser>();

                foreach (var user in entities)
                {
                    newUsers.Add(Add(user));
                }
                return newUsers;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
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
                UnitOfWork.Commit();
                //newUser = Reload(newUser);
                //await AddToRoleAsync(newUser, "Users");
                user = await ReloadAsync(newUser);

            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public async Task DeleteAsync(ApplicationUser user)
        {
            try
            {
                Delete(user);
                await UnitOfWork.CommitAsync();
                UpdateCache(user);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public async Task<ApplicationUser> FindByIdAsync(int userId)
        {
            try
            {
                ApplicationUser _user = await GetAsync(userId);
                return _user;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            try
            {
                ApplicationUser _user = null;

                var queryresult = base.All();
                _user = queryresult.SingleOrDefault(w => w.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase));

                return Task.FromResult(_user);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
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

                dbUser.CloneFrom(user);
                dbUser.LastUpdateUserId = getCurrentLoginedUserId();
                dbUser.LastActivityTime
                    = dbUser.LastUpdateTime = DateTime.UtcNow;

                UnitOfWork.Context.Entry(dbUser).State = EntityState.Modified;
                Task commitTask = UnitOfWork.CommitAsync();
                commitTask.Wait();

                return commitTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
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

                IApplicationRoleRepository roleRepo = RepositoryHelper.GetApplicationRoleRepository(UnitOfWork);

                Task<ApplicationRole> roleTask = roleRepo.FindByNameAsync(roleName);
                roleTask.Wait();

                ApplicationRole role = roleTask.Result;
                user.ApplicationRole.Clear();
                user.ApplicationRole.Add(role);
                roleRepo.UnitOfWork.Context.Entry(role).State = EntityState.Modified;
                UpdateAsync(user).Wait();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                ApplicationUser cacheUser = await GetAsync(user.Id);

                return
                   (IList<string>)cacheUser
                   .ApplicationRole
                   .Select(s => s.Name)
                   .ToList();
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName)
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

                return await Task.FromResult(
                    user.ApplicationRole.Any(s => s.Name.Equals(roleName,
                     StringComparison.InvariantCultureIgnoreCase)));
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public async Task RemoveFromRoleAsync(ApplicationUser user, string roleName)
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

                ApplicationUser userfromcache = await GetAsync(user.Id);

                if (userfromcache == null)
                {
                    //load from db
                    userfromcache =
                        await ObjectSet.FirstOrDefaultAsync(s => s.Id == user.Id);

                    if (userfromcache == null)
                        throw new NullReferenceException(string.Format("User '{0}' is not existed.", user.UserName));
                }

                Task<bool> isInRole = IsInRoleAsync(user, roleName);
                isInRole.Wait();

                if (isInRole.Result)
                {
                    user.ApplicationRole.Remove(userfromcache.
                        ApplicationRole.SingleOrDefault(w => w.Name.Equals(roleName,
                        StringComparison.InvariantCultureIgnoreCase)));

                    UnitOfWork.Context.Entry(user).State = EntityState.Modified;
                    await UnitOfWork.CommitAsync();
                }

                throw new Exception(string.Format("User '{0}' is not in roles of {1}.", user.UserName, roleName));

            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }
        #endregion

        #region Email Store
        public async Task SetEmailAsync(ApplicationUser user, string email)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                if (string.IsNullOrEmpty(email))
                    throw new ArgumentNullException(nameof(email));

                ApplicationUser userinDB = await FindByIdAsync(user.Id);
                if (userinDB != null)
                {
                    userinDB.EMail = email;
                    userinDB.LastUpdateTime = DateTime.UtcNow;
                    userinDB.LastUpdateUserId = getCurrentLoginedUserId();

                    if (IsOnline)
                    {
                        userinDB.LastActivityTime = DateTime.UtcNow;
                    }

                    if (userinDB.TwoFactorEnabled)
                    {
                        userinDB.EMailConfirmed = false;
                    }
                    else
                    {
                        userinDB.EMailConfirmed = true;
                    }
                }
                await UpdateAsync(user);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }
        public Task<string> GetEmailAsync(ApplicationUser user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                return Task.FromResult(user.EMail);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
        }
        public async Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                ApplicationUser userinDB = await FindByIdAsync(user.Id);

                if (userinDB != null)
                {
                    userinDB.EMailConfirmed = confirmed;
                    userinDB.LastUpdateTime = DateTime.UtcNow;

                    if (_IsOnline)
                    {
                        userinDB.LastActivityTime = DateTime.UtcNow;
                    }

                    await UpdateAsync(userinDB);
                }

            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
        }
        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                return Task.FromResult(user.EMailConfirmed);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }
        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            try
            {
                ApplicationUser founduser =
                  await All().SingleOrDefaultAsync(w =>
                  w.EMail.Equals(email, StringComparison.InvariantCultureIgnoreCase));

                return founduser;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
        }
        #endregion

        #region User Lockout Store
        public Task<DateTimeOffset> GetLockoutEndDateAsync(ApplicationUser user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                DateTimeOffset LockoutEndDateFrom = new DateTimeOffset(user.LockoutEndDate ?? new DateTime(1754, 1, 1));
                return Task.FromResult(LockoutEndDateFrom);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset lockoutEnd)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                ApplicationUser userInDb = Get(user.Id);

                userInDb.LastUpdateTime = DateTime.UtcNow;
                userInDb.LastActivityTime = DateTime.UtcNow;
                userInDb.LastUpdateUserId = getCurrentLoginedUserId();
                userInDb.LockoutEndDate = new DateTime(lockoutEnd.Ticks).ToUniversalTime();

                UpdateAsync(userInDb).Wait();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                ApplicationUser userInDb = Get(user.Id);

                if (userInDb != null)
                {
                    userInDb.AccessFailedCount += 1;
                    userInDb.LastUpdateTime = DateTime.UtcNow;
                    userInDb.LastActivityTime = DateTime.UtcNow;
                    userInDb.LastUpdateUserId = getCurrentLoginedUserId();
                    UpdateAsync(userInDb).Wait();
                    userInDb = Reload(userInDb);
                    return Task.FromResult(userInDb.AccessFailedCount);
                }

                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task ResetAccessFailedCountAsync(ApplicationUser user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                ApplicationUser userInDb = Get(user.Id);

                if (userInDb != null)
                {
                    userInDb.AccessFailedCount = 0;
                    userInDb.LastUpdateTime = DateTime.UtcNow;
                    userInDb.LastActivityTime = DateTime.UtcNow;
                    userInDb.LastUpdateUserId = getCurrentLoginedUserId();
                    userInDb.LastUpdateUserId = getCurrentLoginedUserId();

                    UpdateAsync(userInDb).Wait();
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task<int> GetAccessFailedCountAsync(ApplicationUser user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                return Task.FromResult(user.AccessFailedCount);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task<bool> GetLockoutEnabledAsync(ApplicationUser user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));
                return Task.FromResult(user.LockoutEnabled ?? false);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                ApplicationUser userInDb = Get(user.Id);

                userInDb.AccessFailedCount = 0;
                userInDb.LastUpdateTime = DateTime.UtcNow;
                userInDb.LastActivityTime = DateTime.UtcNow;
                userInDb.LastUpdateUserId = getCurrentLoginedUserId();
                userInDb.LockoutEnabled = enabled;

                UpdateAsync(userInDb).Wait();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }
        #endregion

        #region User Password Store
        public async Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                if (string.IsNullOrEmpty(passwordHash))
                    throw new ArgumentNullException(nameof(passwordHash));

                ApplicationUser userinDB = await FindByIdAsync(user.Id);

                if (userinDB != null)
                {
                    userinDB.PasswordHash = passwordHash;
                    userinDB.LastUpdateUserId = getCurrentLoginedUserId();
                    userinDB.LastActivityTime = userinDB.LastUpdateTime = DateTime.Now.ToUniversalTime();

                    await UpdateAsync(userinDB);
                }

            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public async Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                ApplicationUser userinDB = await FindByIdAsync(user.Id);

                if (userinDB != null)
                {
                    return userinDB.PasswordHash;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public async Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                ApplicationUser userinDB = await FindByIdAsync(user.Id);

                if (!string.IsNullOrEmpty(userinDB.Password))
                    return true;
                if (!string.IsNullOrEmpty(userinDB.PasswordHash))
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }
        #endregion

        #region User Phone Number Store
        public async Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                if (string.IsNullOrEmpty(phoneNumber))
                    throw new ArgumentNullException(nameof(phoneNumber));

                ApplicationUser userinDB = await FindByIdAsync(user.Id);

                if (userinDB != null)
                {
                    userinDB.PhoneNumber = phoneNumber;
                    userinDB.PhoneConfirmed = true;

                    if (await GetTwoFactorEnabledAsync(user))
                        userinDB.PhoneConfirmed = false;

                    await UpdateAsync(userinDB);
                }

            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }

        }

        public Task<string> GetPhoneNumberAsync(ApplicationUser user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                return Task.FromResult(user.PhoneNumber);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                return Task.FromResult(user.PhoneConfirmed);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public async Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                ApplicationUser userinDB = await FindByIdAsync(user.Id);

                if (userinDB != null)
                {
                    userinDB.PhoneConfirmed = confirmed;
                    await UpdateAsync(userinDB);
                }

            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }

        }
        #endregion

        #region Security Stamp Store
        public Task<string> GetSecurityStampAsync(ApplicationUser user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                return Task.FromResult(user.SecurityStamp);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public async Task SetSecurityStampAsync(ApplicationUser user, string stamp)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                if (string.IsNullOrEmpty(stamp))
                    throw new ArgumentNullException(nameof(stamp));

                ApplicationUser userinDB = await FindByIdAsync(user.Id);

                if (userinDB != null)
                {
                    userinDB.SecurityStamp = stamp;
                    await UpdateAsync(userinDB);
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }

        }
        #endregion

        #region TwoFactor
        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                return Task.FromResult(user.TwoFactorEnabled);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }

        }

        public async Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                ApplicationUser userinDB = await FindByIdAsync(user.Id);

                if (userinDB != null)
                {
                    userinDB.TwoFactorEnabled = enabled;
                    await UpdateAsync(userinDB);
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }

        }
        #endregion

        #region User Claim Store
        public async Task AddClaimAsync(ApplicationUser user, Claim claim)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                if (claim == null)
                    throw new ArgumentNullException(nameof(claim));

                ApplicationUser userinDB = await FindByIdAsync(user.Id);

                if (userinDB != null)
                {
                    userinDB.ApplicationUserClaim.Add(new ApplicationUserClaim()
                    {
                        ClaimType = claim.ValueType,
                        ClaimValue = claim.Value,
                        UserId = user.Id
                    });
                    await UpdateAsync(userinDB);
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }

        }

        public Task<IList<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                return Task.FromResult(
                    user.ApplicationUserClaim
                    .ToList().
                    ConvertAll(c => new Claim(c.ClaimType, c.ClaimValue)) as IList<Claim>
                );
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }

        }

        public async Task RemoveClaimAsync(ApplicationUser user, Claim claim)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                if (claim == null)
                    throw new ArgumentNullException(nameof(claim));

                ApplicationUser userinDB = await FindByIdAsync(user.Id);

                if (userinDB != null)
                {
                    List<ApplicationUserClaim> removeList = userinDB.ApplicationUserClaim.ToList();
                    foreach (ApplicationUserClaim listitem in removeList)
                    {
                        userinDB.ApplicationUserClaim.Remove(listitem);
                    }
                    await UpdateAsync(userinDB);
                }

            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }
        #endregion


        #region Helper Function
        protected override int getCurrentLoginedUserId()
        {
            try
            {

                if (System.Web.HttpContext.Current != null)
                {
                    return base.getCurrentLoginedUserId();
                }
                else
                {
                    Task<ApplicationUser> findbyNameTask
                        = FindByNameAsync("root");
                    findbyNameTask.Wait();

                    ApplicationUser updatedUser
                        = findbyNameTask.Result;

                    if (updatedUser != null)
                        return updatedUser.Id;
                }

                return 0;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }

        }
        #endregion

    }

    public partial interface IApplicationUserRepository : IRepositoryBase<ApplicationUser>
    {
        #region For Setings
        bool IsOnline { get; set; }
        #endregion

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
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task<bool> GetEmailConfirmedAsync(ApplicationUser user);
        Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed);
        #endregion

        #region User Lockout Store
        Task<DateTimeOffset> GetLockoutEndDateAsync(ApplicationUser user);

        Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset lockoutEnd);

        Task<int> IncrementAccessFailedCountAsync(ApplicationUser user);

        Task ResetAccessFailedCountAsync(ApplicationUser user);

        Task<int> GetAccessFailedCountAsync(ApplicationUser user);

        Task<bool> GetLockoutEnabledAsync(ApplicationUser user);

        Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled);
        #endregion

        #region User Password Store
        Task SetPasswordHashAsync(ApplicationUser user, string passwordHash);

        Task<string> GetPasswordHashAsync(ApplicationUser user);

        Task<bool> HasPasswordAsync(ApplicationUser user);
        #endregion

        #region User Phone Number Store
        Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber);

        Task<string> GetPhoneNumberAsync(ApplicationUser user);

        Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user);

        Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed);
        #endregion

        #region Security Stamp Store
        Task<string> GetSecurityStampAsync(ApplicationUser user);

        Task SetSecurityStampAsync(ApplicationUser user, string stamp);
        #endregion

        #region TwoFactor
        Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user);

        Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled);
        #endregion

        #region User Claim Store
        Task AddClaimAsync(ApplicationUser user, Claim claim);
        Task<IList<Claim>> GetClaimsAsync(ApplicationUser user);
        Task RemoveClaimAsync(ApplicationUser user, Claim claim);
        #endregion
    }
}