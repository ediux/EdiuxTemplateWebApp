using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.ApplicationLevelModels
{
	public partial class EFUnitOfWork : IUnitOfWork
	{	
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
