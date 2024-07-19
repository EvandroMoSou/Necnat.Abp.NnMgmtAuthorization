using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public interface IHierarchicalStructureStore
    {
        Task<List<Guid>> GetListHierarchyComponentIdRecursiveAsync(Guid id);
        Task<bool> HasHierarchyComponentIdRecursiveAsync(Guid id, Guid hierarchyComponentId);

        Task<List<Guid>> GetListHierarchicalStructureIdRecursiveAsync(Guid id);
        Task<List<Guid>> GetListHierarchicalStructureIdRecursiveAsync(List<Guid> idList);
        Task<bool> HasHierarchicalStructureIdRecursiveAsync(Guid id, Guid hierarchicalStructureId);
        Task<bool> HasHierarchicalStructureIdRecursiveAsync(List<Guid> idList, Guid hierarchicalStructureId);
    }
}
