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
    public class EfCoreHierarchyComponentGroupRepository : EfCoreRepository<INnMgmtAuthorizationDbContext, HierarchyComponentGroup, Guid>, IHierarchyComponentGroupRepository
    {
        public EfCoreHierarchyComponentGroupRepository(IDbContextProvider<INnMgmtAuthorizationDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
    }
}
