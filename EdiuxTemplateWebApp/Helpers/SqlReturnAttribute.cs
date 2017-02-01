using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace EdiuxTemplateWebApp
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class SqlReturnAttribute : Attribute
    {
        public string ParamterName { get; set; }

        public DbType ReturnType { get; set; }

        public SqlReturnAttribute(string paramterName, DbType returnType) : base()
        {
            ParamterName = paramterName;
            ReturnType = returnType;
        }
    }
}