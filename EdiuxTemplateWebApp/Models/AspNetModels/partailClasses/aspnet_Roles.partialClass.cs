using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_Roles : IRole<Guid>, ICloneable
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

        internal void Update()
        {
            RoleRepository.UnitOfWork.Context.Entry(this).State = System.Data.Entity.EntityState.Modified;
            RoleRepository.UnitOfWork.Commit();
        }
    }
}