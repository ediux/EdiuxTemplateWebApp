namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    	public partial class aspnet_Membership_UpdateUser_InputParameter
    	{
    		public virtual string applicationName { get; set; }       
    		public virtual string userName { get; set; }       
    		public virtual string email { get; set; }       
    		public virtual string comment { get; set; }       
    		public virtual bool isApproved { get; set; }       
    		public virtual System.DateTime lastLoginDate { get; set; }       
    		public virtual System.DateTime lastActivityDate { get; set; }       
    		public virtual int uniqueEmail { get; set; }       
    		public virtual System.DateTime currentTimeUtc { get; set; }       
    		public virtual int ReturnValue { get; set; }
    	}
}
