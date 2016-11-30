using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models
{
	public partial class EFUnitOfWork : IUnitOfWork
	{
		private DbContext _context;
		public DbContext Context { get{ return _context;} set{ _context=value;} }

		public EFUnitOfWork()
		{
			Context = new AspNetDbEntities();
		}

		public void Commit()
		{
            try
            {
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
#if !TEST
                Elmah.ErrorSignal.Get(new MvcApplication()).Raise(ex);
#endif
                List<string> list = new List<string>();

                if(ex is System.Data.Entity.Validation.DbEntityValidationException)
                {
                    System.Data.Entity.Validation.DbEntityValidationException dbValidationEx =
                        (System.Data.Entity.Validation.DbEntityValidationException)ex;

                    foreach(var error in dbValidationEx.EntityValidationErrors)
                    {
                        foreach(var detailerror in error.ValidationErrors)
                        {
                            list.Add(string.Format("{0}:{1}", detailerror.PropertyName, detailerror.ErrorMessage));
                        }
                    }

                    throw new Exception(string.Join("\n", list.ToArray()));
                }
                throw ex;
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
