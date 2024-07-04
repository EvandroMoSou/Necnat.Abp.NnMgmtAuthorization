using Necnat.Abp.NnLibCommon.Domains.NnIdentity;
using Necnat.Abp.NnMgmtAuthorization.Domains;
using Necnat.Abp.NnMgmtAuthorization.Domains.DmHierarchicalAccess;
using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using Volo.Abp.SimpleStateChecking;

namespace Necnat.Abp.NnMgmtAuthorization.Permissions
{
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IPermissionChecker), typeof(PermissionChecker))]
    public class AppPermissionChecker : PermissionChecker, IPermissionChecker, ITransientDependency
    {
        protected readonly IHierarchicalAccessStore _hierarchicalAccessStore;
        protected readonly IHierarchicalStructureStore _hierarchicalStructureStore;
        protected readonly INnRoleStore _nnRoleStore;
        protected readonly IPermissionStore _permissionStore;

        public const string _separator = "&";

        public AppPermissionChecker(
            ICurrentPrincipalAccessor principalAccessor,
            IPermissionDefinitionManager permissionDefinitionManager,
            ICurrentTenant currentTenant,
            IPermissionValueProviderManager permissionValueProviderManager,
            ISimpleStateCheckerManager<PermissionDefinition> stateCheckerManager,
            IHierarchicalAccessStore hierarchicalAccessStore,
            IHierarchicalStructureStore hierarchicalStructureStore,
            INnRoleStore nnRoleStore,
            IPermissionStore permissionStore) : base(principalAccessor, permissionDefinitionManager, currentTenant, permissionValueProviderManager, stateCheckerManager)
        {
            _hierarchicalAccessStore = hierarchicalAccessStore;
            _hierarchicalStructureStore = hierarchicalStructureStore;
            _nnRoleStore = nnRoleStore;
            _permissionStore = permissionStore;
        }

        public override async Task<bool> IsGrantedAsync(
            ClaimsPrincipal? claimsPrincipal,
            string name)
        {
            Check.NotNull(name, nameof(name));

            var permissionName = name;
            Guid? hierarchyComponentId = null;
            if (name.Contains(_separator))
            {
                var splitName = name.Split(_separator);
                permissionName = splitName[0];
                hierarchyComponentId = new Guid(splitName[1]);
            }

            var permission = await PermissionDefinitionManager.GetOrNullAsync(permissionName);
            if (permission == null)
            {
                return false;
            }

            if (!permission.IsEnabled)
            {
                return false;
            }

            if (!await StateCheckerManager.IsEnabledAsync(permission))
            {
                return false;
            }

            var multiTenancySide = claimsPrincipal?.GetMultiTenancySide() ?? CurrentTenant.GetMultiTenancySide();
            if (!permission.MultiTenancySide.HasFlag(multiTenancySide))
            {
                return false;
            }

            // Client
            var clientId = claimsPrincipal?.FindFirst(AbpClaimTypes.ClientId)?.Value;
            if (clientId != null)
            {
                if (await _permissionStore.IsGrantedAsync(permissionName, "C", clientId))
                    return true;
            }

            // HierarchicalAccess
            var userId = claimsPrincipal?.FindFirst(AbpClaimTypes.UserId)?.Value;
            if (userId == null)
                return false;

            var userHierarchicalAccessList = await _hierarchicalAccessStore.GetListUserHierarchicalAccessByUserIdAsync(new Guid(userId));
            foreach (var iUserHierarchicalAccess in userHierarchicalAccessList)
            {
                if (!await _nnRoleStore.HasPermissionNameAsync(iUserHierarchicalAccess.RId, permissionName))
                    continue;

                if (hierarchyComponentId == null)
                    return true;
                else if (await _hierarchicalStructureStore.HasHierarchyComponentIdAsync(iUserHierarchicalAccess.HSId, (Guid)hierarchyComponentId))
                    return true;
            }

            return false;
        }

        public new async Task<MultiplePermissionGrantResult> IsGrantedAsync(ClaimsPrincipal? claimsPrincipal, string[] names)
        {
            var multiplePermissionGrantResult = new MultiplePermissionGrantResult();
            foreach (var name in names)
                multiplePermissionGrantResult.Result.Add(name, (await IsGrantedAsync(claimsPrincipal, name)) ? PermissionGrantResult.Granted : PermissionGrantResult.Prohibited);

            return multiplePermissionGrantResult;
        }
    }
}
