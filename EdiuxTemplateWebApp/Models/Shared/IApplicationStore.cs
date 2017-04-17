using EdiuxTemplateWebApp.Models.AspNetModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.Shared
{
    public interface IApplicationStore<TApp, in TKey> where TApp : class, IApplicationData<TKey>
    {
        Task createApplicationIfNotExisted();
        Task CreateAsync(TApp app);
        Task DeleteAsync(TApp app);
        Task<IEnumerable<TApp>> FindByIdAsync(TKey appId);
        Task<IEnumerable<TApp>> FindByNameAsync(string appName);
        Task<TApp> GetByIdAsync(TKey appId);
        Task<TApp> GetByNameAsync(string appName);
        Task UpdateAsync(TApp app);
        Task<string> GetApplicationNameFromConfiguratinFileAsync();
        IQueryable<TApp> Applications { get; }
    }
}
