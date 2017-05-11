namespace EdiuxTemplateWebApp.Models
{
    public interface IUserPageSettingsStore<TEntity, in TKey> : IStoreBase<TEntity, TKey>
        where TEntity : class
    {

    }
}
