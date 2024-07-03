using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public interface IHierarchicalAccessRepository : IRepository<HierarchicalAccess, Guid>
    {
        Task<List<HierarchicalAccess>> GetListByIdListAsync(List<Guid> idList);
        Task<List<HierarchicalAccess>> GetListByUserIdAsync(Guid userId);        
        Task<List<HierarchicalAccess>> GetListByUserIdAndRoleIdAsync(Guid userId, Guid roleId);
        Task<HierarchicalAccess> InsertIfNotExistsAsync(HierarchicalAccess entity, bool autoSave = false);
        Task CheckByHierarchyComponentIdWithHierarchyAsync(Guid userId, string permissionName, Guid hierarchyComponentId);
        Task CheckByHierarchicalStructureIdAsync(Guid userId, string permissionName, Guid hierarchicalStructureId);
        Task CheckByHierarchicalStructureIdWithHierarchyAsync(Guid userId, string permissionName, Guid hierarchicalStructureId);
        Task<List<Guid>> SearchHierarchyComponentIdWithHierarchyAsync(Guid userId, string permissionName);
        Task<List<Guid>> SearchHierarchicalStructureIdAsync(Guid userId, string permissionName);
        Task<List<Guid>> SearchHierarchicalStructureIdWithHierarchyAsync(Guid userId, string permissionName);
        Task MantainByUserIdAsync(Guid userId, List<MantainHierarchicalAccessByUserIdModel> lMaintain);
    }
}
