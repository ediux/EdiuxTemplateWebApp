namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_Applications_CreateApplication_InputParameter
    {
        public virtual string ApplicationName { get; set; }
        public virtual aspnet_Applications_CreateApplication_OutputParameter OutputParameter { get; set; }
        public virtual int ReturnValue { get; set; }
    }
}
