namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    [MetadataType(typeof(aspnet_ApplicationsMetaData))]
    public partial class aspnet_Applications
    {
        public static aspnet_Applications Create(string applicationName, string applicationDescription = "")
        {
           
            return new aspnet_Applications()
            {
                ApplicationId = Guid.NewGuid(),
                ApplicationName = applicationName,
                Description = applicationDescription,
                LoweredApplicationName = applicationName.ToLowerInvariant()
            };
        }

        public aspnet_Users FindUserById(Guid userId)
        {
            return aspnet_Users.Where(s => s.Id == userId).SingleOrDefault();
        }

        public aspnet_Users FindUserByName(string userName)
        {
            return aspnet_Users.Where(s => s.UserName == userName).SingleOrDefault();
        }

        public IList<string> GetAllRoles()
        {
            return aspnet_Roles.Select(s => s.Name).ToList() as IList<string>;
        }

        public aspnet_Roles FindRoleById(Guid roleId)
        {
            return aspnet_Roles.Where(s => s.Id == roleId).SingleOrDefault();
        }

        public aspnet_Roles FindRoleByName(string roleName)
        {
            return aspnet_Roles.Where(
                s => s.Name == roleName || s.Name == LoweredApplicationName)
                .SingleOrDefault();
        }
    }

    public partial class aspnet_ApplicationsMetaData
    {

        [StringLength(256, ErrorMessage = "欄位長度不得大於 256 個字元")]
        [Required]
        public string ApplicationName { get; set; }

        [StringLength(256, ErrorMessage = "欄位長度不得大於 256 個字元")]
        [Required]
        public string LoweredApplicationName { get; set; }
        [Required]
        public System.Guid ApplicationId { get; set; }

        [StringLength(256, ErrorMessage = "欄位長度不得大於 256 個字元")]
        public string Description { get; set; }

        public virtual ICollection<aspnet_Membership> aspnet_Membership { get; set; }
        public virtual ICollection<aspnet_Paths> aspnet_Paths { get; set; }
        public virtual ICollection<aspnet_Roles> aspnet_Roles { get; set; }
        public virtual ICollection<aspnet_Users> aspnet_Users { get; set; }
    }
}
