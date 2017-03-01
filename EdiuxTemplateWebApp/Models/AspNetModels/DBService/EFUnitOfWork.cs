using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class EFUnitOfWork : IUnitOfWork
    {
        private DbContext _context;
        public DbContext Context { get { return _context; } set { _context = value; } }

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

        protected virtual void WriteErrorLog(Exception ex)
        {
            if (System.Web.HttpContext.Current == null)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
            else
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }

        public T GetDbContext<T>() where T : DbContext
        {
            return ((T)Context);
        }
    }
}
