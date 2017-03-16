namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_Users_DeleteUser_InputParameter
    {
        public virtual string ApplicationName { get; set; }
        public virtual string UserName { get; set; }
        public virtual int TablesToDeleteFrom { get; set; }
        public virtual aspnet_Users_DeleteUser_OutputParameter OutputParameter { get; set; }
        public virtual int ReturnValue { get; set; }
    }
}
