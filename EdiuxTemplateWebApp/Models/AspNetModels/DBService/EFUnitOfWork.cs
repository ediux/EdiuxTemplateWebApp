using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class EFUnitOfWork : IUnitOfWork
    {
        public EFUnitOfWork()
        {
          
            _context = new AspNetDbEntities2();
        }

        public void Commit()
        {
            _context.SaveChanges();
           
        }

        public async Task CommitAsync()
        {
           
            await _context.SaveChangesAsync();
        }

        public bool LazyLoadingEnabled
        {
            get { return _context.Configuration.LazyLoadingEnabled; }
            set { _context.Configuration.LazyLoadingEnabled = value; }
        }

        public bool ProxyCreationEnabled
        {
            get { return _context.Configuration.ProxyCreationEnabled; }
            set { _context.Configuration.ProxyCreationEnabled = value; }
        }

        public string ConnectionString
        {
            get { return _context.Database.Connection.ConnectionString; }
            set { _context.Database.Connection.ConnectionString = value; }
        }

        public IObjectContextAdapter Context
        {
            get
            {
                return _context;
            }

            set
            {
                _context = (AspNetDbEntities2)value;
            }
        }

        public IRepositoryCollection Repositories
        {
            get
            {
                return _context;
            }

            set
            {
                _context = (AspNetDbEntities2)value;
            }
        }

        public IDbConnection Connection
        {
            get
            {
                return _context.Database.Connection;
            }
        }

        private AspNetDbEntities2 _context;
     

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

        public DbEntityEntry<T> Entry<T>(T entity) where T : class
        {
            return _context.Entry<T>(entity);
        }

        public DbEntityEntry Entry(object entity)
        {
            return _context.Entry(entity);
        }

        public DbSet Set(Type entityType)
        {
            return _context.Set(entityType);
        }

        public DbSet<T> Set<T>() where T : class
        {
            return _context.Set<T>();
        }

        public T GetTypedContext<T>() where T : IObjectContextAdapter
        {
            return (T)Convert.ChangeType( _context,typeof(T)) ;
        }
    }
}
