using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EdiuxTemplateWebApp.Models
{
    public class UserProfileViewModel
    {
        public UserProfileViewModel()
        {
            avatarFilePath = "/Content/images/user.jpg";
            positinTitle = "Development manager";
            companyName = "Riaxe";
            companyWebSiteURL = "http://www.riaxe.com/";
        }

        private string avatarFilePath;

        /// <summary>
        /// 頭像檔案路徑
        /// </summary>
        [Required]
        [Display(Name = "頭像檔案路徑")]
        public string AvatarFilePath { get { return avatarFilePath; } set { avatarFilePath = value; } }

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

        private string positinTitle;
        /// <summary>
        /// 職位頭銜
        /// </summary>
        [Display(Name = "職位頭銜")]
        public string PositionTitle { get { return positinTitle; } set { positinTitle = value; } }

        private string companyName;

        [Display(Name = "公司名稱")]
        public string CompanyName { get { return companyName; } set { companyName = value; } }

        private string companyWebSiteURL;
        [Display(Name = "公司網站")]
        public string CompanyWebSiteURL { get { return companyWebSiteURL; } set { companyWebSiteURL = value; } }

    }
}