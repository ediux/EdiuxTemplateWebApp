namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    	public partial class aspnet_Profile_GetNumberOfInactiveProfiles_InputParameter
    	{
    		public virtual string applicationName { get; set; }       
    		public virtual int profileAuthOptions { get; set; }       
    		public virtual System.DateTime inactiveSinceDate { get; set; }       
    		public virtual int ReturnValue { get; set; }
    	}
}
