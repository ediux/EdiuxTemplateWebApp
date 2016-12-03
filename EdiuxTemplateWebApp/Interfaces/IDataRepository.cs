using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Interfaces
{
    public interface IDataRepository<T> where T:class
    {
        void ClearCache(string key);
        System.Collections.ObjectModel.ObservableCollection<T> GetCache();
    }
}
