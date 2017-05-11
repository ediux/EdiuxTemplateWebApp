using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EdiuxTemplateWebApp.Models
{
    public interface IApplicationStore<TApp, in TKey> : IStoreBase<TApp, TKey> 
        where TApp : class, IApplicationData<TKey> 
    {
        TApp GetEntityByQuery(string ApplicationName);
        Task<TApp> GetEntityByQueryAsync(string ApplicationName);
        string GetApplicationNameFromConfiguratinFile();
        Task<string> GetApplicationNameFromConfiguratinFileAsync();
        IQueryable<TApp> Applications { get; }
        TApp GetApplicationFromConfiguratinFile();
        void Initialization(ActionExecutingContext filterContext);
        Task InitializationAsync(ActionExecutingContext filterContext);
    }
}
