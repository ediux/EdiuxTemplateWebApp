namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    
    [MetadataType(typeof(aspnet_Profile_GetProperties_ResultMetaData))]
    public partial class aspnet_Profile_GetProperties_Result
    {
    }
    
    public partial class aspnet_Profile_GetProperties_ResultMetaData
    {
        [Required]
        public string PropertyNames { get; set; }
        [Required]
        public string PropertyValuesString { get; set; }
        [Required]
        public byte[] PropertyValuesBinary { get; set; }
    }
}
