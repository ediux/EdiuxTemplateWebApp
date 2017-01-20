namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    
    [MetadataType(typeof(aspnet_Membership_GetPassword_ResultMetaData))]
    public partial class aspnet_Membership_GetPassword_Result
    {
    }
    
    public partial class aspnet_Membership_GetPassword_ResultMetaData
    {
        
        [StringLength(128, ErrorMessage="欄位長度不得大於 128 個字元")]
        public string Column1 { get; set; }
        public Nullable<int> Column2 { get; set; }
    }
}
