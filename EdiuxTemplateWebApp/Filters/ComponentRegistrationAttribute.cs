using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using System.Web.Routing;
using EdiuxTemplateWebApp.Models.AspNetModels;
using System.Runtime.Caching;
using Microsoft.AspNet.Identity.Owin;
using EdiuxTemplateWebApp.Models;
using Microsoft.Owin;
using EdiuxTemplateWebApp.Models.Identity;
using System.Collections.Generic;

namespace EdiuxTemplateWebApp.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ComponentRegistrationAttribute : ActionFilterAttribute, IActionFilter
    {
        public string Title { get; set; }

        public string CSS { get; set; }

        public string Description { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                //1.取得OWIN的IOC環境
                //2.先取得Store存取服務層
                //3.取得Application資訊
                //4.從Application資訊取得系統已註冊的頁面
                //5.判斷目前所在URL是否已註冊
                //5.1 如已存在，直接返回
                //5.2 如不存在,新增資料到資料庫
                IOwinContext ioc = filterContext.HttpContext.GetOwinContext();
                IEdiuxAspNetSqlUserStore store = ioc.Get<IEdiuxAspNetSqlUserStore>();
                IPathStore<aspnet_Paths, Guid> PathStore = store;
           
                PathStore.Initialization(filterContext);
            }
            catch (Exception ex)
            {
                if (ex is DbEntityValidationException)
                {
                    filterContext.Controller.ViewBag.Exception = ex as DbEntityValidationException;
                }
                else
                {
                    filterContext.Controller.ViewBag.Exception = ex;
                }
                throw ex;
            }
        }
    }
}