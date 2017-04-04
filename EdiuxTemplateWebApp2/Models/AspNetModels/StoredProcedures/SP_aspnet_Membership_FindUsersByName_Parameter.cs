namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    	public partial class aspnet_Membership_FindUsersByName_InputParameter
    	{
    		public virtual string ApplicationName { get; set; }       
    		public virtual string UserNameToMatch { get; set; }       
    		public virtual int PageIndex { get; set; }       
    		public virtual int PageSize { get; set; }       
    		public virtual int ReturnValue { get; set; }
    	}
}
