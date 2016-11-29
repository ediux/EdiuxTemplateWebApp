using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models
{
    public partial interface IUnitOfWork : EdiuxTemplateWebApp.Interfaces.ICacheProvider, IDisposable
    {
    }
}
