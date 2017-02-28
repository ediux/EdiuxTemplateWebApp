namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    	public partial class aspnet_Paths_CreatePath_InputParameter
    	{
    		public virtual System.Guid applicationId { get; set; }       
    		public virtual string path { get; set; }       
    		public virtual aspnet_Paths_CreatePath_OutputParameter OutputParameter { get; set; }      
    				public virtual int ReturnValue { get; set; }
    	}
}
