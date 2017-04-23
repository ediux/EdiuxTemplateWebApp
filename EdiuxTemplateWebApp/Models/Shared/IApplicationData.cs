namespace EdiuxTemplateWebApp.Models
{
    public interface IApplicationData<out TKey> 
    {
        TKey ApplicationId { get;  }

        string ApplicationName { get; set; }
    }
}
