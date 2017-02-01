using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace EdiuxTemplateWebApp
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class SqlInputAttribute : Attribute
    {
        public string ParamterName { get; set; }

        public DbType DataType { get; set; }

        public SqlInputAttribute(string paramterName) : base()
        {
            ParamterName = paramterName;
        }
    }
}