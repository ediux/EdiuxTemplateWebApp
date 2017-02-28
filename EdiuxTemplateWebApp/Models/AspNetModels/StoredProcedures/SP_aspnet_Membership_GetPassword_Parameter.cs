namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    	public partial class aspnet_Membership_GetPassword_InputParameter
    	{
    		public virtual string applicationName { get; set; }       
    		public virtual string userName { get; set; }       
    		public virtual int maxInvalidPasswordAttempts { get; set; }       
    		public virtual int passwordAttemptWindow { get; set; }       
    		public virtual System.DateTime currentTimeUtc { get; set; }       
    		public virtual string passwordAnswer { get; set; }       
    		public virtual int ReturnValue { get; set; }
    	}
}
