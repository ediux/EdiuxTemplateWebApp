using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EdiuxTemplateCoreApp
{
	public class aspnet_Applications
	{
		public aspnet_Applications()
		{
		}

		public string ApplicationName { get; set; }
		public string LoweredApplicationName { get; set; }
		[Key]
		public System.Guid ApplicationId { get; set; }
		public string Description { get; set; }


		public virtual ICollection<aspnet_Membership> aspnet_Membership { get; set; }

		public virtual ICollection<aspnet_Paths> aspnet_Paths { get; set; }
	
		public virtual ICollection<aspnet_Roles> aspnet_Roles { get; set; }

		public virtual ICollection<aspnet_Users> aspnet_Users { get; set; }

		public virtual ICollection<aspnet_VoidUsers> aspnet_VoidUsers { get; set; }

		public virtual ICollection<Menus> Menus { get; set; }
	}
}
