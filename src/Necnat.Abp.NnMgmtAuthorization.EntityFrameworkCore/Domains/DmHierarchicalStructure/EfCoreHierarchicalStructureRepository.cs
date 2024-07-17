using Microsoft.EntityFrameworkCore;
using Necnat.Abp.NnMgmtAuthorization.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Necnat.Abp.NnMgmtAuthorization.Domains.DmHierarchicalStructure
{
    public class EfCoreHierarchicalStructureRepository : EfCoreRepository<INnMgmtAuthorizationDbContext, HierarchicalStructure, Guid>, IHierarchicalStructureRepository
    {
        public EfCoreHierarchicalStructureRepository(IDbContextProvider<INnMgmtAuthorizationDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public virtual async Task<HierarchicalStructure?> GetAsync(Guid hierarchyId, Guid? hierarchicalStructureIdParent, int hierarchyComponentType, Guid hierarchyComponentId)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.Where(x => x.HierarchyId == hierarchyId
                && x.HierarchicalStructureIdParent == hierarchicalStructureIdParent
                && x.HierarchyComponentType == hierarchyComponentType
                && x.HierarchyComponentId == hierarchyComponentId).FirstOrDefaultAsync();
        }

        public virtual async Task<HierarchicalStructure?> GetAsync(Guid hierarchyId, int hierarchyComponentType, Guid hierarchyComponentId)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.Where(x => x.HierarchyId == hierarchyId
                && x.HierarchyComponentType == hierarchyComponentType
                && x.HierarchyComponentId == hierarchyComponentId).FirstOrDefaultAsync();
        }

        public virtual async Task<HierarchicalStructure> GetOrInsertAsync(Guid hierarchyId, Guid? hierarchicalStructureIdParent, int hierarchyComponentType, Guid hierarchyComponentId, bool autoSave = false)
        {
            var e = await GetAsync(hierarchyId, hierarchicalStructureIdParent, hierarchyComponentType, hierarchyComponentId);
            if (e == null)
            {
                e = await InsertAsync(
                    new HierarchicalStructure
                    {
                        HierarchyId = hierarchyId,
                        HierarchicalStructureIdParent = hierarchicalStructureIdParent,
                        HierarchyComponentType = hierarchyComponentType,
                        HierarchyComponentId = hierarchyComponentId
                    },
                    autoSave: autoSave);
            }

            return e;
        }

        public virtual async Task<List<HierarchicalStructure>> SearchByHierarchyComponentIdAsync(Guid hierarchyComponentId)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.Where(x =>
                x.HierarchyComponentId == hierarchyComponentId).ToListAsync();
        }

        public virtual async Task<List<HierarchicalStructure>> SearchByHierarchicalStructureIdParentAndHierarchyComponentTypeAndNotInHierarchyComponentIdAsync(Guid? hierarchicalStructureIdParent, int hierarchyComponentType, List<Guid> lHierarchyComponentId)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.Where(x =>
                x.HierarchicalStructureIdParent == hierarchicalStructureIdParent
                && x.HierarchyComponentType == hierarchyComponentType
                && (!lHierarchyComponentId.Contains(x.HierarchyComponentId))).ToListAsync();
        }

        public virtual async Task<List<HierarchicalStructure>> SearchByHierarchicalStructureIdParentAsync(Guid? hierarchicalStructureIdParent)
        {
            return await (await GetDbSetAsync()).Where(x => x.HierarchicalStructureIdParent == hierarchicalStructureIdParent).ToListAsync();
        }

        public virtual async Task<List<HierarchicalStructure>> SearchByHierarchyIdAndHierarchicalStructureIdParentAsync(Guid? hierarchyId, Guid? hierarchicalStructureIdParent)
        {
            return await ((await GetDbSetAsync()).Where(x => x.HierarchyId == hierarchyId && x.HierarchicalStructureIdParent == hierarchicalStructureIdParent)).ToListAsync();
        }

        public virtual async Task<bool> AnyByHierarchyIdAndHierarchicalStructureIdParentAsync(Guid hierarchyId, Guid? hierarchicalStructureIdParent)
        {
            return (await GetDbSetAsync()).Any(x => x.HierarchyId == hierarchyId && x.HierarchicalStructureIdParent == hierarchicalStructureIdParent);
        }

        public virtual async Task<bool> AnyByHierarchyIdAndHierarchicalComponentIdAsync(Guid hierarchyId, Guid hierarchicalComponentId)
        {
            return (await GetDbSetAsync()).Any(x => x.HierarchyId == hierarchyId && x.HierarchyComponentId == hierarchicalComponentId);
        }

        public virtual async Task<Dictionary<Guid, List<Guid>>> GetDictionaryHierarchyComponentIdToHierarchicalStructureIdListAsync(List<Guid> lHierarchyComponentId)
        {
            lHierarchyComponentId = lHierarchyComponentId.Distinct().ToList();

            var dbSet = await GetDbSetAsync();
            var lHierarchicalStructure = await dbSet.Where(x =>
                lHierarchyComponentId.Contains(x.HierarchyComponentId)).ToListAsync();

            var dict = new Dictionary<Guid, List<Guid>>();
            foreach (var iHierarchyComponentId in lHierarchyComponentId)
                dict.Add(iHierarchyComponentId, lHierarchicalStructure.Where(x => x.HierarchyComponentId == iHierarchyComponentId).Select(x => x.Id).ToList());

            return dict;
        }

        public async Task<int> DeleteAllByHierarchyIdAsync(Guid hierarchyId)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.Where(x => x.HierarchyId == hierarchyId).ExecuteDeleteAsync();
        }
    }
}
