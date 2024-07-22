using Necnat.Abp.NnMgmtAuthorization.Domains;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Necnat.Abp.NnMgmtAuthorization.HierarchicalPermissions
{
    public interface IHierarchicalAuthorizationService
    {
        Task<string> GetHierarchyComponentNameByHierarchyComponentIdAsync(Guid hierarchyComponentId);
        Task<List<Guid>> GetListHierarchicalStructureIdAsync(string permissionName);
        Task<List<HierarchyComponentDto>> GetListHierarchyComponentAsync(string permissionName, int? hierarchyComponentTypeId = null);
        Task<List<HierarchyComponentDto>> GetListHierarchyComponentAsync(List<Guid> hierarchyComponentIdList, int? hierarchyComponentTypeId = null);
        Task<List<Guid>> GetListHierarchyComponentIdAsync(List<Guid> hierarchicalStructureIdList);
        Task<bool> IsGrantedAsync(string permissionName, Guid? hierarchyComponentId = null);
    }
}
