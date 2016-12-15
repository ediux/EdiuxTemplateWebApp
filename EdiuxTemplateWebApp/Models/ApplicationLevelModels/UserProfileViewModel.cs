using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EdiuxTemplateWebApp.Models
{
    public class UserProfileViewModel : ApplicationUser
    {

        private IApplicationRoleRepository roleRepo;

        public UserProfileViewModel() : base()
        {
            avatarFilePath = "/Content/images/user.jpg";
            positinTitle = "Development manager";
            companyName = "Riaxe";
            companyWebSiteURL = "http://www.riaxe.com/";

            roleRepo = RepositoryHelper.GetApplicationRoleRepository();
        }

        public void SetUnitOfWork(IUnitOfWork unitofwork)
        {
            roleRepo.UnitOfWork = unitofwork;
        }
        public UserProfileViewModel(IUnitOfWork unitofwork) : base()
        {
            roleRepo = RepositoryHelper.GetApplicationRoleRepository(unitofwork);
        }

        private int getRoleId()
        {
            if (ApplicationRole.Any())
            {
                return ApplicationRole.First().Id;
            }

            return 0;
        }

        private void setRoleId(int value)
        {
            int currentroleid = getRoleId();
            if (currentroleid != value)
            {
                //先移除現在的
                ApplicationRole.Clear();

                //再加入新增的
                Task<ApplicationRole> getRoleTask = roleRepo.FindByIdAsync(currentroleid);
                getRoleTask.Wait();
                ApplicationRole.Add(getRoleTask.Result);
            }
        }

        public override void CloneFrom(ApplicationUser source)
        {
            base.CloneFrom(source);

            if (ApplicationUserClaim.Count > 0)
            {
                foreach (var item in ApplicationUserClaim)
                {
                    switch (item.ClaimType)
                    {
                        case "FirstName":
                            FirstName = item.ClaimValue;
                            break;
                        case "MiddleName":
                            MiddleName = item.ClaimValue;
                            break;
                        case "LastName":
                            LastName = item.ClaimValue;
                            break;
                        case "Gender":
                            bool isMale = false;
                            if(bool.TryParse(item.ClaimValue,out isMale) == false)
                            {
                                Gender = false;
                            }
                            break;
                    }

                }
            }
        }
        public int RoleId { get { return getRoleId(); } set { setRoleId(value); } }


        public IndexViewModel UserAccountManage { get; set; }

        private string avatarFilePath;

        /// <summary>
        /// 頭像檔案路徑
        /// </summary>
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

        [Required]
        [Display(Name = "性別")]
        public bool Gender { get; set; }
    }
}