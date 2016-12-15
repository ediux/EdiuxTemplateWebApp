using System.Data.Entity;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models
{
    public partial class EFUnitOfWork : IUnitOfWork
    {
        private DbContext _context;
        public DbContext Context { get { return _context; } set { _context = value; } }

        public EFUnitOfWork()
        {
            Context = new AspNetDbEntities();
        }

        public void Commit()
        {
            try
            {
                Context.SaveChanges();
                if (allKeys.Count > 0)
                {
                    var keys = allKeys.ToArray();
                    foreach(string key in keys)
                    {
                        Invalidate(key);
                    }
                }
            }
            catch (System.Exception)
            {
                throw;
            }

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
