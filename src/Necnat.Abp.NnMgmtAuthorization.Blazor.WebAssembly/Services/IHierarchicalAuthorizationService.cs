using Necnat.Abp.NnMgmtAuthorization.Domains;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Necnat.Abp.NnMgmtAuthorization.Blazor.WebAssembly.Services
{
    public interface IHierarchicalAuthorizationService
    {
        Task<bool> CheckAsync();
        Task<List<HierarchyComponentDto>> GetListHierarchyComponentAsync(string permissionName, int? hierarchyComponentTypeId = null);
        Task<List<Guid>> GetListHierarchicalStructureId(string permissionName);
        Task<List<Guid>> GetListHierarchyComponentId(List<Guid> hierarchicalStructureIdList);
        Task<List<HierarchyComponentDto>> GetListHierarchyComponent(List<Guid> hierarchyComponentIdList, int? hierarchyComponentTypeId = null);
        Task<bool> IsGrantedAsync(string permissionName, Guid? hierarchyComponentId = null);
    }
}
