using EdiuxTemplateWebApp.Helpers;
using EdiuxTemplateWebApp.Models.AspNetModels;
using Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace EdiuxTemplateWebApp
{
    partial class Startup
    {
        public const string ApplicationInfoKey = "ApplicationInfo";
        internal const string ApplicationName = "ApplicationName";
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

            aspnet_Users rootUser = appInfo.GetUserByName("root", appRepo);

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

            aspnet_Users rootUser = new aspnet_Users();  // ("root", "!QAZ2wsx");
            rootUser.ApplicationId = appInfo.ApplicationId;
            rootUser.UserName = "root";
            rootUser.LoweredUserName = "root";
            rootUser.IsAnonymous = false;
            rootUser.LastActivityDate = DateTime.Now;
            rootUser.MobileAlias = "";

            rootUser.aspnet_Membership = new aspnet_Membership();
            rootUser.aspnet_Membership.AccessFailedCount = 0;
            rootUser.aspnet_Membership.ApplicationId = appInfo.ApplicationId;
            rootUser.aspnet_Membership.aspnet_Applications = appInfo;
            rootUser.aspnet_Membership.Comment = "";
            rootUser.aspnet_Membership.CreateDate = DateTime.Now.Date;
            rootUser.aspnet_Membership.Email = "root@localhost.local";
            rootUser.aspnet_Membership.EmailConfirmed = true;
            rootUser.aspnet_Membership.FailedPasswordAnswerAttemptCount = 0;
            rootUser.aspnet_Membership.FailedPasswordAnswerAttemptWindowStart = new DateTime(1754, 1, 1);
            rootUser.aspnet_Membership.FailedPasswordAttemptCount = 0;
            rootUser.aspnet_Membership.FailedPasswordAttemptWindowStart = new DateTime(1754, 1, 1);
            rootUser.aspnet_Membership.IsApproved = true;
            rootUser.aspnet_Membership.IsLockedOut = false;
            rootUser.aspnet_Membership.LastLockoutDate = new DateTime(1754, 1, 1);
            rootUser.aspnet_Membership.LastLoginDate = new DateTime(1754, 1, 1);
            rootUser.aspnet_Membership.LastPasswordChangedDate = new DateTime(1754, 1, 1);
            rootUser.aspnet_Membership.LoweredEmail = rootUser.aspnet_Membership.Email.ToLowerInvariant();
            rootUser.aspnet_Membership.MobilePIN = "123456";
            rootUser.aspnet_Membership.Password = "!QAZ2wsx";
            rootUser.aspnet_Membership.PasswordAnswer = "";
            rootUser.aspnet_Membership.PasswordFormat = (int)System.Web.Security.MembershipPasswordFormat.Hashed;
            rootUser.aspnet_Membership.PasswordQuestion = "";
            rootUser.aspnet_Membership.PasswordSalt = Path.GetRandomFileName();
            rootUser.aspnet_Membership.PhoneConfirmed = true;
            rootUser.aspnet_Membership.PhoneNumber = "0901-123-456";
            rootUser.aspnet_Membership.ResetPasswordToken = "";

            Models.EdiuxAspNetSqlUserStore store = new Models.EdiuxAspNetSqlUserStore(appRepo.UnitOfWork);
            store.CreateAsync(rootUser);
            store.AddToRoleAsync(rootUser, "Admins");

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
            Iaspnet_RolesRepository roleRepo = null;
            appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();
            roleRepo = RepositoryHelper.Getaspnet_RolesRepository(appRepo.UnitOfWork);

            string appName = getApplicationNameFromConfiguationFile();
            aspnet_Applications appInfo = getApplicationInformationFromCache(appName, appRepo);

            aspnet_Roles[] defaultRoles = new aspnet_Roles[] {
                    new aspnet_Roles() {
                        ApplicationId = appInfo.ApplicationId,
                        Description = "系統管理員",
                        LoweredRoleName = "admins",
                        Name = "Admins",
                        Id = Guid.NewGuid()
                    },
                    new aspnet_Roles() {
                        ApplicationId = appInfo.ApplicationId,
                        Description = "次要管理員",
                        LoweredRoleName = "coadmins",
                        Name = "CoAdmins",
                        Id = Guid.NewGuid()
                    },
                    new aspnet_Roles() {
                        ApplicationId = appInfo.ApplicationId,
                        Description = "使用者",
                        LoweredRoleName = "users",
                        Name = "Users",
                        Id = Guid.NewGuid()
                    }
                };
            
            if (roleRepo.(w=>w appInfo.GetRoleByName("Admins") == null)
            {
                appInfo.aspnet_Roles.AddRole(new aspnet_Roles
                {

                });
                setToMemoryCache(appRepo);
            }
            if (appInfo.GetRoleByName("CoAdmins") == null)
            {
                appInfo.aspnet_Roles.AddRole(new aspnet_Roles
                {

                });
                setToMemoryCache(appRepo);
            }
            if (!roleRepo.IsExists(roles["CoAdmins"]))
            {
                roles["CoAdmins"] = roleRepo.Add(roles["CoAdmins"]);
                setToMemoryCache();
            }
            if (!roleRepo.IsExists(roles["Users"]))
            {
                roles["Users"] = roleRepo.Add(roles["Users"]);
                setToMemoryCache();
            }
        }

        private static aspnet_Applications getApplicationInformationFromCache(string appName, Iaspnet_ApplicationsRepository appRepo = null)
        {
            var appInfo = appName.getApplicationGlobalVariable<aspnet_Applications>(ApplicationInfoKey);

            if (appInfo == null)
            {
                appInfo = appRepo.FindByName(ConfigHelper.GetConfig(ApplicationName)).SingleOrDefault();
                WebHelper.setApplicationGlobalVariable(appName, appName, appInfo);
            }

            return appInfo;
        }

        private void addToMemoryCache(Iaspnet_ApplicationsRepository appRepo = null)
        {
            try
            {
                if (appRepo == null)
                    appRepo = RepositoryHelper.Getaspnet_ApplicationsRepository();

                string appName = getApplicationNameFromConfiguationFile();
                var foundApp = appRepo.FindByName(appName).SingleOrDefault();
                aspnet_Applications app = appRepo.CopyTo<aspnet_Applications>(foundApp) as aspnet_Applications;
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
                aspnet_Applications app = appRepo.CopyTo<aspnet_Applications>(appRepo.FindByName(appName).Single()) as aspnet_Applications;

                MemoryCache.Default.Set(ApplicationInfoKey, app, new DateTimeOffset(DateTime.Now.AddMinutes(38400)));
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

                if (!appRepo.FindByName(appName).Any())
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
            string appName = ConfigHelper.GetConfig(ApplicationName);
            return appName;
        }
    }
}