//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
	using System;
	using System.Collections.Generic;

	public partial class aspnet_UserLogin
	{
		public System.Guid UserId { get; set; }
		[Key]
		[Column("LoginProvider",Order = 0)]
		public string LoginProvider { get; set; }
		[Key]
		[Column("ProviderKey",Order = 1)]
		public string ProviderKey { get; set; }

		[ForeignKey("UserId")]
		public virtual aspnet_Users aspnet_Users { get; set; }
	}
}