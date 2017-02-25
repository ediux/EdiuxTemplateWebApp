namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    	public partial class aspnet_Profile_SetProperties_InputParameter
    	{
    			public virtual string applicationName { get; set; }       
    			public virtual string propertyNames { get; set; }       
    			public virtual string propertyValuesString { get; set; }       
    			public virtual byte[] propertyValuesBinary { get; set; }       
    			public virtual string userName { get; set; }       
    			public virtual bool isUserAnonymous { get; set; }       
    			public virtual System.DateTime currentTimeUtc { get; set; }       
    	}
}
