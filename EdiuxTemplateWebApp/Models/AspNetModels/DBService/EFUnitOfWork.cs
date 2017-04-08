﻿using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;


namespace EdiuxTemplateWebApp.Models.AspNetModels
{

	public partial class EFUnitOfWork : IUnitOfWork
	{
		private AspNetDbEntities _databaseObject;

        public EFUnitOfWork()
        {
            _databaseObject = new AspNetDbEntities();
        }

		public IDbConnection Connection
		{
			get
			{
				return _databaseObject.Database.Connection;
			}
		}

		public string ConnectionString
		{
			get
			{
				return _databaseObject.Database.Connection.ConnectionString;
			}

			set
			{
                _databaseObject.Database.Connection.ConnectionString = value;
			}
		}

		public IObjectContextAdapter Context
		{
			get
			{
				return _databaseObject;
			}

		}

		public bool LazyLoadingEnabled
		{
			get
			{
				return _databaseObject.Configuration.LazyLoadingEnabled;
			}

			set
			{
                _databaseObject.Configuration.LazyLoadingEnabled = value;
			}
		}

		public bool ProxyCreationEnabled
		{
			get
			{
				return _databaseObject.Configuration.ProxyCreationEnabled;
			}

			set
			{
                _databaseObject.Configuration.ProxyCreationEnabled = value;
			}
		}



		public IRepositoryCollection Repositories
		{
			get
			{
				return _databaseObject;
			}

			set
			{
				var repositories = value;
			}
		}

		public void Commit()
		{
			if (!transcationMode)
                _databaseObject.SaveChanges();
		}

		public virtual Task CommitAsync()
		{
			if (!transcationMode)
				return _databaseObject.SaveChangesAsync();
			else
				return Task.CompletedTask;
		}

		public T GetTypedContext<T>() where T : IObjectContextAdapter
		{
			return (T)Context;
		}

        public DbEntityEntry Entry(object entity)
        {
            return _databaseObject.Entry(entity);
        }

        public DbEntityEntry<T> Entry<T>(T entity) where T : class
        {
           return _databaseObject.Entry<T>(entity);
        }

        public DbSet Set(Type entityType)
        {
            return _databaseObject.Set(entityType);
        }

        public DbSet<T> Set<T>() where T : class
        {
            return _databaseObject.Set<T>();
        }

        public T GetRepository<T>() where T : IRepositoryBase
        {
            return _databaseObject.GetRepository<T>();
        }

        public void Add(IRepositoryBase item)
        {
            _databaseObject.Add(item);
        }

        public void Clear()
        {
            _databaseObject.Clear();
        }

        public bool Contains(IRepositoryBase item)
        {
            return _databaseObject.Contains(item);
        }

        public void CopyTo(IRepositoryBase[] array, int arrayIndex)
        {
            _databaseObject.CopyTo(array, arrayIndex);
        }

        public bool Remove(IRepositoryBase item)
        {
            return _databaseObject.Remove(item);
        }


        public void Dispose()
        {
            _databaseObject.Dispose();
        }

        bool transcationMode = false;

		/// <summary>
		/// 取得或設定目前是否處於交易模式。
		/// <see cref="T:EdiuxTemplateWebApp.Models.AspNetModels.aspnet_MembershipRepository"/> transcation mode.
		/// </summary>
		/// <value>值如果為 <c>true</c> 則處於交易模式，不會呼叫Commit()方法; 假如為 <c>false</c> 會直接呼叫 Commit()。</value>
		public bool TranscationMode
		{
			get
			{
				return transcationMode;
			}

			set
			{
				transcationMode = value;
			}
		}

        public int Count
        {
            get
            {
                return _databaseObject.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return _databaseObject.IsReadOnly;
            }
        }
    }

}
