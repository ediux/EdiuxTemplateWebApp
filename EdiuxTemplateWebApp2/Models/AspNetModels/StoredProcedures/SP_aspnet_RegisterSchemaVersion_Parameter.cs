namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    	public partial class aspnet_RegisterSchemaVersion_InputParameter
    	{
    		public virtual string feature { get; set; }       
    		public virtual string compatibleSchemaVersion { get; set; }       
    		public virtual bool isCurrentVersion { get; set; }       
    		public virtual bool removeIncompatibleSchema { get; set; }       
    		public virtual int ReturnValue { get; set; }
    	}
}
