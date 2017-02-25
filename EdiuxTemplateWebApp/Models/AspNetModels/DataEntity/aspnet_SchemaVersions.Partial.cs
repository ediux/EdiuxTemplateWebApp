namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using System;
    using System.Collections.Generic;
    
    using System.ComponentModel.DataAnnotations;
    
    [MetadataType(typeof(aspnet_SchemaVersionsMetaData))]
    public partial class aspnet_SchemaVersions
    {
    }
    
    public partial class aspnet_SchemaVersionsMetaData
    {
        
        [StringLength(128, ErrorMessage="欄位長度不得大於 128 個字元")]
        [Required]
        public string Feature { get; set; }
        
        [StringLength(128, ErrorMessage="欄位長度不得大於 128 個字元")]
        [Required]
        public string CompatibleSchemaVersion { get; set; }
        [Required]
        public bool IsCurrentVersion { get; set; }
    }
}
