using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EdiuxTemplateWebApp.Models.ApplicationLevelModels
{
    public class ControllerActionViewModel
    {
        [Required]
        [Display(Name = "資料識別碼")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "動作名稱")]
        public string Name { get; set; }
    }
}