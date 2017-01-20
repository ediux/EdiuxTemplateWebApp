using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.ApplicationLevelModels
{
	public partial class EFRepository<T> : IRepositoryBase<T> where T : class
	{
		protected AspNetDbEntities InternalDatabaseAlias { get {
            return (AspNetDbEntities)UnitOfWork.Context;
        } }

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
	}
}