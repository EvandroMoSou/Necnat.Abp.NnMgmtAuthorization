using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace Necnat.Abp.NnMgmtAuthorization.Domains.DmHierarchicalAccess
{
    public class HierarchicalAccessStore : IHierarchicalAccessStore, ITransientDependency
    {
        protected readonly IHierarchicalAccessRepository _repository;
        protected readonly IDistributedCache<UserHierarchicalAccessCacheItem> _cache;

        public HierarchicalAccessStore(
            IHierarchicalAccessRepository repository,
            IDistributedCache<UserHierarchicalAccessCacheItem> cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public virtual async Task<List<UserHierarchicalAccess>> GetListUserHierarchicalAccessByUserIdAsync(Guid userId)
        {
            return (await GetCacheItemAsync(userId)).List;
        }

        protected virtual async Task<UserHierarchicalAccessCacheItem> GetCacheItemAsync(Guid userId)
        {
            return (await _cache.GetOrAddAsync(
                "haui:" + userId.ToString(),
                async () => await GetFromDatabaseAsync(userId),
                () => new DistributedCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddHours(8) }
            ))!;
        }

        protected virtual async Task<UserHierarchicalAccessCacheItem> GetFromDatabaseAsync(Guid userId)
        {
            var hierarchicalAccessList = await _repository.GetListByUserIdAsync(userId);
            var l = new List<UserHierarchicalAccess>();
            foreach (var iHierarchicalAccess in hierarchicalAccessList)
                l.Add(new UserHierarchicalAccess { RId = iHierarchicalAccess.RoleId, HSId = iHierarchicalAccess.HierarchicalStructureId });

            return new UserHierarchicalAccessCacheItem(l);
        }
    }
}
