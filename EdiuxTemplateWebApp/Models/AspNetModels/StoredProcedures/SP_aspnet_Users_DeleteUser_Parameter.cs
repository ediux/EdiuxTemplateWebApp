namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    	public partial class aspnet_Users_DeleteUser_InputParameter
    	{
    		public virtual string applicationName { get; set; }       
    		public virtual string userName { get; set; }       
    		public virtual int tablesToDeleteFrom { get; set; }       
    		public virtual aspnet_Users_DeleteUser_OutputParameter OutputParameter { get; set; }      
    				public virtual int ReturnValue { get; set; }
    	}
}
