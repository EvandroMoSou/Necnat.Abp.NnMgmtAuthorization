using Microsoft.Extensions.Caching.Distributed;
using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Caching;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class HierarchicalStructureStore : IHierarchicalStructureStore
    {
        readonly IDistributedCache<HierarchicalStructureRecursiveCacheItem> _hierarchicalStructureRecursiveCache;
        readonly IHierarchicalStructureRepository _hierarchicalStructureRepository;

        public HierarchicalStructureStore(
            IDistributedCache<HierarchicalStructureRecursiveCacheItem> hierarchicalStructureRecursiveCache,
            IHierarchicalStructureRepository hierarchicalStructureRepository)
        {
            _hierarchicalStructureRecursiveCache = hierarchicalStructureRecursiveCache;
            _hierarchicalStructureRepository = hierarchicalStructureRepository;
        }

        public virtual async Task<bool> HasHierarchyComponentIdAsync(Guid id, Guid hierarchyComponentId)
        {
            return (await GetListHierarchyComponentIdRecursiveAsync(id)).Contains(hierarchyComponentId);
        }

        public virtual async Task<List<Guid>> GetListHierarchyComponentIdRecursiveAsync(Guid id)
        {
            return (await GetCacheItemAsync(id)).HS.LHCId;
        }

        protected virtual async Task<HierarchicalStructureRecursiveCacheItem> GetCacheItemAsync(Guid hierarchicalStructureId)
        {
            return (await _hierarchicalStructureRecursiveCache.GetOrAddAsync(
                hierarchicalStructureId.ToString(), //Cache key
                async () => await GetDataAsync(hierarchicalStructureId),
                () => new DistributedCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddHours(1) }
            ))!;
        }

        protected virtual async Task<HierarchicalStructureRecursiveCacheItem> GetDataAsync(Guid hierarchicalStructureId)
        {
            var hierarchicalStructure = await _hierarchicalStructureRepository.GetAsync(hierarchicalStructureId);
            var lHierarchicalStructure = await _hierarchicalStructureRepository.GetListAsync(x => x.HierarchyId == hierarchicalStructure.HierarchyId);

            var hs = new HS { Id = hierarchicalStructureId };
            hs.LHCId = GetRecursive(lHierarchicalStructure, lHierarchicalStructure.Where(x => x.Id == hierarchicalStructureId).First());

            return new HierarchicalStructureRecursiveCacheItem { HS = hs };
        }

        protected virtual List<Guid> GetRecursive(List<HierarchicalStructure> lHierarchicalStructure, HierarchicalStructure hierarchicalStructure)
        {
            var l = new List<Guid> { hierarchicalStructure.HierarchyComponentId };

            var lChild = lHierarchicalStructure.Where(x => x.HierarchicalStructureIdParent == hierarchicalStructure.Id).ToList();
            foreach (var iChild in lChild)
                l.AddRange(GetRecursive(lHierarchicalStructure, iChild));

            return l;
        }
    }
}
