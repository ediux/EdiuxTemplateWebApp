﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_PathsRepository : EFRepository<aspnet_Paths>, Iaspnet_PathsRepository
    {
        public void Update(aspnet_Paths entity)
        {
            var foundPath = Get(entity.PathId, entity.Path, entity.ApplicationId);
            foundPath = CopyTo<aspnet_Paths>(entity);
            UnitOfWork.Commit();
        }

        public override aspnet_Paths Reload(aspnet_Paths entity)
        {
            entity = ObjectSet.Attach(entity);
            return base.Reload(entity);
        }
    }

    public partial interface Iaspnet_PathsRepository : IRepositoryBase<aspnet_Paths>
    {
        void Update(aspnet_Paths entity);
    }
}
