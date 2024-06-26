using Necnat.Abp.NnMgmtAuthorization.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Necnat.Abp.NnMgmtAuthorization.Domains.DmHierarchyComponentGroup
{
    public class EfCoreHierarchyComponentGroupRepository : EfCoreRepository<NnMgmtAuthorizationDbContext, HierarchyComponentGroup, Guid>, IHierarchyComponentGroupRepository
    {
        public EfCoreHierarchyComponentGroupRepository(IDbContextProvider<NnMgmtAuthorizationDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
    }
}
