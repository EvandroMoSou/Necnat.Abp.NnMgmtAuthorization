using Microsoft.EntityFrameworkCore;
using Necnat.Abp.NnMgmtAuthorization.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.PermissionManagement;

namespace Necnat.Abp.NnMgmtAuthorization.Domains.NnPermission
{
    public class EfCoreNnPermissionGrantRepository : EfCoreRepository<INnMgmtAuthorizationDbContext, PermissionGrant, Guid>, INnPermissionGrantRepository
    {
        public EfCoreNnPermissionGrantRepository(IDbContextProvider<INnMgmtAuthorizationDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<List<PermissionGrant>> GetListByProviderKeyAsync(string providerKey)
        {
            return await (await GetDbSetAsync()).Where(x => x.ProviderKey == providerKey).ToListAsync();
        }

        public async Task<List<string>> GetListProviderKeyByNameAsync(string name)
        {
            return await (await GetDbSetAsync()).Where(x => x.ProviderName == "R" && x.Name == name).Select(x => x.ProviderKey).ToListAsync();
        }

        public async Task<int> DeleteByProviderKeyAsync(string providerKey)
        {
            return await (await GetDbSetAsync()).Where(x => x.ProviderKey == providerKey).ExecuteDeleteAsync();
        }
    }
}
