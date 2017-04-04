namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    	public partial class aspnet_PersonalizationAdministration_ResetUserState_InputParameter
    	{
    		public virtual string applicationName { get; set; }       
    		public virtual System.DateTime inactiveSinceDate { get; set; }       
    		public virtual string userName { get; set; }       
    		public virtual string path { get; set; }       
    		public virtual aspnet_PersonalizationAdministration_ResetUserState_OutputParameter OutputParameter { get; set; }      
    				public virtual int ReturnValue { get; set; }
    	}
}
