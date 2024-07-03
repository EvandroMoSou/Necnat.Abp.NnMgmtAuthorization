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

        public async Task<List<Guid>> GetListHierarchyComponentIdRecursiveAsync(Guid hierarchicalStructureId, int? hierarchyComponentType = null)
        {
            var q = (await GetListHierarchicalStructureRecursiveAsync(hierarchicalStructureId)).AsQueryable();

            var l = new List<Guid>();
            foreach (var e in q.Select(x => x.LHCId))
                l.AddRange(e);

            return l;
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
            return (await GetListHierarchicalStructureRecursiveAsync(hierarchicalStructureId)).Select(x => x.Id).ToList();
        }

        public async Task<List<Guid>> GetListHierarchicalStructureIdRecursiveAsync(List<Guid> lHierarchicalStructureId)
        {
            var l = new List<Guid>();

            foreach (var iHierarchicalStructureId in lHierarchicalStructureId)
                l.AddRange(await GetListHierarchicalStructureIdRecursiveAsync(iHierarchicalStructureId));

            return l.Distinct().ToList();
        }

        public async Task<List<HS>> GetListHierarchicalStructureRecursiveAsync(Guid hierarchicalStructureId)
        {
            return (await GetCacheItemAsync(hierarchicalStructureId)).LHSR;
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
            var lHierarchicalStructure = await _hierarchicalStructureRepository.GetListAsync();
            return new HierarchicalStructureRecursiveCacheItem { LHSR = GetRecursive(lHierarchicalStructure, lHierarchicalStructure.Where(x => x.Id == hierarchicalStructureId).First()) };
        }

        private List<HS> GetRecursive(List<HierarchicalStructure> lHierarchicalStructure, HierarchicalStructure hierarchicalStructure)
        {
            var l = new List<HS> { FromHierarchicalStructure(hierarchicalStructure) };

            var lChild = lHierarchicalStructure.Where(x => x.HierarchicalStructureIdParent == hierarchicalStructure.Id).ToList();
            foreach (var iChild in lChild)
                l.AddRange(GetRecursive(lHierarchicalStructure, iChild));

            return l;
        }

        private HS FromHierarchicalStructure(HierarchicalStructure eh)
        {
            return new HS
            {
                Id = eh.Id
            };
        }

        public Task<List<Guid>> GetHierarchyComponentIdAsync(List<Guid> lHierarchicalStructureId, int? hierarchyComponentType = null)
        {
            throw new NotImplementedException();
        }
    }
}
