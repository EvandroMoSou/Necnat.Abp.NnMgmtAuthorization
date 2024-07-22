using Necnat.Abp.NnLibCommon.Domains.NnIdentity;
using Necnat.Abp.NnMgmtAuthorization.Domains;
using Necnat.Abp.NnMgmtAuthorization.Domains.DmHierarchicalAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Clients;
using Volo.Abp.MultiTenancy;
using Volo.Abp.SimpleStateChecking;
using Volo.Abp.Users;

namespace Necnat.Abp.NnMgmtAuthorization.HierarchicalPermissions
{
    public class HierarchicalAuthorizationService : IHierarchicalAuthorizationService
    {
        protected readonly ICurrentClient _currentClient;
        protected readonly ICurrentTenant _currentTenant;
        protected readonly ICurrentUser _currentUser;
        protected readonly IHierarchicalAccessStore _hierarchicalAccessStore;
        protected readonly IHierarchicalStructureStore _hierarchicalStructureStore;
        protected readonly INnRoleStore _nnRoleStore;
        protected readonly IPermissionDefinitionManager _permissionDefinitionManager;
        protected readonly IPermissionStore _permissionStore;
        protected readonly ISimpleStateCheckerManager<PermissionDefinition> _stateCheckerManager;

        public HierarchicalAuthorizationService(
            ICurrentClient currentClient,
            ICurrentTenant currentTenant,
            ICurrentUser currentUser,
            IHierarchicalAccessStore hierarchicalAccessStore,
            IHierarchicalStructureStore hierarchicalStructureStore,
            INnRoleStore nnRoleStore,
            IPermissionDefinitionManager permissionDefinitionManager,
            IPermissionStore permissionStore,
            ISimpleStateCheckerManager<PermissionDefinition> stateCheckerManager)
        {
            _currentClient = currentClient;
            _currentTenant = currentTenant;
            _currentUser = currentUser;
            _hierarchicalAccessStore = hierarchicalAccessStore;
            _hierarchicalStructureStore = hierarchicalStructureStore;
            _nnRoleStore = nnRoleStore;
            _permissionDefinitionManager = permissionDefinitionManager;
            _permissionStore = permissionStore;
            _stateCheckerManager = stateCheckerManager;
        }

        public string GetHierarchyComponentNameByHierarchyComponentId(Guid hierarchyComponentId)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetHierarchyComponentNameByHierarchyComponentIdAsync(Guid hierarchyComponentId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Guid>> GetListHierarchicalStructureIdAsync(string permissionName)
        {
            var list = new List<Guid>();

            var userHierarchicalAccessList = await _hierarchicalAccessStore.GetListUserHierarchicalAccessByUserIdAsync((Guid)_currentUser.Id!);
            foreach (var iUserHierarchicalAccess in userHierarchicalAccessList)
                if (await _nnRoleStore.HasPermissionNameAsync(iUserHierarchicalAccess.RId, permissionName))
                    list.Add(iUserHierarchicalAccess.HSId);

            return list.Distinct().ToList();
        }

        public async Task<List<HierarchyComponentDto>> GetListHierarchyComponentAsync(string permissionName, int? hierarchyComponentTypeId = null)
        {
            return await GetListHierarchyComponentAsync(await GetListHierarchyComponentIdAsync(await GetListHierarchicalStructureIdAsync(permissionName)), hierarchyComponentTypeId);
        }

        public Task<List<HierarchyComponentDto>> GetListHierarchyComponentAsync(List<Guid> hierarchyComponentIdList, int? hierarchyComponentTypeId = null)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Guid>> GetListHierarchyComponentIdAsync(List<Guid> hierarchicalStructureIdList)
        {
            var list = new List<Guid>();

            foreach (var iHierarchicalStructureId in hierarchicalStructureIdList)
                list.AddRange(await _hierarchicalStructureStore.GetListHierarchyComponentIdRecursiveAsync(iHierarchicalStructureId));

            return list.Distinct().ToList();
        }

        public async Task<bool> IsGrantedAsync(string permissionName, Guid? hierarchyComponentId = null)
        {
            var permission = await _permissionDefinitionManager.GetOrNullAsync(permissionName);
            if (permission == null)
            {
                return false;
            }

            if (!permission.IsEnabled)
            {
                return false;
            }

            if (!await _stateCheckerManager.IsEnabledAsync(permission))
            {
                return false;
            }

            var multiTenancySide = _currentTenant.GetMultiTenancySide();
            if (!permission.MultiTenancySide.HasFlag(multiTenancySide))
            {
                return false;
            }

            // Client
            if (_currentClient.Id != null)
            {
                if (await _permissionStore.IsGrantedAsync(permissionName, "C", _currentClient.Id))
                    return true;
            }

            // Hierarchical Access
            if (_currentUser.Id == null)
                return false;

            var userHierarchicalAccessList = await _hierarchicalAccessStore.GetListUserHierarchicalAccessByUserIdAsync((Guid)_currentUser.Id);
            foreach (var iUserHierarchicalAccess in userHierarchicalAccessList)
            {
                if (!await _nnRoleStore.HasPermissionNameAsync(iUserHierarchicalAccess.RId, permissionName))
                    continue;

                if (hierarchyComponentId == null)
                    return true;
                else if (await _hierarchicalStructureStore.HasHierarchyComponentIdRecursiveAsync((Guid)iUserHierarchicalAccess.HSId, (Guid)hierarchyComponentId))
                    return true;
            }

            return false;
        }
    }
}
