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
        Task<List<Guid>> SearchHierarchyComponentIdRecursiveAsync(Guid hierarchicalStructureId, int? hierarchyComponentType = null);
        Task<List<Guid>> SearchHierarchyComponentIdRecursiveAsync(List<Guid> lHierarchicalStructureId, int? hierarchyComponentType = null);
        Task<List<Guid>> SearchHierarchicalStructureIdRecursiveAsync(Guid hierarchicalStructureId);
        Task<List<Guid>> SearchHierarchicalStructureIdRecursiveAsync(List<Guid> lHierarchicalStructureId);
        Task<List<HS>> SearchHierarchicalStructureRecursiveAsync(Guid hierarchicalStructureId);
    }
}
