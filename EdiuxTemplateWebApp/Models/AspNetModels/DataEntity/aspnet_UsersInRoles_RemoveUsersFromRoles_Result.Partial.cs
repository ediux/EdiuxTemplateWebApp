namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    
    [MetadataType(typeof(aspnet_UsersInRoles_RemoveUsersFromRoles_ResultMetaData))]
    public partial class aspnet_UsersInRoles_RemoveUsersFromRoles_Result
    {
    }
    
    public partial class aspnet_UsersInRoles_RemoveUsersFromRoles_ResultMetaData
    {
        
        [StringLength(1, ErrorMessage="欄位長度不得大於 1 個字元")]
        [Required]
        public string Column1 { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        [Required]
        public string Name { get; set; }
    }
}
