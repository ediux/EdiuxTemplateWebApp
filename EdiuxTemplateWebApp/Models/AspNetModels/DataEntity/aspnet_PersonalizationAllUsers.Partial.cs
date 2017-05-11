namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using EdiuxTemplateWebApp.Filters;
    using EdiuxTemplateWebApp.Models.Identity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;

    [MetadataType(typeof(aspnet_PersonalizationAllUsersMetaData))]
    public partial class aspnet_PersonalizationAllUsers
    {
        /// <summary>
        ///  根據參數建立資料執行個體
        /// </summary>
        /// <param name="path"></param>
        /// <param name="basePageSettingModel"></param>
        /// <returns></returns>
        internal static aspnet_PersonalizationAllUsers Create(aspnet_Paths path, PageSettingsBaseModel basePageSettingModel = null)
        {
            var newInstace = new aspnet_PersonalizationAllUsers()
            {
                aspnet_Paths = path,
                LastUpdatedDate = DateTime.UtcNow,
                PageSettings = (basePageSettingModel ?? new PageSettingsBaseModel()
                {
                    ActionName = "",
                    ControllerName = "",
                    Area = string.Empty,
                    ArgumentObject = null,
                    CommonSettings = new Dictionary<string, object>(),
                    CSS = string.Empty,
                    Description = string.Empty,
                    MenuId = Guid.Empty,
                    Title = string.Empty,
                    AllowAnonymous = true,
                    AllowExcpetionRoles = new Dictionary<string, bool>(),
                    AllowExcpetionUsers = new Dictionary<string, bool>()
                }).Serialize(),
                PathId = path.PathId
            };
         
            return newInstace;
        }

        internal static aspnet_PersonalizationAllUsers Create(aspnet_Paths path, ActionExecutingContext filterContext)
        {
            PageSettingsBaseModel newSettingModel = new PageSettingsBaseModel()
            {
                ActionName = filterContext.ActionDescriptor.ActionName,
                ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                Area = filterContext.RequestContext.RouteData.DataTokens["area"]?.ToString(),
                ArgumentObject = filterContext.ActionParameters,
                CommonSettings = new Dictionary<string, object>(),
                CSS = filterContext.Controller.ViewBag.CSS ?? string.Empty,
                Description = filterContext.Controller.ViewBag.Description ?? string.Empty,
                MenuId = Guid.Empty,
                Title = filterContext.Controller.ViewBag.Title ?? string.Empty,
                AllowAnonymous = true,
                AllowExcpetionRoles = new Dictionary<string, bool>(),
                AllowExcpetionUsers = new Dictionary<string, bool>()
            };

            #region 尋找原始的授權屬性
            var auth = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AuthorizeAttribute), true)
                .Select(s => (AuthorizeAttribute)s).ToList().SingleOrDefault();

            if (auth != null)
            {
                var allowRoles = auth.Roles.ToLowerInvariant().Split(',');

                foreach (var r in allowRoles)
                {
                    newSettingModel.AllowExcpetionRoles.Add(r, true);
                }

                var allowUsers = auth.Users.ToLowerInvariant().Split(',');

                foreach (var u in allowUsers)
                {
                    newSettingModel.AllowExcpetionUsers.Add(u, true);
                }

                newSettingModel.AllowAnonymous = false;
            }

            #endregion

            #region 尋找自訂的授權屬性
            var adbuth = filterContext.ActionDescriptor.GetCustomAttributes(typeof(DbAuthorizeAttribute), true)
                .Select(s => (DbAuthorizeAttribute)s).ToList().SingleOrDefault();

            if (auth != null)
            {
                var allowRoles = auth.Roles.ToLowerInvariant().Split(',');

                foreach (var r in allowRoles)
                {
                    newSettingModel.AllowExcpetionRoles.Add(r, true);
                }

                var allowUsers = auth.Users.ToLowerInvariant().Split(',');

                foreach (var u in allowUsers)
                {
                    newSettingModel.AllowExcpetionUsers.Add(u, true);
                }

                newSettingModel.AllowAnonymous = false;
            }
            #endregion

            return new aspnet_PersonalizationAllUsers()
            {
                aspnet_Paths = path,
                LastUpdatedDate = DateTime.UtcNow,
                PageSettings = newSettingModel.Serialize(),
                PathId = path.PathId
            };
        }

        [ScriptIgnore]
        [IgnoreDataMember]
        public PageSettingsBaseModel Settings
        {
            get
            {
                return PageSettings.Deserialize<PageSettingsBaseModel>();
            }
            set
            {
                PageSettings = value.Serialize();
            }
        }
    }

    public partial class aspnet_PersonalizationAllUsersMetaData
    {
        [Display(Name = "路徑識別碼")]
        [Required]
        public System.Guid PathId { get; set; }
        [Display(Name = "網頁設定儲存體")]
        [Required]
        public byte[] PageSettings { get; set; }
        [Display(Name = "最後更新日期")]
        [Required]
        public System.DateTime LastUpdatedDate { get; set; }
        [Display(Name = "對應路徑")]
        public virtual aspnet_Paths aspnet_Paths { get; set; }

        [Display(Name = "網頁共通設定")]
        public PageSettingsBaseModel Settings { get; set; }
    }
}
