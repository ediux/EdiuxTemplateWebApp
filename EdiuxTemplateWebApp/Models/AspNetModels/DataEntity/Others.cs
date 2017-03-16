using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
/// =========================================
/// 產生額外資料庫回應物件
/// =========================================
namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class AspNetDbEntities2 : IRepositoryCollection
    {
        ICollection<IRepositoryBase> internalRepository = new Collection<IRepositoryBase>();
        
        public int Count
        {
            get
            {

                return internalRepository.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(IRepositoryBase item)
        {
            internalRepository.Add(item);
        }

        public void Clear()
        {
            internalRepository.Clear();
        }

        public bool Contains(IRepositoryBase item)
        {
            return internalRepository.Contains(item);
        }

        public void CopyTo(IRepositoryBase[] array, int arrayIndex)
        {
            internalRepository.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IRepositoryBase> GetEnumerator()
        {
            return (IEnumerator<IRepositoryBase>)internalRepository.GetEnumerator();
        }

        public T GetRepository<T>() where T : IRepositoryBase
        {
            T repository = default(T);
            var items = internalRepository.GetEnumerator();
            do
            {
                
                if (items.Current.GetType() == typeof(T))
                {
                    repository = (T)items.Current;
                    break;
                }

            } while (internalRepository.GetEnumerator().MoveNext());
            return repository;
        }

        public bool Remove(IRepositoryBase item)
        {
            return internalRepository.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return internalRepository.GetEnumerator();
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
}
