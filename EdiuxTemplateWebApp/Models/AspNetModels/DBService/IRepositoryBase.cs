using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial interface IRepositoryCollection : ICollection<IRepositoryBase>, IDisposable
    {

        T GetRepository<T>() where T : IRepositoryBase;
    }

    public partial interface IDepencyRepositoryBase : IDisposable
    {
        /// <summary>
        /// 取得相依的儲存庫
        /// </summary>
        IDictionary<string,IDepencyRepositoryBase> DependcyRepository { get; }

        void RegisterDependcyRepository(IDepencyRepositoryBase repository);

        void UnRegisterDepencyRepository(Type repositoryType);
    }

    public partial interface IRepositoryBase : IDisposable
    {

    }

    /// <summary>
    /// <paramref name="T"/> 的儲存庫操作介面。
    /// </summary>
    /// <typeparam name="T">資料實體物件。</typeparam>	
    public partial interface IRepositoryBase<T> : IRepositoryBase
        where T : class
    {
        /// <summary>
        /// 取得或設定統一操作物件的參考。
        /// </summary>
        IUnitOfWork UnitOfWork { get; set; }

        /// <summary>
        /// 取得Entity全部筆數的IQueryable。
        /// </summary>
        /// <returns>Entity全部筆數的IQueryable。</returns>
        IQueryable<T> All();

        /// <summary>
        /// 取得Entity全部筆數的IQueryable<<typeparamref name="T>"></typeparamref>>的非同步版本。
        /// </summary>
        Task<IQueryable<T>> AllAsync();

        /// <summary>
        /// 傳回<paramref name="T"/> 的篩選結果。
        /// </summary>
        /// <param name="expression">代表Where查詢子句的Lamba方法</param>
        IQueryable<T> Where(Expression<Func<T, bool>> expression);

        /// <summary>
        /// 插入一列新的資料列。
        /// </summary>
        /// <param name="entity">資料列實體。</param>
        /// <returns></returns>
        T Add(T entity);

        /// <summary>
        /// 建立多列資料的批次新增。
        /// </summary>
        /// <param name="entities">資料列實體。</param>
        IList<T> BatchAdd(IEnumerable<T> entities);

        /// <summary>
        /// 從資料庫刪除實體。
        /// </summary>
        /// <param name="entity">資料列實體。</param>
        void Delete(T entity);

        /// <summary>
        /// 依據傳入的主索引鍵值尋找符合的資料列並回傳。
        /// </summary>
        /// <param name="predicate">主索引鍵值(組)</param>
        /// <returns>傳回符合鍵值的資料列。</returns>
        T Get(params object[] values);

        /// <summary>
        /// 以非同步方法依據傳入的主索引鍵值尋找符合的資料列並回傳。
        /// </summary>
        /// <param name="values">主索引鍵值(組)</param>
        /// <returns>傳回符合鍵值的資料列。</returns>
        Task<T> GetAsync(params object[] values);

        /// <summary>
        /// 重新從資料庫載入實體。
        /// </summary>
        /// <param name="entity">資料列實體。</param>
        T Reload(T entity);

        /// <summary>
        /// 重新從資料庫載入實體的非同步方法。
        /// </summary>
        /// <param name="entity">資料列實體。</param>
        Task<T> ReloadAsync(T entity);

        /// <summary>
        /// 將實體加入變更追蹤。尤其針對預存程序轉換來的實體。
        /// </summary>
        /// <param name="entity"></param>
        void Attach(T entity);

        T ConvertFrom<TResult>(TResult entity);

        R CopyTo<R>(T entity);
    }
}
