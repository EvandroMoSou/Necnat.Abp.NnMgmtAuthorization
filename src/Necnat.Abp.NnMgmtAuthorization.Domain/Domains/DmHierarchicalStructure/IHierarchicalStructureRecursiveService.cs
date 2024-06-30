using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public interface IHierarchicalStructureRecursiveService
    {
        Task<List<Guid>> GetHierarchyComponentIdAsync(List<Guid> lHierarchicalStructureId, int? hierarchyComponentType = null);
        Task<List<Guid>> GetListHierarchyComponentIdRecursiveAsync(Guid hierarchicalStructureId, int? hierarchyComponentType = null);
        Task<List<Guid>> GetListHierarchyComponentIdRecursiveAsync(List<Guid> lHierarchicalStructureId, int? hierarchyComponentType = null);
        Task<List<Guid>> GetListHierarchicalStructureIdRecursiveAsync(Guid hierarchicalStructureId);
        Task<List<Guid>> GetListHierarchicalStructureIdRecursiveAsync(List<Guid> lHierarchicalStructureId);
        Task<List<HS>> GetListHierarchicalStructureRecursiveAsync(Guid hierarchicalStructureId);
    }
}
