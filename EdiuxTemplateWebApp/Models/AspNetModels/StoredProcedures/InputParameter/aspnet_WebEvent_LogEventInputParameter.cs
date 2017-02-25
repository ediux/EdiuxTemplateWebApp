namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    	public partial class aspnet_WebEvent_LogEvent_InputParameter
    	{
    			public virtual string eventId { get; set; }       
    			public virtual System.DateTime eventTimeUtc { get; set; }       
    			public virtual System.DateTime eventTime { get; set; }       
    			public virtual string eventType { get; set; }       
    			public virtual decimal eventSequence { get; set; }       
    			public virtual decimal eventOccurrence { get; set; }       
    			public virtual int eventCode { get; set; }       
    			public virtual int eventDetailCode { get; set; }       
    			public virtual string message { get; set; }       
    			public virtual string applicationPath { get; set; }       
    			public virtual string applicationVirtualPath { get; set; }       
    			public virtual string machineName { get; set; }       
    			public virtual string requestUrl { get; set; }       
    			public virtual string exceptionType { get; set; }       
    			public virtual string details { get; set; }       
    	}
}
