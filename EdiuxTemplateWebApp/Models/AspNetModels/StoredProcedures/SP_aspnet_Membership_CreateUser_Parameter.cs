namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    	public partial class aspnet_Membership_CreateUser_InputParameter
    	{
    		public virtual string applicationName { get; set; }       
    		public virtual string userName { get; set; }       
    		public virtual string password { get; set; }       
    		public virtual string passwordSalt { get; set; }       
    		public virtual string email { get; set; }       
    		public virtual string passwordQuestion { get; set; }       
    		public virtual string passwordAnswer { get; set; }       
    		public virtual bool isApproved { get; set; }       
    		public virtual System.DateTime currentTimeUtc { get; set; }       
    		public virtual System.DateTime createDate { get; set; }       
    		public virtual int uniqueEmail { get; set; }       
    		public virtual int passwordFormat { get; set; }       
    		public virtual aspnet_Membership_CreateUser_OutputParameter OutputParameter { get; set; }      
    				public virtual int ReturnValue { get; set; }
    	}
}
