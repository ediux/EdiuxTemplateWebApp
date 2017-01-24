namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using Microsoft.AspNet.Identity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(aspnet_RolesMetaData))]
    public partial class aspnet_Roles : IRole<Guid>
    {
        private static Iaspnet_RolesRepository roleRepository;

        public static Iaspnet_RolesRepository RoleRepository
        {
            get
            {
                if (roleRepository == null)
                    roleRepository = RepositoryHelper.Getaspnet_RolesRepository();

                return roleRepository;
            }
            set
            {
                roleRepository = value;
            }
        }

        internal void Update()
        {
            RoleRepository.UnitOfWork.Context.Entry(this).State = System.Data.Entity.EntityState.Modified;
            RoleRepository.UnitOfWork.Commit();
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
    }
}
