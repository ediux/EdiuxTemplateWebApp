using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Security;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
	public class aspnet_MembershipRepository : EFRepository<aspnet_Membership>, Iaspnet_MembershipRepository
	{
		public override IQueryable<aspnet_Membership> All()
		{
			UnitOfWork.LazyLoadingEnabled = false;

			IQueryable<aspnet_Membership> loadAllQueryable = (from m in ObjectSet
															  join app in UnitOfWork.Set<aspnet_Applications>() on m.ApplicationId equals app.ApplicationId
															  select m).AsQueryable();

			return loadAllQueryable;
		}

		public override aspnet_Membership Add(aspnet_Membership entity)
		{
			aspnet_Membership_CreateUser_InputParameter paramObject = new aspnet_Membership_CreateUser_InputParameter();
			paramObject.ApplicationName = entity.aspnet_Applications.ApplicationName;

			paramObject.CurrentTimeUtc = DateTime.UtcNow;
			paramObject.CreateDate = paramObject.CurrentTimeUtc.Date;
			paramObject.Email = entity.Email;
			paramObject.IsApproved = entity.IsApproved;
			paramObject.Password = entity.Password;
			paramObject.PasswordAnswer = entity.PasswordAnswer;
			paramObject.PasswordFormat = entity.PasswordFormat;
			paramObject.PasswordQuestion = entity.PasswordQuestion;
			paramObject.PasswordSalt = entity.PasswordSalt;
			paramObject.UniqueEmail = Membership.Provider.RequiresUniqueEmail ? 1 : 0;
			paramObject.UserName = user.UserName;

			UnitOfWork.Repositories.GetRepository.aspnet_Membership_CreateUser(paramObject);

			if (paramObject.ReturnValue == 0)
			{
				if (paramObject.OutputParameter.CreateStatus == (int)MembershipCreateStatus.Success)
				{
					UnitOfWork.Set<aspnet_Users>().Attach(userRepo.Get(paramObject.OutputParameter.UserId));
				}
			}
		}

		public IEnumerator<aspnet_Membership_FindUsersByEmail_Result> FindByEmail(string applicationName, string eMailToMatch, int pageIndex, int pageSize)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<aspnet_Membership_FindUsersByEmail_Result> FindByName(string applicationName, string userNameToMatch, int pageIndex, int pageSize)
		{
			throw new NotImplementedException();
		}
	}

	public partial interface Iaspnet_MembershipRepository : IRepositoryBase<aspnet_Membership>
	{
		/// <summary>
		/// Finds the by email.
		/// </summary>
		/// <returns>The by email.</returns>
		/// <param name="applicationName">Application name.</param>
		/// <param name="eMailToMatch">E mail to match.</param>
		/// <param name="pageIndex">Page index.</param>
		/// <param name="pageSize">Page size.</param>
		IEnumerator<aspnet_Membership_FindUsersByEmail_Result> FindByEmail(string applicationName, string eMailToMatch, int pageIndex, int pageSize);

		/// <summary>
		/// Finds the name of the by.
		/// </summary>
		/// <returns>The by name.</returns>
		/// <param name="applicationName">Application name.</param>
		/// <param name="userNameToMatch">User name to match.</param>
		/// <param name="pageIndex">Page index.</param>
		/// <param name="pageSize">Page size.</param>
		IEnumerator<aspnet_Membership_FindUsersByEmail_Result> FindByName(string applicationName, string userNameToMatch, int pageIndex, int pageSize);


	}
}
