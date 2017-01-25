namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(aspnet_WebEvent_EventsMetaData))]
    public partial class aspnet_WebEvent_Events : ICloneable
    {
        public object Clone()
        {
            Type sourceType = this.GetType();

            var copy = Activator.CreateInstance(sourceType);

            Type tatgetType = copy.GetType();

            var props_src = sourceType.GetProperties();

            foreach (var srcProp in props_src)
            {
                if (srcProp.PropertyType.IsGenericType)
                {
                    if (srcProp.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                    {
                        var targetProp = tatgetType.GetProperty(srcProp.Name);
                        targetProp.SetValue(copy, Activator.CreateInstance(typeof(Collection<>).MakeGenericType(srcProp.PropertyType.GenericTypeArguments)));
                    }
                }
                else
                {
                    var targetProp = tatgetType.GetProperty(srcProp.Name);
                    targetProp.SetValue(copy, srcProp.GetValue(this));
                }
            }

            return copy;
        }
    }

    public partial class aspnet_WebEvent_EventsMetaData
    {
        
        [StringLength(32, ErrorMessage="欄位長度不得大於 32 個字元")]
        [Required]
        public string EventId { get; set; }
        [Required]
        public System.DateTime EventTimeUtc { get; set; }
        [Required]
        public System.DateTime EventTime { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        [Required]
        public string EventType { get; set; }
        [Required]
        public decimal EventSequence { get; set; }
        [Required]
        public decimal EventOccurrence { get; set; }
        [Required]
        public int EventCode { get; set; }
        [Required]
        public int EventDetailCode { get; set; }
        
        [StringLength(1024, ErrorMessage="欄位長度不得大於 1024 個字元")]
        public string Message { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        public string ApplicationPath { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        public string ApplicationVirtualPath { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        [Required]
        public string MachineName { get; set; }
        
        [StringLength(1024, ErrorMessage="欄位長度不得大於 1024 個字元")]
        public string RequestUrl { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        public string ExceptionType { get; set; }
        public string Details { get; set; }
    }
}
