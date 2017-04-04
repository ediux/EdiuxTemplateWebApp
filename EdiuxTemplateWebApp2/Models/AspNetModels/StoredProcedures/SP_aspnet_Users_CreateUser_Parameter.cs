namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    	public partial class aspnet_Users_CreateUser_InputParameter
    	{
    		public virtual System.Guid applicationId { get; set; }       
    		public virtual string userName { get; set; }       
    		public virtual bool isUserAnonymous { get; set; }       
    		public virtual System.DateTime lastActivityDate { get; set; }       
    		public virtual aspnet_Users_CreateUser_OutputParameter OutputParameter { get; set; }      
    				public virtual int ReturnValue { get; set; }
    	}
}
