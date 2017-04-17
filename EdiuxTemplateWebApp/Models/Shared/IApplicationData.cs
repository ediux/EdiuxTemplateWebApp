using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.Shared
{
    public interface IApplicationData<out TKey> 
    {
        TKey ApplicationId { get;  }

        string ApplicationName { get; set; }
    }
}
