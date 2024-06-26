using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.PermissionManagement;

namespace Necnat.Abp.NnMgmtAuthorization.Domains.NnPermission
{
    public interface INnPermissionGrantRepository : IRepository<PermissionGrant, Guid>
    {
        Task<List<PermissionGrant>> GetListByProviderKeyAsync(string providerKey);
        Task<List<string>> GetListProviderKeyByNameAsync(string name);
        Task<int> DeleteByProviderKeyAsync(string providerKey);
    }
}
