using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EdiuxTemplateWebApp.Models
{
    namespace ApplicationLevelModels
    {
        //public partial class ApplicationUser 
        //{
        //    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager)
        //    {
        //        // 注意 authenticationType 必須符合 CookieAuthenticationOptions.AuthenticationType 中定義的項目
        //        var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
        //        // 在這裡新增自訂使用者宣告
        //        if (ApplicationUserClaim.Count > 0)
        //        {
        //            foreach (var item in ApplicationUserClaim)
        //            {
        //                userIdentity.AddClaim(new Claim(item.ClaimType, item.ClaimValue));
        //            }
        //        }
        //        return userIdentity;
        //    }
        //}
    }
    // 您可以在 ApplicationUser 類別新增更多屬性，為使用者新增設定檔資料，請造訪 http://go.microsoft.com/fwlink/?LinkID=317594 以深入了解。

}