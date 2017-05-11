using EdiuxTemplateWebApp.Models;
using EdiuxTemplateWebApp.Models.AspNetModels;
using EdiuxTemplateWebApp.Models.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Mvc;

namespace EdiuxTemplateWebApp
{
    public static class UserStoreExtension
    {
        public static T Deserialize<T>(this byte[] stream) where T : class
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream buffer = new MemoryStream(stream, false);
            T value = (T)bf.Deserialize(buffer);
            return value;
        }

        public static byte[] Serialize<T>(this T obj) where T : class
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream buffer = new MemoryStream();
            bf.Serialize(buffer, obj);
            return buffer.ToArray();
        }

        #region Profile
        /// <summary>
        /// 取得反序列化後的個人化資訊
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="profileObject"></param>
        /// <returns></returns>
        public static UserProfileViewModel GetProfile(this aspnet_Users profileObject)
        {
            return UserProfileViewModel.Get(profileObject.Id);
        }

        public static void SetProfile(this aspnet_Users user, UserProfileViewModel prop)
        {
            UserProfileViewModel.Set(prop, user.Id);
        }
        #endregion

        #region Application Information
        public static aspnet_Applications getApplicationInfo(this Controller ctr)
        {
            if (ctr.ViewBag.ApplicationInfo == null)
            {
                aspnet_Applications appInfo = Helpers.WebHelper.GetApplicationGlobalVariable<aspnet_Applications>(ctr, EdiuxAspNetSqlUserStore.ApplicationInfoKey);

                if (appInfo == null)
                {
                    throw new Exception("Application information is not found.");
                }

                return appInfo;
            }

            return ctr.ViewBag.ApplicationInfo as aspnet_Applications;
        }

        public static aspnet_Applications GetApplicationInfo<T>(this T obj) where T : class
        {
            IApplicationStore<aspnet_Applications, Guid> store = HttpContext.Current.GetOwinContext().Get<IEdiuxAspNetSqlUserStore>();
            return store.GetEntityByQuery(store.GetApplicationNameFromConfiguratinFile());
        }

        public static aspnet_Applications addApplicationInfotoServer(this object obj)
        {
            try
            {
                Iaspnet_ApplicationsRepository appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();

                aspnet_Applications appInfo = new aspnet_Applications() { ApplicationName = EdiuxAspNetSqlUserStore.ApplicationInfoKey };
                appInfo.LoweredApplicationName = appInfo.ApplicationName.ToLowerInvariant();
                appInfo.Description = appInfo.ApplicationName;
                appInfo = appRepo.Add(appInfo);

                appRepo.UnitOfWork.Commit();
                return appInfo;
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                throw;
            }

        }
        #endregion
    }
}