using EdiuxTemplateWebApp.Models.AspNetModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EdiuxTemplateWebApp.Models.Identity
{
    [Serializable]
    public class PageSettingsBaseModel
    {
        public PageSettingsBaseModel()
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
        }

        public PageSettingsBaseModel(aspnet_PersonalizationAllUsers pagebaseData) : this()
        {
            var data = pagebaseData.PageSettings.Deserialize<PageSettingsBaseModel>();

            if (data != null)
            {
                AllowAnonymous = data.AllowAnonymous;
                AllowExcpetionRoles = data.AllowExcpetionRoles;
                AllowExcpetionUsers = data.AllowExcpetionUsers;
                Title = data.Title;
                Description = data.Description;
                Area = data.Area;
                ControllerName = data.ControllerName;
                ActionName = data.ActionName;
                ArgumentObject = data.ArgumentObject;
                MenuId = data.MenuId;
                CSS = data.CSS;
                CommonSettings = data.CommonSettings;

            }
        }

        public static PageSettingsBaseModel Get(Guid pathId)
        {
            Iaspnet_PersonalizationAllUsersRepository pageSettingRepo = RepositoryHelper.Getaspnet_PersonalizationAllUsersRepository();
            aspnet_PersonalizationAllUsers data = pageSettingRepo.Where(s => s.PathId == pathId).SingleOrDefault();


            if (data == null)
            {
                data = new aspnet_PersonalizationAllUsers();


                PageSettingsBaseModel newSettings = new PageSettingsBaseModel();
                data.LastUpdatedDate = new DateTime(1900, 1, 1);
                data.PathId = pathId;
                data.PageSettings = newSettings.Serialize();

                pageSettingRepo.Add(data);
                pageSettingRepo.UnitOfWork.Commit();
                data = pageSettingRepo.Reload(data);
            }
            return data.PageSettings.Deserialize<PageSettingsBaseModel>();
        }

        public static void Set(PageSettingsBaseModel value, Guid pathId)
        {
            Iaspnet_PersonalizationAllUsersRepository pageSettingRepo = RepositoryHelper.Getaspnet_PersonalizationAllUsersRepository();
            aspnet_PersonalizationAllUsers profileData = pageSettingRepo.Where(w => w.PathId == pathId).SingleOrDefault();

            if (profileData == null)
            {
                PageSettingsBaseModel newSettings = new PageSettingsBaseModel();
                profileData.LastUpdatedDate = new DateTime(1900, 1, 1);
                profileData.PathId = pathId;
                profileData.PageSettings = newSettings.Serialize();
                pageSettingRepo.Add(profileData);
            }
            else
            {
                profileData.PageSettings = profileData.Serialize();
                profileData.PathId = pathId;
                profileData.LastUpdatedDate = DateTime.Now;

            }
            pageSettingRepo.UnitOfWork.Commit();

        }

        [DisplayName("允許匿名存取")]
        public bool AllowAnonymous { get; set; }

        [DisplayName("允許例外存取的角色列表")]
        public Dictionary<string, bool> AllowExcpetionRoles { get; set; }

        [DisplayName("允許例外存取的使用者列表")]
        public Dictionary<string, bool> AllowExcpetionUsers { get; set; }

        [DisplayName("頁面標題")]
        public string Title { get; set; }

        [DisplayName("頁面描述")]
        public string Description { get; set; }

        [DisplayName("子區域名稱")]
        public string Area { get; set; }

        [DisplayName("對應控制器名稱")]
        public string ControllerName { get; set; }

        [DisplayName("對應動作名稱")]
        public string ActionName { get; set; }

        [DisplayName("引數物件")]
        public object ArgumentObject { get; set; }

        [DisplayName("對應選單識別碼")]
        public Guid MenuId { get; set; }

        [DisplayName("預設的CSS內容資源")]
        public string CSS { get; set; }

        [DisplayName("功能全域設定檔")]
        public Dictionary<string, object> CommonSettings { get; set; }
    }
}