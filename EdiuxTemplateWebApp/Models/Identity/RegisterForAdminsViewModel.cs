using EdiuxTemplateWebApp.Models.AspNetModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EdiuxTemplateWebApp.Models.Identity
{ 
    public class RegisterForAdminsViewModel: RegisterViewModel
    {
        [Required]
        [Display(Name = "顯示名稱")]
        public string DisplayName { get; set; }
        [Required]
        public Guid RoleId { get; set; }

        public virtual aspnet_Roles Role { get; set; }
    }
}