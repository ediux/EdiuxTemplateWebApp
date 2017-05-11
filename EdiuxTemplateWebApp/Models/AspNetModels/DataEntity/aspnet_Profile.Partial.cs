namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using EdiuxTemplateWebApp.Models.Identity;
    using Newtonsoft.Json;
    using System;

    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Web.Script.Serialization;

    [MetadataType(typeof(aspnet_ProfileMetaData))]
    public partial class aspnet_Profile
    {
        /// <summary>
        /// 根據參數建立資料執行個體
        /// </summary>
        /// <returns></returns>
        internal static aspnet_Profile Create(aspnet_Users user = null)
        {
            aspnet_Profile newProfile = new aspnet_Profile();

            var profileViweModel = UserProfileViewModel.Create();
            
            newProfile.PropertyValuesBinary = newProfile.Serialize();
            newProfile.PropertyValuesString = JsonConvert.SerializeObject(profileViweModel);
            newProfile.PropertyNames = string.Join(",", profileViweModel.GetProperties().Keys.ToArray());
            newProfile.LastUpdatedDate = DateTime.UtcNow;
            newProfile.UserId = user?.Id ?? Guid.Empty;
            newProfile.aspnet_Users = user;

            return newProfile;
        }

        [ScriptIgnore]
        [IgnoreDataMember]
        public UserProfileViewModel Profile
        {
            get
            {
                return PropertyValuesBinary.Deserialize<UserProfileViewModel>();
            }
            set
            {
                PropertyValuesBinary = value.Serialize();
            }
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
