using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public interface IHierarchicalAuthorizationService
    {
        string ToJson();
        Task InitializeAsync();
        bool CheckPermission(string permissionName, int hierarchyComponentType, Guid hierarchyComponentId);
        bool CheckPermission(string permissionName, Guid hierarchicalStructureId);
        HierarchicalStructureDto? GetHierarchicalStructure(Guid hierarchicalStructureId);
        string? GetHierarchyComponentName(Guid hierarchicalStructureId);
        List<HierarchyComponentModel> SearchHierarchyComponent(string permissionName, int? hierarchyComponentType = null);
        List<HierarchicalStructureDto> SearchHierarchicalStructure();
        List<HierarchicalStructureDto> SearchHierarchicalStructure(string permissionName);
        List<Guid> SearchHierarchicalStructureId(string permissionName);
        List<HierarchyDto> SearchHierarchy(string permissionName);
        List<HSC> SearchHierarchicalStructureContainer(string permissionName);
        List<HierarchicalStructureDto> SearchHierarchicalStructureHead(HSC hierarchicalStructure);
        bool IsChild(Guid hierarchicalStructureId, int hierarchyComponentType, Guid hierarchyComponentId);
        bool IsChild(Guid hierarchicalStructureId, Guid hierarchicalStructureIdFilho);

        bool GetWithHierarchyLastSelected(string permissionName, int? hierarchyComponentType = null);
        Guid GetHierarchyComponentIdLastSelected(string permissionName, int? hierarchyComponentType = null);
        List<Guid> GetHierarchyComponentIdListLastSelected(string permissionName, int? hierarchyComponentType = null);
        void SetWithHierarchyLastSelected(string permissionName, bool withHierarchy, int? hierarchyComponentType = null);
        void SetHierarchyComponentLastSelected(string permissionName, Guid id, int? hierarchyComponentType = null);
        void SetListHierarchyComponentLastSelected(string permissionName, List<Guid> lId, int? hierarchyComponentType = null);
    }
}
