using EdiuxTemplateWebApp.Models.AspNetModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace EdiuxTemplateWebApp.Models
{
    public class PageSettingsBaseModel
    {
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

        [DisplayName("允許存取的角色列表")]
        public Dictionary<Guid, bool> AllowRoles { get; set; }

        [DisplayName("允許存取的使用者列表")]
        public Dictionary<Guid, bool> AllowUsers { get; set; }

        [DisplayName("允許例外存取的角色列表")]
        public Dictionary<Guid, bool> AllowExcpetionRoles { get; set; }

        [DisplayName("允許例外存取的使用者列表")]
        public Dictionary<Guid, bool> AllowExcpetionUsers { get; set; }

        [DisplayName("頁面標題")]
        public string Title { get; set; }

        [DisplayName("頁面描述")]
        public string Description { get; set; }

        [DisplayName("對應控制器名稱")]
        public string ControllerName { get; set; }

        [DisplayName("對應動作名稱")]
        public string ActionName { get; set; }

        [DisplayName("引數物件")]
        public object ArgumentObject { get; set; }

        [DisplayName("對應選單識別碼")]
        public Guid MenuId { get; set; }
    }
}