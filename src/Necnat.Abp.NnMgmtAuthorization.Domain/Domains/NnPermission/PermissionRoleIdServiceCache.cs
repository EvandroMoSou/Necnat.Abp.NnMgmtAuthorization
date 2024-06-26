using Microsoft.Extensions.Caching.Distributed;
using Necnat.Abp.NnLibCommon.Domains.NnIdentity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace Necnat.Abp.NnMgmtAuthorization.Domains.NnPermission
{
    public class PermissionRoleIdServiceCache : IPermissionRoleIdService, ITransientDependency
    {
        readonly INnPermissionGrantRepository _necnatPermissionGrantRepository;
        readonly INnIdentityRoleRepository _nnIdentityRoleRepository;
        readonly IDistributedCache<PermissionRoleIdCacheItem> _permissionRoleIdCache;

        public PermissionRoleIdServiceCache(
            INnPermissionGrantRepository necnatPermissionGrantRepository,
            INnIdentityRoleRepository nnIdentityRoleRepository,
            IDistributedCache<PermissionRoleIdCacheItem> permissionRoleIdCache)
        {
            _permissionRoleIdCache = permissionRoleIdCache;
            _nnIdentityRoleRepository = nnIdentityRoleRepository;
            _necnatPermissionGrantRepository = necnatPermissionGrantRepository;
        }

        public async Task<List<Guid>> SearchRoleIdAsync(string permissionName)
        {
            return (await GetCacheItemAsync(permissionName)).RoleIdList;
        }

        public async Task<PermissionRoleIdCacheItem> GetCacheItemAsync(string permissionName)
        {
            return (await _permissionRoleIdCache.GetOrAddAsync(
                permissionName, //Cache key
                async () => await GetFromDatabaseAsync(permissionName),
                () => new DistributedCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddHours(1) }
            ))!;
        }

        private async Task<PermissionRoleIdCacheItem> GetFromDatabaseAsync(string permissionName)
        {
            var lRoleName = await _necnatPermissionGrantRepository.GetListProviderKeyByNameAsync(permissionName);
            var lRoleId = await _nnIdentityRoleRepository.GetListIdByNameAsync(lRoleName);
            return new PermissionRoleIdCacheItem { RoleIdList = lRoleId };
        }
    }
}
