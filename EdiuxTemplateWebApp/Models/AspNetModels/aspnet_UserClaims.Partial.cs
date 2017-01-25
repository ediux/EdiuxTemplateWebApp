namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(aspnet_UserClaimsMetaData))]
    public partial class aspnet_UserClaims : ICloneable
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

    public partial class aspnet_UserClaimsMetaData
    {
        [Required]
        public System.Guid UserId { get; set; }
        [Required]
        public int Id { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
