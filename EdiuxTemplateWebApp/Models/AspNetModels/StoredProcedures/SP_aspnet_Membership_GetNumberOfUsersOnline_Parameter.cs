namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_Membership_GetNumberOfUsersOnline_InputParameter
    {
        public virtual string ApplicationName { get; set; }
        public virtual int MinutesSinceLastInActive { get; set; }
        public virtual System.DateTime CurrentTimeUtc { get; set; }
        public virtual aspnet_Membership_GetNumberOfUsersOnline_OutputParameter OutputParameter { get; set; }
        public virtual int ReturnValue { get; set; }
    }
}
