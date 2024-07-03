using Necnat.Abp.NnMgmtAuthorization.Domains;
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
    public class ServerPermissionChecker : PermissionChecker, IPermissionChecker, ITransientDependency
    {
        protected readonly IHierarchicalStructureStore _hierarchicalStructureRecursiveService;
        protected readonly IMgmtAuthorizationService _mgmtAuthorizationService;
        protected readonly IPermissionStore _permissionStore;

        public virtual string Separator { get; set; } = "&";

        public ServerPermissionChecker(
            ICurrentPrincipalAccessor principalAccessor,
            IPermissionDefinitionManager permissionDefinitionManager,
            ICurrentTenant currentTenant,
            IPermissionValueProviderManager permissionValueProviderManager,
            ISimpleStateCheckerManager<PermissionDefinition> stateCheckerManager,
            IHierarchicalStructureStore hierarchicalStructureRecursiveService,
            IMgmtAuthorizationService mgmtAuthorizationService,
            IPermissionStore permissionStore) : base(principalAccessor, permissionDefinitionManager, currentTenant, permissionValueProviderManager, stateCheckerManager)
        {
            _hierarchicalStructureRecursiveService = hierarchicalStructureRecursiveService;
            _mgmtAuthorizationService = mgmtAuthorizationService;
            _permissionStore = permissionStore;
        }

        public override async Task<bool> IsGrantedAsync(
            ClaimsPrincipal? claimsPrincipal,
            string name)
        {
            Check.NotNull(name, nameof(name));

            var permissionName = name;
            Guid? hierarchyComponentId = null;
            if (name.Contains(Separator))
            {
                var splitName = name.Split(Separator);
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

            var hierarchicalStructureIdList = await _mgmtAuthorizationService.GetListHierarchicalStructureIdByUserIdAndPermissionNameAsync(new Guid(userId), permissionName);
            foreach (var iHierarchicalStructureId in hierarchicalStructureIdList)
            {
                var hierarchyComponentIdList = await _hierarchicalStructureRecursiveService.GetListHierarchyComponentIdRecursiveAsync((Guid)iHierarchicalStructureId!);
                if (hierarchyComponentIdList.Contains((Guid)hierarchyComponentId!))
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
