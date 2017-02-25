namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    	public partial class aspnet_Profile_GetProfiles_InputParameter
    	{
    			public virtual string applicationName { get; set; }       
    			public virtual int profileAuthOptions { get; set; }       
    			public virtual int pageIndex { get; set; }       
    			public virtual int pageSize { get; set; }       
    			public virtual string userNameToMatch { get; set; }       
    			public virtual System.DateTime inactiveSinceDate { get; set; }       
    	}
}
