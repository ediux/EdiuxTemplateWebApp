using EdiuxTemplateWebApp.Models.AspNetModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace EdiuxTemplateWebApp.Models.Identity
{
    [Serializable]
    public class PageSettingByUserViewModel : PageSettingsBaseModel
    {
        public PageSettingByUserViewModel() : base()
        {
            Permission = new PagePermissionForUserModel();
            UserSettings = new Dictionary<string, object>();
        }

        public PageSettingByUserViewModel(aspnet_PersonalizationAllUsers commonUserData,Guid userId) :
            base(commonUserData)
        {
            if (commonUserData.aspnet_Paths == null)
            {
                AllowAnonymous = true;
                AllowExcpetionRoles = new Dictionary<string, bool>();
                AllowExcpetionRoles.Add("Admins", true);
                AllowExcpetionUsers = new Dictionary<string, bool>();
                AllowExcpetionUsers.Add("root", true);
                Title = "未命名";
                Description = "";
                Area = "";
                ControllerName = "";
                ActionName = "";
                ArgumentObject = new object();
                MenuId = Guid.Empty;
                CSS = "";
                CommonSettings = new Dictionary<string, object>();
                Permission = new PagePermissionForUserModel();
                UserSettings = new Dictionary<string, object>();
            }
            else
            {
                var currentdata = (from um in commonUserData.aspnet_Paths.aspnet_PersonalizationPerUser
                                   where um.UserId == userId
                                   select um).SingleOrDefault();

                if (currentdata != null)
                {
                    PageSettingByUserViewModel userSettings = currentdata.PageSettings.Deserialize<PageSettingByUserViewModel>();

                    Permission = userSettings.Permission;
                    UserSettings = userSettings.UserSettings;
                }
                else
                {
                    AllowAnonymous = true;
                    AllowExcpetionRoles = new Dictionary<string, bool>();
                    AllowExcpetionRoles.Add("Admins", true);
                    AllowExcpetionUsers = new Dictionary<string, bool>();
                    AllowExcpetionUsers.Add("root", true);
                    Title = "未命名";
                    Description = "";
                    Area = "";
                    ControllerName = "";
                    ActionName = "";
                    ArgumentObject = new object();
                    MenuId = Guid.Empty;
                    CSS = "";
                    CommonSettings = new Dictionary<string, object>();
                    Permission = new PagePermissionForUserModel();
                    UserSettings = new Dictionary<string, object>();
                }

            }

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