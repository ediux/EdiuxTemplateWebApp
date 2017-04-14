using EdiuxTemplateWebApp.Models.AspNetModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace EdiuxTemplateWebApp.Models.Identity
{
    public class PageSettingByUserViewModel : PageSettingsBaseModel
    {
        public PageSettingByUserViewModel() : base()
        {
            Permission = new PagePermissionForUserModel();
            UserSettings = new Dictionary<string, object>();
        }

        public PageSettingByUserViewModel(aspnet_PersonalizationPerUser userData) :
            base(userData.aspnet_Paths.aspnet_PersonalizationAllUsers)
        {
            var currentdata = userData.PageSettings.Deserialize<PageSettingByUserViewModel>();
            AllowAnonymous = currentdata.AllowAnonymous;
            AllowExcpetionRoles = currentdata.AllowExcpetionRoles;
            AllowExcpetionUsers = currentdata.AllowExcpetionUsers;
            Title = currentdata.Title;
            Description = currentdata.Description;
            Area = currentdata.Area;
            ControllerName = currentdata.ControllerName;
            ActionName = currentdata.ActionName;
            ArgumentObject = currentdata.ArgumentObject;
            MenuId = currentdata.MenuId;
            CSS = currentdata.CSS;
            Permission = currentdata.Permission;
            UserSettings = currentdata.UserSettings;
        }

        /// <summary>
        /// 功能權限
        /// </summary>
        [DisplayName("功能權限")]
        public PagePermissionForUserModel Permission { get; set; }

        /// <summary>
        /// 使用者設定檔
        /// </summary>
        [DisplayName("使用者設定檔")]
        public Dictionary<string,object> UserSettings { get; set; }
    }
}