//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace EdiuxTemplateWebApp.Models.ApplicationLevelModels
{
    using System;
    using System.Collections.Generic;
    
    public partial class Menus
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Menus()
        {
            this.ChildMenus = new HashSet<Menus>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public short Order { get; set; }
        public bool Void { get; set; }
        public bool IsRightAligned { get; set; }
        public bool IsNoActionPage { get; set; }
        public bool AfterBreak { get; set; }
        public Nullable<int> ParentMenuId { get; set; }
        public Nullable<System.Guid> PathId { get; set; }
        public string IconCSS { get; set; }
        public bool IsExternalLinks { get; set; }
        public System.Guid CreateUserId { get; set; }
        public System.DateTime CreateTime { get; set; }
        public Nullable<System.Guid> LastUpdateUserId { get; set; }
        public Nullable<System.DateTime> LastUpdateTime { get; set; }
        public bool AllowAnonymous { get; set; }
        public Nullable<System.Guid> ApplicationId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Menus> ChildMenus { get; set; }
        public virtual Menus ParentMenu { get; set; }
    }
}
