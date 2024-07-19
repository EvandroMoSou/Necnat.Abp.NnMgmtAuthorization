using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Caching;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class HierarchicalStructureStore : IHierarchicalStructureStore
    {
        readonly IDistributedCache<HierarchyComponentIdRecursiveCacheItem> _hierarchyComponentIdRecursiveCache;
        readonly IDistributedCache<HierarchicalStructureIdRecursiveCacheItem> _hierarchicalStructureIdRecursiveCache;
        readonly IHierarchicalStructureRepository _hierarchicalStructureRepository;

        public HierarchicalStructureStore(
            IDistributedCache<HierarchyComponentIdRecursiveCacheItem> hierarchyComponentIdRecursiveCache,
            IDistributedCache<HierarchicalStructureIdRecursiveCacheItem> hierarchicalStructureIdRecursiveCache,
            IHierarchicalStructureRepository hierarchicalStructureRepository)
        {
            _hierarchyComponentIdRecursiveCache = hierarchyComponentIdRecursiveCache;
            _hierarchicalStructureIdRecursiveCache = hierarchicalStructureIdRecursiveCache;
            _hierarchicalStructureRepository = hierarchicalStructureRepository;
        }

        #region HierarchyComponentIdRecursive

        public virtual async Task<List<Guid>> GetListHierarchyComponentIdRecursiveAsync(Guid id)
        {
            return (await GetHierarchyComponentIdRecursiveCacheItem(id)).HierarchyComponentIdList;
        }

        public virtual async Task<bool> HasHierarchyComponentIdRecursiveAsync(Guid id, Guid hierarchyComponentId)
        {
            return (await GetListHierarchyComponentIdRecursiveAsync(id)).Contains(hierarchyComponentId);
        }


        protected virtual async Task<HierarchyComponentIdRecursiveCacheItem> GetHierarchyComponentIdRecursiveCacheItem(Guid id)
        {
            return (await _hierarchyComponentIdRecursiveCache.GetOrAddAsync(
                id.ToString(), //Cache key
                async () => await GetHierarchyComponentIdRecursiveData(id),
                () => new DistributedCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddHours(1) }
            ))!;
        }

        protected virtual async Task<HierarchyComponentIdRecursiveCacheItem> GetHierarchyComponentIdRecursiveData(Guid id)
        {
            var hierarchicalStructure = await _hierarchicalStructureRepository.GetAsync(id);
            var lHierarchicalStructure = await _hierarchicalStructureRepository.GetListAsync(x => x.HierarchyId == hierarchicalStructure.HierarchyId);

            return new HierarchyComponentIdRecursiveCacheItem { HierarchyComponentIdList = GetHierarchyComponentIdRecursive(lHierarchicalStructure, lHierarchicalStructure.Where(x => x.Id == id).First()) };
        }

        protected virtual List<Guid> GetHierarchyComponentIdRecursive(List<HierarchicalStructure> lHierarchicalStructure, HierarchicalStructure hierarchicalStructure)
        {
            var l = new List<Guid> { hierarchicalStructure.HierarchyComponentId };

            var lChild = lHierarchicalStructure.Where(x => x.HierarchicalStructureIdParent == hierarchicalStructure.Id).ToList();
            foreach (var iChild in lChild)
                l.AddRange(GetHierarchyComponentIdRecursive(lHierarchicalStructure, iChild));

            return l;
        }

        #endregion

        #region HierarchicalStructureIdRecursive

        public virtual async Task<List<Guid>> GetListHierarchicalStructureIdRecursiveAsync(Guid id)
        {
            return (await GetHierarchicalStructureIdRecursiveCacheItem(id)).HierarchicalStructureIdList;
        }

        public virtual async Task<List<Guid>> GetListHierarchicalStructureIdRecursiveAsync(List<Guid> idList)
        {
            var l = new List<Guid>();

            foreach (var id in idList)
                l.AddRange(await GetListHierarchicalStructureIdRecursiveAsync(id));

            return l;
        }

        public virtual async Task<bool> HasHierarchicalStructureIdRecursiveAsync(Guid id, Guid hierarchicalStructureId)
        {
            return (await GetListHierarchicalStructureIdRecursiveAsync(id)).Contains(hierarchicalStructureId);
        }

        public virtual async Task<bool> HasHierarchicalStructureIdRecursiveAsync(List<Guid> idList, Guid hierarchicalStructureId)
        {
            foreach (var id in idList)
                if (await HasHierarchicalStructureIdRecursiveAsync(id, hierarchicalStructureId))
                    return true;

            return false;
        }

        protected virtual async Task<HierarchicalStructureIdRecursiveCacheItem> GetHierarchicalStructureIdRecursiveCacheItem(Guid id)
        {
            return (await _hierarchicalStructureIdRecursiveCache.GetOrAddAsync(
                id.ToString(), //Cache key
                async () => await GetHierarchicalStructureIdRecursiveData(id),
                () => new DistributedCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddHours(1) }
            ))!;
        }

        protected virtual async Task<HierarchicalStructureIdRecursiveCacheItem> GetHierarchicalStructureIdRecursiveData(Guid id)
        {
            var hierarchicalStructure = await _hierarchicalStructureRepository.GetAsync(id);
            var lHierarchicalStructure = await _hierarchicalStructureRepository.GetListAsync(x => x.HierarchyId == hierarchicalStructure.HierarchyId);

            return new HierarchicalStructureIdRecursiveCacheItem { HierarchicalStructureIdList = GetHierarchicalStructureIdRecursive(lHierarchicalStructure, lHierarchicalStructure.Where(x => x.Id == id).First()) };
        }

        protected virtual List<Guid> GetHierarchicalStructureIdRecursive(List<HierarchicalStructure> lHierarchicalStructure, HierarchicalStructure hierarchicalStructure)
        {
            var l = new List<Guid> { hierarchicalStructure.Id };

            var lChild = lHierarchicalStructure.Where(x => x.HierarchicalStructureIdParent == hierarchicalStructure.Id).ToList();
            foreach (var iChild in lChild)
                l.AddRange(GetHierarchicalStructureIdRecursive(lHierarchicalStructure, iChild));

            return l;
        }

        #endregion
    }
}
