using Microsoft.Extensions.Caching.Distributed;
using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class HierarchicalStructureStore : IHierarchicalStructureStore, ITransientDependency
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

        public async Task<List<Guid>> GetListHierarchyComponentIdRecursiveAsync(Guid id)
        {
            return (await GetCacheItemAsync(id)).HS.LHCId;
        }

        private async Task<HierarchicalStructureRecursiveCacheItem> GetCacheItemAsync(Guid hierarchicalStructureId)
        {
            return (await _hierarchicalStructureRecursiveCache.GetOrAddAsync(
                hierarchicalStructureId.ToString(), //Cache key
                async () => await GetFromDatabaseAsync(hierarchicalStructureId),
                () => new DistributedCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddHours(1) }
            ))!;
        }

        private async Task<HierarchicalStructureRecursiveCacheItem> GetFromDatabaseAsync(Guid hierarchicalStructureId)
        {
            var hierarchicalStructure = await _hierarchicalStructureRepository.GetAsync(hierarchicalStructureId);
            var lHierarchicalStructure = await _hierarchicalStructureRepository.GetListAsync(x => x.HierarchyId == hierarchicalStructure.HierarchyId);

            var hs = new HS { Id = hierarchicalStructureId };
            hs.LHCId = GetRecursive(lHierarchicalStructure, lHierarchicalStructure.Where(x => x.Id == hierarchicalStructureId).First());

            return new HierarchicalStructureRecursiveCacheItem { HS = hs };
        }

        private List<Guid> GetRecursive(List<HierarchicalStructure> lHierarchicalStructure, HierarchicalStructure hierarchicalStructure)
        {
            var l = new List<Guid> { hierarchicalStructure.HierarchyComponentId };

            var lChild = lHierarchicalStructure.Where(x => x.HierarchicalStructureIdParent == hierarchicalStructure.Id).ToList();
            foreach (var iChild in lChild)
                l.AddRange(GetRecursive(lHierarchicalStructure, iChild));

            return l;
        }



        public async Task<List<Guid>> GetListHierarchyComponentIdRecursiveAsync(Guid hierarchicalStructureId, int? hierarchyComponentType = null)
        {
            var cache = await GetCacheItemAsync(hierarchicalStructureId);
            return cache.HS.LHCId;
        }

        public async Task<List<Guid>> GetListHierarchyComponentIdRecursiveAsync(List<Guid> lHierarchicalStructureId, int? hierarchyComponentType = null)
        {
            var l = new List<Guid>();

            foreach (var iHierarchicalStructureId in lHierarchicalStructureId)
                l.AddRange(await GetListHierarchyComponentIdRecursiveAsync(iHierarchicalStructureId, hierarchyComponentType));

            return l.Distinct().ToList();
        }

        public async Task<List<Guid>> GetListHierarchicalStructureIdRecursiveAsync(Guid hierarchicalStructureId)
        {
            throw new NotImplementedException();
            //return (await GetListHierarchicalStructureRecursiveAsync(hierarchicalStructureId)).LHCId;
        }

        public async Task<List<Guid>> GetListHierarchicalStructureIdRecursiveAsync(List<Guid> lHierarchicalStructureId)
        {
            var l = new List<Guid>();

            foreach (var iHierarchicalStructureId in lHierarchicalStructureId)
                l.AddRange(await GetListHierarchicalStructureIdRecursiveAsync(iHierarchicalStructureId));

            return l.Distinct().ToList();
        }

        public async Task<HS> GetListHierarchicalStructureRecursiveAsync(Guid hierarchicalStructureId)
        {
            return (await GetCacheItemAsync(hierarchicalStructureId)).HS;
        }

        public Task<List<Guid>> GetHierarchyComponentIdAsync(List<Guid> lHierarchicalStructureId, int? hierarchyComponentType = null)
        {
            throw new NotImplementedException();
        }
    }
}
