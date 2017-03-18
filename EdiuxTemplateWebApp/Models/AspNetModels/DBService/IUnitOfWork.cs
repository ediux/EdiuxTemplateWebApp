using System;
using System.Data;
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
		IObjectContextAdapter Context { get; }

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

		/// <summary>
		/// 取得或設定是否建立遠端Proxy參考物件。
		/// Gets or sets a value indicating whether this <see cref="T:EdiuxTemplateWebApp.Models.AspNetModels.IUnitOfWork"/>
		/// proxy creation enabled.
		/// </summary>
		/// <value><c>true</c> if proxy creation enabled; otherwise, <c>false</c>.</value>
		bool ProxyCreationEnabled { get; set; }

		/// <summary>
		/// Gets or sets the connection string.
		/// </summary>
		/// <value>The connection string.</value>
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

		/// <summary>
		/// Entry the specified entity.
		/// </summary>
		/// <returns>The entry.</returns>
		/// <param name="entity">Entity.</param>
		DbEntityEntry Entry(object entity);

		/// <summary>
		/// Entry the specified entity.
		/// </summary>
		/// <returns>The entry.</returns>
		/// <param name="entity">Entity.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		DbEntityEntry<T> Entry<T>(T entity) where T : class;

		/// <summary>
		/// Set the specified entityType.
		/// </summary>
		/// <returns>The set.</returns>
		/// <param name="entityType">Entity type.</param>
		DbSet Set(Type entityType);

		/// <summary>
		/// Set this instance.
		/// </summary>
		/// <returns>The set.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		DbSet<T> Set<T>() where T : class;

		/// <summary>
		/// Gets the typed context.
		/// </summary>
		/// <returns>The typed context.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		T GetTypedContext<T>() where T : IObjectContextAdapter;
	}

}
