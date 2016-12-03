using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity;

namespace EdiuxTemplateWebApp.Models
{
    public partial class ApplicationRoleRepository : EFRepository<ApplicationRole>, IApplicationRoleRepository
    {
#if CS6
        private const string KeyName = nameof(ApplicationRole);
#else
        private static string KeyName = typeof(ApplicationRole).Name;
#endif

        public override IQueryable<ApplicationRole> All()
        {
            return base.All().Where(w => w.Void == false);
        }

        #region Role Store
        public async Task CreateAsync(ApplicationRole role)
        {
            try
            {
                if (role == null)
                    throw new ArgumentNullException(nameof(role));  //C# 6.0 新語法

                Add(role);
                await UnitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public async Task DeleteAsync(ApplicationRole role)
        {
            try
            {
                if (role == null)
                    throw new ArgumentNullException(nameof(role));  //C# 6.0 新語法

                role.Void = true;
                role.LastUpdateTime = DateTime.UtcNow;
                role.LastUpdateUserId = getCurrentLoginedUserId();

                await UpdateAsync(role);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task<ApplicationRole> FindByIdAsync(int roleId)
        {
            try
            {
                return Task.FromResult(Get(roleId));
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public Task<ApplicationRole> FindByNameAsync(string roleName)
        {
            try
            {
                if (string.IsNullOrEmpty(roleName))
                    throw new ArgumentNullException(nameof(roleName));  //C# 6.0 新語法

                var role = (from q in All()
                            where q.Name.Equals(roleName, StringComparison.InvariantCultureIgnoreCase)
                            select q).SingleOrDefault();

                return Task.FromResult(role);

            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }

        public async Task UpdateAsync(ApplicationRole role)
        {
            try
            {
                if (role == null)
                    throw new ArgumentNullException(nameof(role));  //C# 6.0 新語法

                UnitOfWork.Context.Entry(role).State = EntityState.Modified;
                await UnitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw ex;
            }
        }
        #endregion   
    }

    public partial interface IApplicationRoleRepository : IRepositoryBase<ApplicationRole>
    {
        #region Role Store
        Task CreateAsync(ApplicationRole role);
        Task UpdateAsync(ApplicationRole role);
        Task DeleteAsync(ApplicationRole role);
        Task<ApplicationRole> FindByIdAsync(int roleId);
        Task<ApplicationRole> FindByNameAsync(string roleName);
        #endregion
    }
}