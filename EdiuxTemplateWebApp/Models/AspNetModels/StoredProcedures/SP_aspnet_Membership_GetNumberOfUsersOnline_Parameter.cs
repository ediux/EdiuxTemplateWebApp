namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    	public partial class aspnet_Membership_GetNumberOfUsersOnline_InputParameter
    	{
    		public virtual string applicationName { get; set; }       
    		public virtual int minutesSinceLastInActive { get; set; }       
    		public virtual System.DateTime currentTimeUtc { get; set; }       
    		public virtual aspnet_Membership_GetNumberOfUsersOnline_OutputParameter OutputParameter { get; set; }      
    				public virtual int ReturnValue { get; set; }
    	}
}
