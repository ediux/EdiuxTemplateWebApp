using System;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial interface IUnitOfWork
    {
        /// <summary>
        /// 統一操作介面的資料庫連接器(EF框架使用或是其他ORM框架使用)
        /// </summary>
        IObjectContextAdapter Context { get; set; }

        /// <summary>
        /// 所有對應資料表的儲存庫物件
        /// </summary>
        IRepositoryCollection Repositories { get; set; }

        /// <summary>
        /// 共用的資料庫連線
        /// </summary>
        /// <remarks>
        /// 此屬性是供底層為ADO.NET連接層使用
        /// </remarks>
        IDbConnection Connection { get; }

        /// <summary>
        /// 取得或設定啟用延遲載入
        /// </summary>
        bool LazyLoadingEnabled { get; set; }
        bool ProxyCreationEnabled { get; set; }
        string ConnectionString { get; set; }

        /// <summary>
        /// 提交資料庫變更要求的同步方法。
        /// </summary>
        void Commit();

        /// <summary>
        /// 提交資料庫變更要求的非同步方法。
        /// </summary>
        /// <returns>非同步執行結果。</returns>
        Task CommitAsync();

        DbEntityEntry Entry(object entity);

        DbEntityEntry<T> Entry<T>(T entity) where T : class;

        DbSet Set(Type entityType);

        DbSet<T> Set<T>() where T : class;

        T GetTypedContext<T>() where T : IObjectContextAdapter;
    }
    public partial interface IUnitOfWork<T> : IUnitOfWork where T : IObjectContextAdapter
    {
        new T Context { get; set; }
    }
}
