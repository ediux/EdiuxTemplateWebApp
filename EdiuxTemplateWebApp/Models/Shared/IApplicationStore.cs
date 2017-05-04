using EdiuxTemplateWebApp.Models.AspNetModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models
{
    public interface IApplicationStore<TApp, in TKey> : IStoreBase<TApp, TKey> 
        where TApp : class, IApplicationData<TKey> 
    {
        //TODO:Remove after confirmed, 2017/4/30
        //Task<TApp> GetCurrentApplicationInfoAsync();
        //Task CreateApplicationIfNotExisted();
        //Task CreateAsync(TApp app);
        //Task DeleteAsync(TApp app);
        //Task<IEnumerable<TApp>> FindByIdAsync(TKey appId);
        //Task<IEnumerable<TApp>> FindByNameAsync(string appName);
        //Task<TApp> GetByIdAsync(TKey appId);

        //Task UpdateAsync(TApp app);
        TApp GetEntityByQuery(string ApplicationName);
        Task<TApp> GetEntityByQueryAsync(string ApplicationName);
        string GetApplicationNameFromConfiguratinFile();
        Task<string> GetApplicationNameFromConfiguratinFileAsync();
        IQueryable<TApp> Applications { get; }
    }
}
