using EdiuxTemplateWebApp.Models.AspNetModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EdiuxTemplateWebApp.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "電子郵件")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "代碼")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "記住此瀏覽器?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Display(Name = "使用電子郵件登入")]
        public bool RequireUniqueEmail { get; set; }

        [Required]
        [Display(Name = "帳號名稱")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "電子郵件")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Display(Name = "使用電子郵件登入")]
        public bool RequireUniqueEmail { get; set; }


        [Display(Name = "帳號名稱")]
        public string UserName { get; set; }


        [Display(Name = "電子郵件")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }

        [Display(Name = "記住我?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Display(Name = "使用電子郵件登入")]
        public bool RequireUniqueEmail { get; set; }

        [Required]
        [Display(Name = "帳號名稱")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "電子郵件")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 的長度至少必須為 {2} 個字元。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "確認密碼")]
        [Compare("Password", ErrorMessage = "密碼和確認密碼不相符。")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Display(Name = "使用電子郵件登入")]
        public bool RequireUniqueEmail { get; set; }

        [Required]
        [Display(Name = "帳號名稱")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "電子郵件")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 的長度至少必須為 {2} 個字元。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "確認密碼")]
        [Compare("Password", ErrorMessage = "密碼和確認密碼不相符。")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Display(Name = "使用電子郵件登入")]
        public bool RequireUniqueEmail { get; set; }

        [Required]
        [Display(Name = "帳號名稱")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "電子郵件")]
        public string Email { get; set; }
    }

    public class ProfileModel
    {
        private IUnitOfWork<AspNetDbEntities2> InternalUnitOfWork;
        public ProfileModel()
        {
            PropertyChanged += ProfileModel_PropertyChanged;
            userId = Guid.Empty;
        }

        public ProfileModel(IUnitOfWork UnitOfwork ):this()
        {
            InternalUnitOfWork = UnitOfwork as IUnitOfWork<AspNetDbEntities2>;
        }

        private Guid userId;
        public Guid UserId { get { return userId; } set { userId = value; RaiseChange(nameof(UserId)); } }

        private void ProfileModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Iaspnet_ProfileRepository profileRepo = RepositoryHelper.Getaspnet_ProfileRepository();

            aspnet_Profile profile = profileRepo.Get(userId);

            profile.PropertyValuesString = JsonConvert.SerializeObject(this);
            profile.PropertyValuesBinary = System.Text.Encoding.Unicode.GetBytes(profile.PropertyValuesString);

            profileRepo.UnitOfWork.Context.Entry(profile).State = System.Data.Entity.EntityState.Modified;
            profileRepo.UnitOfWork.Commit();
        }

        private bool twoFactorEnabled;
        public bool TwoFactorEnabled { get { return twoFactorEnabled; } set { twoFactorEnabled = value; RaiseChange(nameof(TwoFactorEnabled)); } }

        private string securityStamp = string.Empty;
        public string SecurityStamp { get { return securityStamp; } set { securityStamp = value; RaiseChange(nameof(SecurityStamp)); } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaiseChange(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
