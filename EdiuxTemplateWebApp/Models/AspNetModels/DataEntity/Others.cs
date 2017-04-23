using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
// =========================================
// 產生額外資料庫回應物件
// =========================================
namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class AspNetDbEntities : IRepositoryCollection
    {
        private Collection<IRepositoryBase> innerSet = new Collection<IRepositoryBase>();

        public int Count => innerSet.Count;

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(IRepositoryBase item)
        {
            innerSet.Add(item);
        }

        public void Clear()
        {
            innerSet.Clear();
        }

        public bool Contains(IRepositoryBase item)
        {
            return innerSet.Contains(item);
        }

        public void CopyTo(IRepositoryBase[] array, int arrayIndex)
        {
            innerSet.CopyTo(array, arrayIndex);
        }


        public IEnumerator<IRepositoryBase> GetEnumerator()
        {
            return innerSet.GetEnumerator();
        }

        public T GetRepository<T>() where T : IRepositoryBase
        {
            return (T)innerSet.Where(w => typeof(T) == w.GetType()).SingleOrDefault();
        }

        public bool Remove(IRepositoryBase item)
        {
            return innerSet.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return innerSet.GetEnumerator();
        }
    }
    public partial class aspnet_Membership_FindUsersByEmail_Result
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordQuestion { get; set; }
        public string Comment { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public DateTime LastPasswordChangedDate { get; set; }
        public Guid UserId { get; set; }
        public DateTime LastLockoutDate { get; set; }
    }

    public partial class aspnet_Membership_FindUsersByName_Result : aspnet_Membership_FindUsersByEmail_Result
    {

    }

    /// <summary>
    /// Aspnet membership get all users result.
    /// </summary>
    public partial class aspnet_Membership_GetAllUsers_Result
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordQuestion { get; set; }
        public string Comment { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime LastActivityDate { get; set; }
        public DateTime LastPasswordChangedDate { get; set; }
        public Guid UserId { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime LastLockoutDate { get; set; }
    }


}
