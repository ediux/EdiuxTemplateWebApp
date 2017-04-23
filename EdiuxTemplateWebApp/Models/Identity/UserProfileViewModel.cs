using EdiuxTemplateWebApp.Models.AspNetModels;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EdiuxTemplateWebApp.Models.Identity
{
    [Serializable]
    public class UserProfileViewModel
    {
        public static UserProfileViewModel Create()
        {
            UserProfileViewModel newProfileViewModel = new UserProfileViewModel()
            {
                AvatarFilePath = "/Content/images/user.jpg",
                CompanyName = "Ediux Workshop",
                PositionTitle = "PG",
                CompanyWebSiteURL = "http://www.riaxe.com/"
            };

            return newProfileViewModel;
        }
        public static UserProfileViewModel Get(Guid userId)
        {
            Iaspnet_ProfileRepository profileRepo = RepositoryHelper.Getaspnet_ProfileRepository();
            aspnet_Profile profileData = profileRepo.Where(w => w.UserId == userId).SingleOrDefault();
            if (profileData == null)
            {
                profileData = new aspnet_Profile();
                profileData.UserId = userId;

                UserProfileViewModel newProfile = new UserProfileViewModel();
                profileData.PropertyValuesString = JsonConvert.SerializeObject(newProfile);

                newProfile.AvatarFilePath = "/Content/images/user.jpg";
                newProfile.CompanyName = "Ediux Workshop";
                newProfile.PositionTitle = "PG";
                newProfile.CompanyWebSiteURL = "http://www.riaxe.com/";

                profileData.PropertyValuesBinary = newProfile.Serialize();
                profileData.PropertyValuesString = "{}";
                profileData.PropertyNames = string.Join(",", newProfile.GetProperties().Keys.ToArray());
                profileData.LastUpdatedDate = DateTime.UtcNow;
               
                profileRepo.Add(profileData);
                profileRepo.UnitOfWork.Commit();
                profileData = profileRepo.Reload(profileData);
            }
            return profileData.PropertyValuesBinary.Deserialize<UserProfileViewModel>();
        }

        public static void Set(UserProfileViewModel value, Guid userId)
        {
            Iaspnet_ProfileRepository profileRepo = RepositoryHelper.Getaspnet_ProfileRepository();
            aspnet_Profile profileData = profileRepo.Where(w => w.UserId == userId).SingleOrDefault();

            if (profileData == null)
            {
                profileData = new aspnet_Profile();
                profileData.UserId = userId;
                profileData.PropertyValuesBinary = value.Serialize();
                profileData.PropertyValuesString = JsonConvert.SerializeObject(value);
                profileData.PropertyNames = string.Join(",", value.GetProperties().Keys.ToArray());
                profileData.LastUpdatedDate = DateTime.UtcNow;

                profileRepo.Add(profileData);
                profileRepo.UnitOfWork.Commit();
            }
            else
            {
                profileData.PropertyValuesBinary = profileData.Serialize();
                profileData.PropertyValuesString = JsonConvert.SerializeObject(value);
                profileData.PropertyNames = string.Join(",", value.GetProperties().Keys.ToArray());
                profileData.LastUpdatedDate = DateTime.UtcNow;
                profileRepo.Update(profileData);
                profileRepo.UnitOfWork.Commit();
            }

        }

        /// <summary>
        /// 頭像檔案路徑
        /// </summary>
        [Display(Name = "頭像檔案路徑")]
        public string AvatarFilePath
        {
            get;
            set;
        }

        /// <summary>
        /// 名字
        /// </summary>
        [Display(Name = "名字")]
        public string FirstName { get; set; }

        /// <summary>
        /// 中間名字
        /// </summary>
        [Display(Name = "中間名字")]
        public string MiddleName { get; set; }

        /// <summary>
        /// 姓氏
        /// </summary>
        [Display(Name = "姓氏")]
        public string LastName { get; set; }

        /// <summary>
        /// 職位頭銜
        /// </summary>
        [Display(Name = "職位頭銜")]
        public string PositionTitle { get; set; }


        [Display(Name = "公司名稱")]
        public string CompanyName { get; set; }

        private string companyWebSiteURL;
        [Display(Name = "公司網站")]
        public string CompanyWebSiteURL { get { return companyWebSiteURL; } set { companyWebSiteURL = value; } }

        [Required]
        [Display(Name = "性別")]
        public bool Gender { get; set; }

        [Display(Name = "啟用兩步驟驗證")]
        public bool TwoFactorEnabled { get; set; }

        [Display(Name = "安全性時戳")]
        public string SecurityStamp { get; set; }

        [Display(Name = "是否記憶瀏覽器")]
        public bool BrowserRemembered { get; set; }

    }
}