namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    
    [MetadataType(typeof(aspnet_Membership_GetPasswordWithFormat_ResultMetaData))]
    public partial class aspnet_Membership_GetPasswordWithFormat_Result
    {
    }
    
    public partial class aspnet_Membership_GetPasswordWithFormat_ResultMetaData
    {
        
        [StringLength(128, ErrorMessage="欄位長度不得大於 128 個字元")]
        public string Column1 { get; set; }
        public Nullable<int> Column2 { get; set; }
        
        [StringLength(128, ErrorMessage="欄位長度不得大於 128 個字元")]
        public string Column3 { get; set; }
        public Nullable<int> Column4 { get; set; }
        public Nullable<int> Column5 { get; set; }
        public Nullable<bool> Column6 { get; set; }
        public Nullable<System.DateTime> Column7 { get; set; }
        public Nullable<System.DateTime> Column8 { get; set; }
    }
}
