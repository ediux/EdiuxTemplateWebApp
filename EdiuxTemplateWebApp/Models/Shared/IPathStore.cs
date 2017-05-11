using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EdiuxTemplateWebApp.Models
{
    public interface IPathStore<TEntity, in TKey> : IStoreBase<TEntity, TKey>
        where TEntity : class
    {
        TEntity GetEntityByQuery(string URL);

        Task<TEntity> GetEntityByQueryAsync(string URL);

        void Initialization(ActionExecutingContext filterContext);

        Task InitializationAsync(ActionExecutingContext filterContext);
    }
}
