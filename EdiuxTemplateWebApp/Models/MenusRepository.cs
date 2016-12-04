using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity;

namespace EdiuxTemplateWebApp.Models
{
    public partial class MenusRepository : EFRepository<Menus>, IMenusRepository
    {
        public override IQueryable<Menus> All()
        {

            var result = base.All().Include(m => m.ChildMenus).Include(m => m.System_ControllerActions)
                 .Where(w => w.Void == false)
                 .AsQueryable();

            if (result.Count() > 0)
                return result;

            result.Load();

            return result;
        }
    }

    public partial interface IMenusRepository : IRepositoryBase<Menus>
    {

    }
}