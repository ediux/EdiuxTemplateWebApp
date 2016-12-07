using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EdiuxTemplateWebApp.Models
{ 
    public class RegisterForAdminsViewModel: RegisterViewModel
    {
        [Required]
        [Display(Name = "顯示名稱")]
        public string DisplayName { get; set; }
        [Required]
        public int RoleId { get; set; }

        public virtual ApplicationRole Role { get; set; }
    }
}