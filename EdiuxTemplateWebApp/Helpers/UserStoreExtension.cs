using EdiuxTemplateWebApp.Models.AspNetModels;
using Microsoft.AspNet.Identity;
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
        public static T GetProfile<T>(this aspnet_Users profileObject) where T : class
        {
            Iaspnet_ProfileRepository profileRepo = RepositoryHelper.Getaspnet_ProfileRepository();

            aspnet_Profile _profileData = profileRepo.Get(profileObject.Id);

            if (_profileData != null)
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream buffer = new MemoryStream(_profileData.PropertyValuesBinary, true);
                T value = (T)bf.Deserialize(buffer);
                return value;
            }

            return default(T);
        }

        public static T SetProfile<T>(this aspnet_Users user, Action<T> prop) where T : class
        {
            Iaspnet_ProfileRepository profileRepo = RepositoryHelper.Getaspnet_ProfileRepository();

            aspnet_Profile _profileData = profileRepo.Get(user.Id);

            if (_profileData != null)
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream buffer = new MemoryStream(_profileData.PropertyValuesBinary, true);
                T value = (T)bf.Deserialize(buffer);
                Dictionary<string, PropertyInfo> props = value.GetProperties<T>();
                _profileData.PropertyNames = string.Join(";", props.Select(s => s.Key).ToArray());
                prop.Invoke(value);
                bf.Serialize(buffer, value);
                _profileData.PropertyValuesBinary = buffer.ToArray();
                buffer.Close();

                profileRepo.UnitOfWork.Entry(_profileData).State = System.Data.Entity.EntityState.Modified;
                profileRepo.UnitOfWork.Commit();

                _profileData = profileRepo.Reload(_profileData);
                buffer = new MemoryStream(_profileData.PropertyValuesBinary, true);
                value = (T)bf.Deserialize(buffer);
                return value;
            }
            else
            {
                _profileData = new aspnet_Profile();
                _profileData.UserId = user.Id;
                T value = Activator.CreateInstance<T>();

                Dictionary<string, PropertyInfo> props = value.GetProperties<T>();
                _profileData.PropertyNames = string.Join(";", props.Select(s => s.Key).ToArray());
                _profileData.PropertyValuesString = "";

                prop.Invoke(value);

                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream buffer = new MemoryStream(_profileData.PropertyValuesBinary, true);

                bf.Serialize(buffer, value);

                _profileData.PropertyValuesBinary = buffer.ToArray();
                buffer.Close();

                profileRepo.Add(_profileData);
                profileRepo.UnitOfWork.Commit();

                _profileData = profileRepo.Reload(_profileData);
                buffer = new MemoryStream(_profileData.PropertyValuesBinary, true);
                value = (T)bf.Deserialize(buffer);
                return value;
            }


        }



        #endregion

        #region Application Information
        public static aspnet_Applications getApplicationInfo(this Controller ctr)
        {
            if (ctr.ViewBag.ApplicationInfo == null)
            {
                aspnet_Applications appInfo = Helpers.WebHelper.getApplicationGlobalVariable<aspnet_Applications>(ctr, Startup.ApplicationInfoKey);

                if (appInfo == null)
                {
                    throw new Exception("Application information is not found.");
                }

                return appInfo;
            }

            return ctr.ViewBag.ApplicationInfo as aspnet_Applications;
        }

        public static aspnet_Applications getApplicationInfo(this object obj)
        {
            return Helpers.WebHelper.getApplicationGlobalVariable<aspnet_Applications>(obj, Startup.ApplicationInfoKey);
        }

        public static aspnet_Applications addApplicationInfotoServer(this object obj)
        {
            try
            {
                Iaspnet_ApplicationsRepository appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();

                aspnet_Applications appInfo = new aspnet_Applications() { ApplicationName = Startup.ApplicationInfoKey };
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