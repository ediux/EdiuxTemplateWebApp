using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EdiuxTemplateWebApp.Models
{
    /// <summary>
    /// 處理網頁功能的個人化設定API
    /// </summary>
    /// <typeparam name="TModel">儲存設定的物件模型</typeparam>
    /// <typeparam name="TKey">主索引鍵</typeparam>
    public interface IProfileStore<TModel, T, in TKey>
        where TModel : class
        where T : class
    {
        bool IsExisted(Expression<Func<T, bool>> filiter);

        Task<bool> IsExistedAsync(Expression<Func<T, bool>> filiter);

        void Add(TModel entity,TKey Key);

        Task AddAsync(TModel entity,TKey Key);

        TModel GetEntityByQuery(TKey key);

        bool Update(TModel entity,TKey Key);

        bool Delete(TModel entity,TKey Key);
      
        Task<TModel> GetEntityByQueryAsync(TKey key);

        Task<bool> UpdateAsync(TModel entity, TKey Key);

        Task<bool> DeleteAsync(TModel entity, TKey Key);

        Task InitializationProfileAsync(IController controller, TModel viewmodel);

        void InitializationProfile(IController controller, TModel viewmodel);
        
    }
}
