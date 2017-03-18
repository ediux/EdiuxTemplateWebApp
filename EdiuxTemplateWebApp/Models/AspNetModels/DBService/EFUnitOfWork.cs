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
			SaveChanges();
		}

		public virtual Task CommitAsync()
		{
			return SaveChangesAsync();
		}

		public T GetTypedContext<T>() where T : IObjectContextAdapter
		{
			return (T)(this as IObjectContextAdapter);
		}
	}
}
