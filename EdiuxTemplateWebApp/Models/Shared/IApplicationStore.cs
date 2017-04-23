using EdiuxTemplateWebApp.Models.AspNetModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models
{
    public interface IApplicationStore<TApp, in TKey> where TApp : class, IApplicationData<TKey>
    {
        Task<aspnet_Applications> GetCurrentApplicationInfoAsync();
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
