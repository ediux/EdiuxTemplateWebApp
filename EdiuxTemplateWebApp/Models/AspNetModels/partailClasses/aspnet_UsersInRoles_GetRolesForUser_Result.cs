using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    [MetadataType(typeof(aspnet_UsersInRoles_GetRolesForUser_ResultMetaData))]
    public partial class aspnet_UsersInRoles_GetRolesForUser_Result
    {
        public string RoleName { get; set; }
    }

    public partial class aspnet_UsersInRoles_GetRolesForUser_ResultMetaData
    {
        [DisplayName("角色名稱")]
        public string RoleName { get; set; }
    }
}