namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using Microsoft.AspNet.Identity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    [MetadataType(typeof(aspnet_RolesMetaData))]
    public partial class aspnet_Roles : IRole<Guid>
    {
        /// <summary>
        /// 根據參數建立資料執行個體
        /// </summary>
        /// <param name="applicationInfo"></param>
        /// <param name="roleName"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        internal static aspnet_Roles Create(aspnet_Applications applicationInfo, string roleName, string description = "")
        {
            return new aspnet_Roles()
            {
                ApplicationId = applicationInfo.ApplicationId,
                aspnet_Applications = applicationInfo,
                aspnet_Users = new Collection<aspnet_Users>(),
                Description = description,
                Id = Guid.NewGuid(),
                LoweredRoleName = roleName.ToLowerInvariant(),
                Name = roleName,
                Menus = new Collection<Menus>()
            };
        }

        /// <summary>
        /// 根據參數建立資料執行個體
        /// </summary>
        /// <param name="applicationInfo"></param>
        /// <param name="Roles"></param>
        /// <param name="user"></param>
        /// <param name="PredefinedMenus"></param>
        /// <returns></returns>
        internal static ICollection<aspnet_Roles> CreateCollection(aspnet_Applications applicationInfo, string[] Roles, aspnet_Users user = null, IList<Menus> PredefinedMenus = null)
        {
            if (applicationInfo == null)
            {
                throw new ArgumentNullException(nameof(applicationInfo));
            }

            if (Roles == null || !Roles.Any())
            {
                if (!applicationInfo.aspnet_Roles.Any())
                {
                    Roles = new string[] { "Admins", "Users", "Guests" };
                }
                else
                {
                    return applicationInfo.aspnet_Roles;
                }

            }

            var existedRoles = (from r in applicationInfo.aspnet_Roles
                                select r.Name).ToArray();

            var addtoCollectionList = existedRoles.Except(Roles);

            ICollection<aspnet_Roles> newRoles = new Collection<aspnet_Roles>(applicationInfo.aspnet_Roles.ToList());

            foreach (var role in addtoCollectionList)
            {
                aspnet_Roles newRole = new aspnet_Roles()
                {
                    ApplicationId = applicationInfo.ApplicationId,
                    aspnet_Applications = applicationInfo,
                    Description = string.Empty,
                    Id = Guid.NewGuid(),
                    LoweredRoleName = role.ToLowerInvariant(),
                    Name = role,
                    aspnet_Users = new Collection<aspnet_Users>(),
                    Menus = new Collection<Menus>()
                };

                if (user != null)
                {
                    newRole.aspnet_Users.Add(user);
                }

                if (PredefinedMenus != null)
                {
                    newRole.Menus = new Collection<Menus>(PredefinedMenus);
                }

                newRoles.Add(newRole);
            }

            return newRoles;
        }
    }

    public partial class aspnet_RolesMetaData
    {
        [Required]
        public System.Guid ApplicationId { get; set; }
        [Required]
        public System.Guid Id { get; set; }

        [StringLength(256, ErrorMessage = "欄位長度不得大於 256 個字元")]
        [Required]
        public string Name { get; set; }

        [StringLength(256, ErrorMessage = "欄位長度不得大於 256 個字元")]
        [Required]
        public string LoweredRoleName { get; set; }

        [StringLength(256, ErrorMessage = "欄位長度不得大於 256 個字元")]
        public string Description { get; set; }
        public virtual aspnet_Applications aspnet_Applications { get; set; }
        public virtual ICollection<aspnet_Users> aspnet_Users { get; set; }
        public virtual ICollection<Menus> Menus { get; set; }
    }
}
