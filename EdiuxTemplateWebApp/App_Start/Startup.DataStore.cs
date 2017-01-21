using EdiuxTemplateWebApp.Models.AspNetModels;
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

            if (checkCurrentAppIsRegistered() == false)
            {
                registerApplication();
            }
            else
            {
                addToMemoryCache();
            }

        }

        private void addToMemoryCache(Iaspnet_ApplicationsRepository appRepo = null)
        {
            try
            {
                if (appRepo == null)
                    appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();

              

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void registerApplication()
        {
            try
            {
                string applicationName = getApplicationNameFromConfiguationFile();

                Iaspnet_ApplicationsRepository appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();

                appRepo.Add(aspnet_Applications.Create(applicationName));
                appRepo.UnitOfWork.Commit();

                addToMemoryCache(appRepo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

        internal static string getApplicationNameFromConfiguationFile()
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