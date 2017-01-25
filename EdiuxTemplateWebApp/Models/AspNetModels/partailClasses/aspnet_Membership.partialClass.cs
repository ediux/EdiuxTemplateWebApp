using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_Membership : ICloneable
    {
        private static Iaspnet_MembershipRepository membershipRepository;
        public static Iaspnet_MembershipRepository MembershipRepository
        {
            get
            {
                if (membershipRepository == null)
                    membershipRepository = RepositoryHelper.Getaspnet_MembershipRepository();

                return membershipRepository;
            }
            set
            {
                membershipRepository = value;
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

        public string GetEmail()
        {
            return Email;
        }

        public bool GetEmailConfirmed()
        {
            return IsApproved;
        }

        public void SetEmail(string email)
        {
            Email = email;
            LoweredEmail = email.ToLowerInvariant();

            if (MembershipRepository == null)
                MembershipRepository = RepositoryHelper.Getaspnet_MembershipRepository();

            MembershipRepository.UnitOfWork.Context.Entry(this).State = System.Data.Entity.EntityState.Modified;
            MembershipRepository.UnitOfWork.Commit();
        }
    }
}