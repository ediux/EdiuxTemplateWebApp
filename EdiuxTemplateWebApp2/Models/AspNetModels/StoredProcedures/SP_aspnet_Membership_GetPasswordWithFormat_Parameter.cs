﻿namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    	public partial class aspnet_Membership_GetPasswordWithFormat_InputParameter
    	{
    		public virtual string applicationName { get; set; }       
    		public virtual string userName { get; set; }       
    		public virtual bool updateLastLoginActivityDate { get; set; }       
    		public virtual System.DateTime currentTimeUtc { get; set; }       
    		public virtual aspnet_Membership_GetPasswordWithFormat_OutputParameter OutputParameter { get; set; }      
    				public virtual int ReturnValue { get; set; }
    	}
}
