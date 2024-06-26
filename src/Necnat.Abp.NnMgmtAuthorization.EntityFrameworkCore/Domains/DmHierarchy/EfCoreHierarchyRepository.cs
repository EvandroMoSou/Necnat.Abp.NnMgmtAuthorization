using Microsoft.EntityFrameworkCore;
using Necnat.Abp.NnMgmtAuthorization.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Necnat.Abp.NnMgmtAuthorization.Domains.DmHierarchy
{
    public class EfCoreHierarchyRepository : EfCoreRepository<NnMgmtAuthorizationDbContext, Hierarchy, Guid>, IHierarchyRepository
    {
        public EfCoreHierarchyRepository(IDbContextProvider<NnMgmtAuthorizationDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public virtual async Task<Hierarchy?> GetByNameAsync(string name)
        {
            return await (await GetDbSetAsync()).Where(x => x.Name == name).FirstOrDefaultAsync();
        }
    }
}
