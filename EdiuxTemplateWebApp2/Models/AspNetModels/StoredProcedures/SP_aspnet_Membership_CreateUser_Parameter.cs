namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_Membership_CreateUser_InputParameter
    {
        public virtual string ApplicationName { get; set; }
        public virtual string UserName { get; set; }
        public virtual string Password { get; set; }
        public virtual string PasswordSalt { get; set; }
        public virtual string Email { get; set; }
        public virtual string PasswordQuestion { get; set; }
        public virtual string PasswordAnswer { get; set; }
        public virtual bool IsApproved { get; set; }
        public virtual System.DateTime CurrentTimeUtc { get; set; }
        public virtual System.DateTime CreateDate { get; set; }
        public virtual int UniqueEmail { get; set; }
        public virtual int PasswordFormat { get; set; }
        public virtual aspnet_Membership_CreateUser_OutputParameter OutputParameter { get; set; }
        public virtual int ReturnValue { get; set; }
    }
}
