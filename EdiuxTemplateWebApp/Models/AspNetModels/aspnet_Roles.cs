//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public partial class aspnet_Roles
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public aspnet_Roles()
        {
            this.aspnet_Users = new HashSet<aspnet_Users>();
        }
    
        public System.Guid ApplicationId { get; set; }
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public string LoweredRoleName { get; set; }
        public string Description { get; set; }
    
        public virtual aspnet_Applications aspnet_Applications { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<aspnet_Users> aspnet_Users { get; set; }

        internal Task Update()
        {
            throw new NotImplementedException();
        }
    }
}
