namespace EdiuxTemplateWebApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;

    [MetadataType(typeof(System_ApplicationsMetaData))]
    public partial class System_Applications
    {
        public bool isUserInApplication(ApplicationUser user)
        {
            return ApplicationUsers.Any(w => w.Id == user.Id);
        }

        public bool isUsersInApplicationByName(params string[] userNames)
        {
            return userNames.Any(s => ApplicationUsers.Any(x => x.UserName == s));
        }

        public bool isUserInApplicationByUserIds(params int[] userIds)
        {
            return ApplicationUsers.Any(w => userIds.Any(x => x == w.Id));
        }

        public bool isRoleInApplication(ApplicationRole role)
        {
            return ApplicationRoles.Any(w => w.Id == role.Id);
        }
        public bool isRolesInApplicationByRoleIds(params int[] roleIds)
        {
            return ApplicationRoles.Any(w => roleIds.Any(x => x == w.Id));
        }

        public bool isRolesInApplication(params string[] roleNames)
        {
            return ApplicationRoles.Any(w => roleNames.Any(x => x == w.Name));
        }

        public bool isActionInApplication(ActionDescriptor actionDesc)
        {
            return System_Controllers.Any(w => w.ClassName == actionDesc.ControllerDescriptor.ControllerType.Name
            && w.Namespace == actionDesc.ControllerDescriptor.ControllerType.Namespace
            && w.System_ControllerActions.Any(x => x.Name == actionDesc.ActionName));
        }

        public System_ControllerActions getMVCActionInfo(ActionDescriptor actionDesc)
        {
            return getMVCControllerInfo(actionDesc).System_ControllerActions.SingleOrDefault(s => s.Name == actionDesc.ActionName);
        }

        public System_Controllers getMVCControllerInfo(ActionDescriptor actionDesc)
        {
            return System_Controllers.SingleOrDefault(w => w.ClassName == actionDesc.ControllerDescriptor.ControllerType.Name 
                                                   && w.Namespace == actionDesc.ControllerDescriptor.ControllerType.Namespace);
        }

        public ApplicationUser[] getUsers(params int[] userIds)
        {
            return ApplicationUsers.Where(w => userIds.Any(s => s == w.Id)).ToArray();
        }

        public ApplicationUser[] getUserByName(params string[] userNames)
        {
            return ApplicationUsers.Where(w => userNames.Any(x => x == w.UserName)).ToArray();
        }

        public ApplicationRole[] getRoles(params int[] roleIds)
        {
            return ApplicationRoles.Where(s => roleIds.Any(x => x == s.Id)).ToArray();
        }

        public ApplicationRole[] getRolesByName(params string[] roleNames)
        {
            return ApplicationRoles.Where(s => roleNames.Any(x => x == s.Name)).ToArray();
        }

        public IQueryable<Menus> getMenusbyCurrentLoginUser(ApplicationUser loginedUser)
        {
            try
            {
                if (loginedUser == null)
                    throw new ArgumentNullException(nameof(loginedUser));

                var authorizedMenus = loginedUser.ApplicationRole.SelectMany(s => s.Menus);
                var allanonymousMenus = loginedUser.ApplicationRole.SelectMany(s => s.Menus.Where(x => x.AllowAnonymous));
                var outputMenus = authorizedMenus.Union(allanonymousMenus).Distinct();

                return outputMenus.AsQueryable();  
            }
            catch (Exception ex)
            {                
                throw ex;
            }
        }
    }

    public partial class System_ApplicationsMetaData
    {
        [Required]
        public int Id { get; set; }

        [StringLength(256, ErrorMessage = "欄位長度不得大於 256 個字元")]
        [Required]
        [Display(Name = "應用程式名稱")]
        public string Name { get; set; }

        [StringLength(256, ErrorMessage = "欄位長度不得大於 256 個字元")]
        [Required]
        [Display(Name = "應用程式名稱")]
        public string LoweredName { get; set; }

        [StringLength(256, ErrorMessage = "欄位長度不得大於 256 個字元")]
        [Display(Name = "描述說明")]
        public string Description { get; set; }

        [StringLength(2048, ErrorMessage = "欄位長度不得大於 2048 個字元")]
        [Display(Name = "命名空間")]
        [Required]
        public string Namespace { get; set; }

        [Display(Name = "選單")]
        public virtual ICollection<Menus> Menus { get; set; }
        [Display(Name = "控制器")]
        public virtual ICollection<System_Controllers> System_Controllers { get; set; }
        [Display(Name = "可用角色")]
        public virtual ICollection<ApplicationRole> ApplicationRoles { get; set; }
        [Display(Name = "可用使用者帳號")]
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }
}
