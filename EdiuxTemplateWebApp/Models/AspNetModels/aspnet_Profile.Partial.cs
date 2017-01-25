namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(aspnet_ProfileMetaData))]
    public partial class aspnet_Profile : ICloneable
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

    public partial class aspnet_ProfileMetaData
    {
        [Required]
        public System.Guid UserId { get; set; }
        [Required]
        public string PropertyNames { get; set; }
        [Required]
        public string PropertyValuesString { get; set; }
        [Required]
        public byte[] PropertyValuesBinary { get; set; }
        [Required]
        public System.DateTime LastUpdatedDate { get; set; }

        public virtual aspnet_Users aspnet_Users { get; set; }
    }
}
