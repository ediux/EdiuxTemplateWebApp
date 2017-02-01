using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace EdiuxTemplateWebApp
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class SqlOutputAttribute : Attribute
    {
        public string ParamterName { get; set; }

        public DbType OutputType { get; set; }

        public SqlOutputAttribute(string paramterName) : base()
        {
            ParamterName = paramterName;
        }
    }
}