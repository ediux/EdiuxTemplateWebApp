using PagedList;
using System.Collections.Generic;

namespace EdiuxTemplateWebApp.Models
{
    public class MyPagedList<T> : PagedList<T>
    {
        public MyPagedList(List<T> source, int PageNumber, int PageSize, int TotalCount) : base(source, PageNumber, PageSize)
        {
            TotalItemCount = TotalCount;
        }
    }
}