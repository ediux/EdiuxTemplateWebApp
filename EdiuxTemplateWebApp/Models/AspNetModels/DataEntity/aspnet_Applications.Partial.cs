namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    using System;
    using System.Collections.Generic;
    
    using System.ComponentModel.DataAnnotations;
    
    [MetadataType(typeof(aspnet_ApplicationsMetaData))]
    public partial class aspnet_Applications : ICloneable
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

        public object Clone()
        {
            aspnet_Applications copy = new aspnet_Applications();

            Type sourceType = this.GetType();
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

            //            copy.aspnet_Membership = new Collection<aspnet_Membership>();

            foreach (aspnet_Membership member in aspnet_Membership)
            {
                try
                {
                    aspnet_Membership cloneObject = member.Clone() as aspnet_Membership;
                    copy.aspnet_Membership.Add(cloneObject);
                }
                catch (Exception ex)
                {
                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                    continue;
                }
            }

            foreach (var items_paths in aspnet_Paths)
            {
                try
                {
                    aspnet_Paths cloneObject = items_paths.Clone() as aspnet_Paths;
                    copy.aspnet_Paths.Add(cloneObject);
                }
                catch (Exception ex)
                {
                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                    continue;
                }
            }

            foreach (var items_Role in aspnet_Roles)
            {
                try
                {
                    aspnet_Roles cloneObject = items_Role.Clone() as aspnet_Roles;
                    copy.aspnet_Roles.Add(cloneObject);
                }
                catch (Exception ex)
                {
                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                    continue;
                }
            }

            foreach (var items_Users in aspnet_Users)
            {
                try
                {
                    aspnet_Users cloneObject = items_Users.Clone() as aspnet_Users;
                    copy.aspnet_Users.Add(cloneObject);
                }
                catch (Exception ex)
                {
                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                    continue;
                }
            }

            foreach (var items_voidUsers in aspnet_VoidUsers)
            {
                try
                {
                    aspnet_VoidUsers cloneObject = items_voidUsers.Clone() as aspnet_VoidUsers;
                    copy.aspnet_VoidUsers.Add(cloneObject);
                }
                catch (Exception ex)
                {
                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                    continue;
                }
            }

            foreach (var item_Menu in Menus)
            {
                try
                {
                    Menus cloneObject = item_Menu.Clone() as Menus;
                    copy.Menus.Add(cloneObject);

                }
                catch (Exception ex)
                {
                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                    continue;
                }
            }

            return copy;
        }

        private Iaspnet_UsersRepository checkAndGetUserRepository()
        {
            Iaspnet_UsersRepository userRepo = RepositoryHelper.Getaspnet_UsersRepository();
            return userRepo;
        }
    }
    
    public partial class aspnet_ApplicationsMetaData
    {
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        [Required]
        public string ApplicationName { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        [Required]
        public string LoweredApplicationName { get; set; }
        [Required]
        public System.Guid ApplicationId { get; set; }
        
        [StringLength(256, ErrorMessage="欄位長度不得大於 256 個字元")]
        public string Description { get; set; }
        public virtual ICollection<aspnet_Membership> aspnet_Membership { get; set; }
        public virtual ICollection<aspnet_Paths> aspnet_Paths { get; set; }
        public virtual ICollection<aspnet_Roles> aspnet_Roles { get; set; }
        public virtual ICollection<aspnet_Users> aspnet_Users { get; set; }
        public virtual ICollection<aspnet_VoidUsers> aspnet_VoidUsers { get; set; }
        public virtual ICollection<Menus> Menus { get; set; }
    }
}
