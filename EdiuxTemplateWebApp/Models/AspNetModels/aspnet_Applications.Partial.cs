namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Runtime.Caching;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity;

    [MetadataType(typeof(aspnet_ApplicationsMetaData))]
    public partial class aspnet_Applications
    {
        private static Iaspnet_ApplicationsRepository applicationRepository;
        public static Iaspnet_ApplicationsRepository ApplicationRepository
        {
            get
            {
                if (applicationRepository == null)
                    return RepositoryHelper.Getaspnet_ApplicationsRepository();

                return applicationRepository;
            }
            set
            {
                applicationRepository = value;
            }
        }

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


        private Iaspnet_UsersRepository checkAndGetUserRepository()
        {
            Iaspnet_UsersRepository userRepo = RepositoryHelper.Getaspnet_UsersRepository(ApplicationRepository.UnitOfWork);
            return userRepo;
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
