namespace EdiuxTemplateWebApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    [MetadataType(typeof(MenusMetaData))]
    public partial class Menus
    {
        public void CloneFrom(Menus source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            Name = source.Name;
            IconCSS = source.IconCSS;
            IsExternalLinks = source.IsExternalLinks;
            ExternalURL = source.ExternalURL;
            ParentMenuId = source.ParentMenuId;
            System_ControllerActionsId = source.System_ControllerActionsId;
            Void = source.Void;
            Order = source.Order;
            AllowAnonymous = source.AllowAnonymous;
        }
    }
    
    public partial class MenusMetaData
    {
        [Required]
        [Display(Name ="資料識別碼")]
        public int Id { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        [Required]
        [Display(Name = "選單標題")]
        public string Name { get; set; }
        
        [StringLength(50, ErrorMessage="欄位長度不得大於 50 個字元")]
        [Display(Name = "圖示")]
        public string IconCSS { get; set; }
        [Required]
        [Display(Name = "是否為外部資源?")]
        [UIHint("YesNoDisplay")]
        public bool IsExternalLinks { get; set; }
        
        [StringLength(2048, ErrorMessage="欄位長度不得大於 2048 個字元")]
        [Display(Name = "外部URL")]
        public string ExternalURL { get; set; }
        [Required]
        [Display(Name = "作廢")]
        [UIHint("VoidDisplay")]
        public bool Void { get; set; }
        [Display(Name = "父選單")]
        public Nullable<int> ParentMenuId { get; set; }
        [Required]
        [UIHint("UserIDMappingDisplay")]
        [Display(Name = "建立者")]
        public int CreateUserId { get; set; }
        [Required]
        [Display(Name = "建立時間")]
        [UIHint("UTCLocalTimeDisplay")]
        public System.DateTime CreateTime { get; set; }
        [UIHint("UserIDMappingDisplay")]
        [Display(Name = "更新者")]
        public Nullable<int> LastUpdateUserId { get; set; }
        [Display(Name = "最後修改時間")]
        [UIHint("UTCLocalTimeDisplay")]
        public Nullable<System.DateTime> LastUpdateTime { get; set; }
        [Required]
        [Display(Name = "允許匿名登入")]
        [UIHint("YesNoDisplay")]
        public bool AllowAnonymous { get; set; }
        [Display(Name = "對應控制器動作")]
        public Nullable<int> System_ControllerActionsId { get; set; }
        [Required]
        [Display(Name = "顯示順序")]
        public int Order { get; set; }
        [Display(Name = "子系選單")]
        public virtual ICollection<Menus> ChildMenus { get; set; }
        [Display(Name = "父選單")]
        public virtual Menus ParentMenu { get; set; }
        //[Display(Name = "對應控制器動作")]
        //public virtual System_ControllerActions System_ControllerActions { get; set; }
        [Display(Name = "授權角色")]
        public virtual ICollection<ApplicationRole> ApplicationRole { get; set; }
    }
}
