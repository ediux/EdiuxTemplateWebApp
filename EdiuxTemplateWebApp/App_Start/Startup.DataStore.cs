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
        public const string ApplicationInfoKey = "ApplicationInfo";
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

            if (checkCurrentAppHasRoles() == false)
            {
                createDefaultRoles();
            }

            if (checkCurrentAppHasRootUser() == false)
            {
                createRootUser();
            }
            if (checkRootUserHasAdminsRole() == false)
            {
                addRootUserToAdminsRole();
            }

        }

        private bool checkRootUserHasAdminsRole(Iaspnet_ApplicationsRepository appRepo = null)
        {
            if (appRepo == null)
                appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();
            string appName = getApplicationNameFromConfiguationFile();
            aspnet_Applications appInfo = appRepo.FindByName(appName).SingleOrDefault();

            Iaspnet_UsersRepository usersRepo = RepositoryHelper.Getaspnet_UsersRepository(appRepo.UnitOfWork);

            aspnet_Users rootUser = usersRepo.GetUserByName(appInfo.ApplicationName, "root");

            if (rootUser != null)
            {
                return rootUser.aspnet_Roles.Any(s => s.Name.Equals("Admins", StringComparison.InvariantCultureIgnoreCase));
            }
            throw new NullReferenceException(string.Format("The object of '{0}' is not found.", nameof(rootUser)));
        }

        private void addRootUserToAdminsRole(Iaspnet_ApplicationsRepository appRepo = null)
        {
            if (appRepo == null)
                appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();
            string appName = getApplicationNameFromConfiguationFile();
            aspnet_Applications appInfo = getApplicationInformationFromCache(appName, appRepo);

            if (appInfo.aspnet_Roles.Any(s => s.Name.Equals("Admins", StringComparison.InvariantCultureIgnoreCase)) == false)
                throw new NullReferenceException(string.Format("The role of name, '{0}', is not found.", "Admins"));

            aspnet_Users rootUser = appInfo.GetUserByName("root",appRepo);

            if (rootUser != null)
            {
                if (rootUser.IsInRole("Admins") == false)
                    rootUser.AddToRole("Admins");

                return;
            }
            throw new NullReferenceException(string.Format("The username , '{0}', is not found.", "root"));
        }

        private bool checkCurrentAppHasRootUser(Iaspnet_ApplicationsRepository appRepo = null)
        {
            if (appRepo == null)
                appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();
            string appName = getApplicationNameFromConfiguationFile();
            aspnet_Applications appInfo = getApplicationInformationFromCache(appName, appRepo);

            if (appInfo != null)
            {
                return appInfo.aspnet_Users.Any(s => s.UserName.Equals("root", StringComparison.InvariantCultureIgnoreCase));
            }

            return false;
        }

        private void createRootUser(Iaspnet_ApplicationsRepository appRepo = null)
        {
            if (appRepo == null)
            {
                appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();

            }

            string appName = getApplicationNameFromConfiguationFile();
            aspnet_Applications appInfo = getApplicationInformationFromCache(appName, appRepo);

            aspnet_Users rootUser = appInfo.AddUser("root", "!QAZ2wsx");
            rootUser.AddToRole("Admins");

        }
        private bool checkCurrentAppHasRoles(Iaspnet_ApplicationsRepository appRepo = null)
        {
            if (appRepo == null)
                appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();
            string appName = getApplicationNameFromConfiguationFile();
            aspnet_Applications appInfo = getApplicationInformationFromCache(appName, appRepo);

            if (appInfo != null)
            {
                if (appInfo.aspnet_Roles.Count == 0)
                {
                    return false;
                }

                return appInfo.aspnet_Roles.Any(s =>
                s.Name.Equals("Admins", StringComparison.InvariantCultureIgnoreCase) ||
                s.Name.Equals("CoAdmins", StringComparison.InvariantCultureIgnoreCase) ||
                s.Name.Equals("Users", StringComparison.InvariantCultureIgnoreCase));
            }

            return false;
        }

        private void createDefaultRoles()
        {
            Iaspnet_ApplicationsRepository appRepo = null;
            appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();

            string appName = getApplicationNameFromConfiguationFile();
            aspnet_Applications appInfo = getApplicationInformationFromCache(appName, appRepo);

            if (appInfo.GetRoleByName("Admins") == null)
            {
                appInfo.aspnet_Roles.AddRole(new aspnet_Roles
                {
                    ApplicationId = appInfo.ApplicationId,
                    Description = "系統管理員",
                    LoweredRoleName = "admins",
                    Name = "Admins",
                    Id = Guid.NewGuid()
                });
                setToMemoryCache(appRepo);
            }
            if (appInfo.GetRoleByName("CoAdmins") == null)
            {
                appInfo.aspnet_Roles.AddRole(new aspnet_Roles
                {
                    ApplicationId = appInfo.ApplicationId,
                    Description = "次要管理員",
                    LoweredRoleName = "coadmins",
                    Name = "CoAdmins",
                    Id = Guid.NewGuid()
                });
                setToMemoryCache(appRepo);
            }
            if (appInfo.GetRoleByName("Users") == null)
            {
                appInfo.aspnet_Roles.AddRole(new aspnet_Roles
                {
                    ApplicationId = appInfo.ApplicationId,
                    Description = "使用者",
                    LoweredRoleName = "users",
                    Name = "Users",
                    Id = Guid.NewGuid()
                });
                setToMemoryCache(appRepo);
            }
        }

        private static aspnet_Applications getApplicationInformationFromCache(string appName, Iaspnet_ApplicationsRepository appRepo = null)
        {
            if (appRepo == null)
                appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();

            return MemoryCache.Default.AddOrGetExisting(
                ApplicationInfoKey,
                appRepo.FindByName(appName),
                new DateTimeOffset(DateTime.Now.AddMinutes(38400))
                ) as aspnet_Applications;
        }

        private void addToMemoryCache(Iaspnet_ApplicationsRepository appRepo = null)
        {
            try
            {
                if (appRepo == null)
                    appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();
                string appName = getApplicationNameFromConfiguationFile();
                aspnet_Applications app = appRepo.FindByName(appName).Clone() as aspnet_Applications;
                MemoryCache.Default.Add(ApplicationInfoKey, app, new DateTimeOffset(DateTime.Now.AddMinutes(38400)));
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void setToMemoryCache(Iaspnet_ApplicationsRepository appRepo = null)
        {
            try
            {
                if (appRepo == null)
                    appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();

                string appName = getApplicationNameFromConfiguationFile();
                aspnet_Applications app = appRepo.FindByName(appName).Clone() as aspnet_Applications;

                MemoryCache.Default.Set(ApplicationInfoKey, app, new DateTimeOffset(DateTime.Now.AddMinutes(38400)));
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

                aspnet_Applications newApplication = new aspnet_Applications();

                newApplication.ApplicationName = applicationName;
                newApplication.Description = applicationName;
                newApplication.LoweredApplicationName = applicationName.ToLowerInvariant();

                appRepo.Add(newApplication);
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