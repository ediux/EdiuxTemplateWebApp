using EdiuxTemplateWebApp.Models.AspNetModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EdiuxTemplateWebApp
{
    public static class UserStoreExtension
    {
        #region User

        public static aspnet_Users AddUser(this aspnet_Applications appObject, string UserName, string Password)
        {
            aspnet_Users.UserRepository = RepositoryHelper.Getaspnet_UsersRepository(aspnet_Applications.ApplicationRepository.UnitOfWork);
            aspnet_Users user = aspnet_Users.UserRepository.Add(UserName, Password, appObject);
            return user;
        }

        public static aspnet_Users FindUserById(this aspnet_Applications appObject, Guid userId)
        {
            return appObject.aspnet_Users.Where(s => s.Id == userId).SingleOrDefault();
        }

        public static aspnet_Users FindUserByName(this aspnet_Applications appObject, string userName)
        {
            return appObject.aspnet_Users.Where(s => s.UserName == userName).SingleOrDefault();
        }

        public static aspnet_Users FindUserByEmail(this aspnet_Applications appObject, string email)
        {
            return appObject.aspnet_Membership.Single(s => s.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase)).aspnet_Users;
        }

        public static aspnet_Users FindUserByLogin(this aspnet_Applications appObject, UserLoginInfo login)
        {
            return appObject.aspnet_Users.SingleOrDefault(s => s.aspnet_UserLogin.Any(k => k.LoginProvider == login.LoginProvider && k.ProviderKey == login.ProviderKey));
        }

        public static aspnet_UserLogin FindLogin(this aspnet_Users userObject,UserLoginInfo login)
        {
            return userObject.aspnet_UserLogin.SingleOrDefault(s => s.LoginProvider == login.LoginProvider && s.ProviderKey == login.ProviderKey);
        }

        public static void DeleteUser(this aspnet_Applications appObject, aspnet_Users user)
        {
            try
            {
                aspnet_Users.UserRepository.Delete(user);
                appObject.aspnet_Users.Remove(user);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Roles
        public static aspnet_Roles AddRole(this aspnet_Applications appObject, string RoleName, string Description = "")
        {
            aspnet_Roles NewRole = aspnet_Roles.RoleRepository.Add(appObject, RoleName, Description);
            appObject.aspnet_Roles.Add(NewRole);
            return NewRole;
        }

        public static void AddRole(this ICollection<aspnet_Roles> objectSet, aspnet_Roles role)
        {
           aspnet_Roles.RoleRepository.Add(role);
            aspnet_Roles.RoleRepository.UnitOfWork.Commit();

            objectSet.Add(role);
        }

        public static void DeleteRole(this ICollection<aspnet_Roles> objectSet, aspnet_Roles role)
        {
            aspnet_Roles.RoleRepository.Delete(role);
            objectSet.Remove(role);
        }

        public static void DeleteRole(this aspnet_Applications appObject, aspnet_Roles role)
        {
            appObject.aspnet_Roles.DeleteRole(role);
        }

        public static aspnet_Roles GetRoleById(this ICollection<aspnet_Roles> objectSet, Guid roleId)
        {
            return objectSet.SingleOrDefault(s => s.Id == roleId);
        }

        public static aspnet_Roles GetRoleById(this aspnet_Applications appObject, Guid roleId)
        {
            return appObject.aspnet_Roles.SingleOrDefault(s => s.Id == roleId);
        }

        public static aspnet_Roles GetRoleByName(this aspnet_Applications appObject, string roleName)
        {
            string loweredRoleName = roleName.ToLowerInvariant();
            return appObject.aspnet_Roles.SingleOrDefault(s => s.Name == roleName && s.LoweredRoleName == loweredRoleName);
        }

        public static IList<string> GetAllRoles(this aspnet_Applications appObject)
        {
            return appObject.aspnet_Roles.Select(s => s.Name).ToList() as IList<string>;
        }
        #endregion

        #region Profile
        public static T GetProfile<T>(this aspnet_Profile profileObject) where T : Models.ProfileModel
        {
            T value= JsonConvert.DeserializeObject<T>(profileObject.PropertyValuesString);
            return value;
        }


        #endregion
    }
}