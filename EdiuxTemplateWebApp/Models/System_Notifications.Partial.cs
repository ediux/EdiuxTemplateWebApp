namespace EdiuxTemplateWebApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    [MetadataType(typeof(System_NotificationsMetaData))]
    public partial class System_Notifications
    {
    }
    
    public partial class System_NotificationsMetaData
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int FromUserId { get; set; }
        [Required]
        public int TargetUserId { get; set; }
        
        [StringLength(1024, ErrorMessage="欄位長度不得大於 1024 個字元")]
        [Required]
        public string Subject { get; set; }
        public string Message { get; set; }
        
        [StringLength(2048, ErrorMessage="欄位長度不得大於 2048 個字元")]
        public string MessageLink { get; set; }
        [Required]
        public bool Read { get; set; }
        public Nullable<int> RelayNotificationId { get; set; }
        [Required]
        public System.DateTime CreateTime { get; set; }
        public Nullable<System.DateTime> RelayTime { get; set; }
    
        public virtual ApplicationUser Sender { get; set; }
        public virtual ApplicationUser Recipient { get; set; }
        public virtual ICollection<System_Notifications> OriginalNotifications { get; set; }
    }
}
