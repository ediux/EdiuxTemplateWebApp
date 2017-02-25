using System.Data.Entity;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels.DataEntity
{
	public partial class EFUnitOfWork : IUnitOfWork
	{
		private DbContext _context;
		public DbContext Context { get{ return _context;} set{ _context=value;} }

		public EFUnitOfWork()
		{
			Context = new AspNetDbEntities2();
		}

		public void Commit()
		{
			Context.SaveChanges();
		}
		
		public async Task CommitAsync()
        {
            await Context.SaveChangesAsync();
        }

		public bool LazyLoadingEnabled
		{
			get { return Context.Configuration.LazyLoadingEnabled; }
			set { Context.Configuration.LazyLoadingEnabled = value; }
		}

		public bool ProxyCreationEnabled
		{
			get { return Context.Configuration.ProxyCreationEnabled; }
			set { Context.Configuration.ProxyCreationEnabled = value; }
		}
		
		public string ConnectionString
		{
			get { return Context.Database.Connection.ConnectionString; }
			set { Context.Database.Connection.ConnectionString = value; }
		}

	}
}
