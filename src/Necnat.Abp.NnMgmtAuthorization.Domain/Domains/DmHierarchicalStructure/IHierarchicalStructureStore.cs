using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public interface IHierarchicalStructureStore
    {
        Task<bool> HasHierarchyComponentIdAsync(Guid id, Guid hierarchyComponentId);
        Task<List<Guid>> GetListHierarchyComponentIdRecursiveAsync(Guid id);

        Task<bool> HasHierarchicalStructureIdAsync(Guid id, Guid hierarchicalStructureId);
        Task<List<Guid>> GetListHierarchicalStructureIdRecursiveAsync(Guid id);
        Task<List<Guid>> GetListHierarchicalStructureIdRecursiveAsync(List<Guid> idList);

        //Task<List<Guid>> GetHierarchyComponentIdAsync(List<Guid> lHierarchicalStructureId, int? hierarchyComponentType = null);
        //Task<List<Guid>> GetListHierarchyComponentIdRecursiveAsync(Guid hierarchicalStructureId, int? hierarchyComponentType = null);
        //Task<List<Guid>> GetListHierarchyComponentIdRecursiveAsync(List<Guid> lHierarchicalStructureId, int? hierarchyComponentType = null);
        //Task<List<Guid>> GetListHierarchicalStructureIdRecursiveAsync(Guid hierarchicalStructureId);
        //Task<List<Guid>> GetListHierarchicalStructureIdRecursiveAsync(List<Guid> lHierarchicalStructureId);
        //Task<List<HS>> GetListHierarchicalStructureRecursiveAsync(Guid hierarchicalStructureId);
    }
}
