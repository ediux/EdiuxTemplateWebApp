using EdiuxTemplateWebApp.Models.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EdiuxTemplateWebApp.Models
{
    /// <summary>
    /// 處理網頁功能的個人化設定API
    /// </summary>
    /// <typeparam name="TModel">儲存設定的物件模型</typeparam>
    /// <typeparam name="TKey">主索引鍵</typeparam>
    public interface IProfileStore<TModel, in TKey> : IStoreBase<TModel, TKey>
        where TModel : class
    {
        //Task<TModel> GetAsync(IController controller);

        //Task<bool> IsHasProfileAsync(IController controller);

        Task InitializationProfileAsync(IController controller, TModel viewmodel);

        //Task<TModel> UpdateAsync(TModel model);

        //Task RemoveAsync(IController controller, TModel model);
    }
}
