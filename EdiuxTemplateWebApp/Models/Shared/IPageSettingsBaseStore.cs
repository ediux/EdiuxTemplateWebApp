using EdiuxTemplateWebApp.Models.AspNetModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EdiuxTemplateWebApp.Models
{
    /// <summary>
    /// 提供關於網頁路徑的API
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IPageSettingsBaseStore<TEntity, in TKey> : IStoreBase<TEntity,TKey>
        where TEntity : class
    {
        /// <summary>
        /// 依據目前控制器物件取得註冊的資訊
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        Task<TEntity> GetAsync(IController controller);

        ///// <summary>
        ///// 確認目前控制器的路徑物件註冊了沒?
        ///// </summary>
        ///// <param name="controller"></param>
        ///// <returns></returns>
        //Task<bool> CheckPathHasRegisteredAsync(IController controller);

        ///// <summary>
        ///// 註冊頁面的路徑路由
        ///// </summary>
        ///// <param name="controller"></param>
        ///// <returns></returns>
        //Task RegisterControllerAsync(IController controller);

        ///// <summary>
        ///// 解除註冊
        ///// </summary>
        ///// <param name="controller">要解除的物件</param>
        ///// <returns></returns>
        //Task UnRegisterControllerAsync(IController controller);

        ///// <summary>
        ///// 更新註冊資訊。
        ///// </summary>
        ///// <param name="controller"></param>
        ///// <param name="entity"></param>
        ///// <returns></returns>
        //Task ReRegisterControllerAsync(IController controller, TEntity entity);

        //Task<bool> IsHasBasePageSetting(IController controller);

        //Task<bool> IsHasUserPageSetting(IController controller);
    }
}
