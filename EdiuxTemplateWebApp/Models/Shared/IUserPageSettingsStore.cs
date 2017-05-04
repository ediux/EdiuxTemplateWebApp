using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.Shared
{
    public interface IUserPageSettingsStore<TEntity, in TKey> : IStoreBase<TEntity, TKey>
        where TEntity : class
    {

    }
}
