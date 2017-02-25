namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    	public partial class aspnet_Membership_UpdateUserInfo_InputParameter
    	{
    			public virtual string applicationName { get; set; }       
    			public virtual string userName { get; set; }       
    			public virtual bool isPasswordCorrect { get; set; }       
    			public virtual bool updateLastLoginActivityDate { get; set; }       
    			public virtual int maxInvalidPasswordAttempts { get; set; }       
    			public virtual int passwordAttemptWindow { get; set; }       
    			public virtual System.DateTime currentTimeUtc { get; set; }       
    			public virtual System.DateTime lastLoginDate { get; set; }       
    			public virtual System.DateTime lastActivityDate { get; set; }       
    	}
}
