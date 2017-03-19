using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
	public partial class EFUnitOfWork : AspNetDbEntities2, IUnitOfWork, IRepositoryCollection
	{

		public IDbConnection Connection
		{
			get
			{
				return Database.Connection;
			}
		}

		public string ConnectionString
		{
			get
			{
				return Database.Connection.ConnectionString;
			}

			set
			{
				Database.Connection.ConnectionString = value;
			}
		}

		public IObjectContextAdapter Context
		{
			get
			{
				return this;
			}

		}

		public bool LazyLoadingEnabled
		{
			get
			{
				return Configuration.LazyLoadingEnabled;
			}

			set
			{
				Configuration.LazyLoadingEnabled = value;
			}
		}

		public bool ProxyCreationEnabled
		{
			get
			{
				return Configuration.ProxyCreationEnabled;
			}

			set
			{
				Configuration.ProxyCreationEnabled = value;
			}
		}



		public IRepositoryCollection Repositories
		{
			get
			{
				return this;
			}

			set
			{
				var repositories = value;
			}
		}

		public void Commit()
		{
			if (!transcationMode)
				SaveChanges();
		}

		public virtual Task CommitAsync()
		{
			if (!transcationMode)
				return SaveChangesAsync();
			else
				return Task.CompletedTask;
		}

		public T GetTypedContext<T>() where T : IObjectContextAdapter
		{
			return (T)(this as IObjectContextAdapter);
		}

		bool transcationMode = false;

		/// <summary>
		/// 取得或設定目前是否處於交易模式。
		/// <see cref="T:EdiuxTemplateWebApp.Models.AspNetModels.aspnet_MembershipRepository"/> transcation mode.
		/// </summary>
		/// <value>值如果為 <c>true</c> 則處於交易模式，不會呼叫Commit()方法; 假如為 <c>false</c> 會直接呼叫 Commit()。</value>
		public bool TranscationMode
		{
			get
			{
				return transcationMode;
			}

			set
			{
				transcationMode = value;
			}
		}
	}
}
