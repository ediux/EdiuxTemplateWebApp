using System;
using System.Collections;
using System.Collections.Generic;
// =========================================
// 產生額外資料庫回應物件
// =========================================
namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class AspNetDbEntities : IRepositoryCollection
    {
        public int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Add(IRepositoryBase item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(IRepositoryBase item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IRepositoryBase[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IRepositoryBase> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public T GetRepository<T>() where T : IRepositoryBase
        {
            throw new NotImplementedException();
        }

        public bool Remove(IRepositoryBase item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
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
