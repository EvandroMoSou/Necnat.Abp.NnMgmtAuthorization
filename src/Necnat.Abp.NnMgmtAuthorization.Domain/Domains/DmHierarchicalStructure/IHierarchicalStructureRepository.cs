using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public interface IHierarchicalStructureRepository : IRepository<HierarchicalStructure, Guid>
    {
        Task<HierarchicalStructure?> GetAsync(Guid hierarchyId, Guid? hierarchicalStructureIdParent, int hierarchyComponentType, Guid hierarchyComponentId);
        Task<HierarchicalStructure?> GetAsync(Guid hierarchyId, int hierarchyComponentType, Guid hierarchyComponentId);
        Task<HierarchicalStructure> GetOrInsertAsync(Guid hierarchyId, Guid? hierarchicalStructureIdParent, int hierarchyComponentType, Guid hierarchyComponentId, bool autoSave = false);
        Task<List<HierarchicalStructure>> SearchByHierarchyComponentIdAsync(Guid hierarchyComponentId);
        Task<List<HierarchicalStructure>> SearchByHierarchicalStructureIdParentAsync(Guid? hierarchicalStructureIdParent);
        Task<List<HierarchicalStructure>> SearchByHierarchicalStructureIdParentAndHierarchyComponentTypeAndNotInHierarchyComponentIdAsync(Guid? hierarchicalStructureIdParent, int hierarchyComponentType, List<Guid> lHierarchyComponentId);
        Task<List<HierarchicalStructure>> SearchByHierarchyIdAndHierarchicalStructureIdParentAsync(Guid? hierarchyId, Guid? hierarchicalStructureIdParent);
        Task<bool> AnyByHierarchyIdAndHierarchicalStructureIdParentAsync(Guid hierarchyId, Guid? hierarchicalStructureIdParent);
        Task<bool> AnyByHierarchyIdAndHierarchicalComponentIdAsync(Guid hierarchyId, Guid hierarchicalComponentId);
        Task<Dictionary<Guid, List<Guid>>> GetDictionaryHierarchyComponentIdToHierarchicalStructureIdListAsync(List<Guid> lHierarchyComponentId);
        Task<int> DeleteAllByHierarchyIdAsync(Guid hierarchyId);
    }
}
