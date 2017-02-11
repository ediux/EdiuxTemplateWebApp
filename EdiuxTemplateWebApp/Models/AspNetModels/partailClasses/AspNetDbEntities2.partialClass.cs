using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class AspNetDbEntities2
    {

        public virtual ObjectResult<aspnet_Membership> aspnet_Membership_GetAllUsers2(string applicationName, Nullable<int> pageIndex, Nullable<int> pageSize, ObjectParameter totalRecords)
        {
            var applicationNameParameter = applicationName != null ?
                new ObjectParameter("ApplicationName", applicationName) :
                new ObjectParameter("ApplicationName", typeof(string));

            var pageIndexParameter = pageIndex.HasValue ?
                new ObjectParameter("PageIndex", pageIndex) :
                new ObjectParameter("PageIndex", typeof(int));

            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("PageSize", pageSize) :
                new ObjectParameter("PageSize", typeof(int));

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<aspnet_Membership>("aspnet_Membership_GetAllUsers", applicationNameParameter, pageIndexParameter, pageSizeParameter, totalRecords);
        }

    }
}