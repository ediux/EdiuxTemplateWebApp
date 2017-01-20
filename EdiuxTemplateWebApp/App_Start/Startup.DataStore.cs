﻿using EdiuxTemplateWebApp.Models.AspNetModels;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace EdiuxTemplateWebApp
{
    partial class Startup
    {
        public void ConfigureDataStore(IAppBuilder app)
        {
            //檢查Application是否已經註冊?

            if (checkCurrentAppIsRegistered()==false)
            {
                registerApplication(getApplicationNameFromConfiguationFile());
            }else
            {
                addToMemoryCache();
            }
            
        }

        private void addToMemoryCache()
        {
            try
            {
                Iaspnet_ApplicationsRepository appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();

                string appName = getApplicationNameFromConfiguationFile();

                MemoryCache.Default.Add("ApplicationInfo", appRepo.FindByName(appName), DateTime.UtcNow.AddMinutes(38400));

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void registerApplication(string v)
        {
            throw new NotImplementedException();
        }

        private bool checkCurrentAppIsRegistered()
        {
            try
            {
                Iaspnet_ApplicationsRepository appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();

                string appName = getApplicationNameFromConfiguationFile();

                if (appRepo.FindByName(appName) == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
         
        }

        private static string getApplicationNameFromConfiguationFile()
        {
            string appName;
            if (System.Web.Configuration.WebConfigurationManager.AppSettings.AllKeys.Contains("ApplicationName"))
            {
                appName = System.Web.Configuration.WebConfigurationManager.AppSettings["ApplicationName"];
            }
            else
            {
                appName = typeof(MvcApplication).Namespace;
            }

            return appName;
        }
    }
}