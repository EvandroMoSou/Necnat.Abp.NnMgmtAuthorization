//using Necnat.Abp.NnLibCommon.Domains.NnIdentity;
//using Necnat.Abp.NnMgmtAuthorization.Domains;
//using System;
//using System.Collections.Generic;
//using System.Security.Claims;
//using System.Security.Principal;
//using System.Threading.Tasks;
//using Volo.Abp;
//using Volo.Abp.Authorization.Permissions;
//using Volo.Abp.DependencyInjection;
//using Volo.Abp.MultiTenancy;
//using Volo.Abp.Security.Claims;
//using Volo.Abp.SimpleStateChecking;

//namespace Necnat.Abp.NnMgmtAuthorization.Permissions
//{
//    public class HierarchicalPermissionChecker : PermissionChecker, IPermissionChecker, ITransientDependency
//    {
//        protected readonly IHierarchicalAccessRepository _hierarchicalAccessRepository;
//        protected readonly IHierarchicalStructureRecursiveService _hierarchicalStructureRecursiveService;
//        protected readonly IPermissionStore _permissionStore;
//        protected readonly IRoleNameService _roleNameService;

//        public virtual string Separator { get; set; } = "&";

//        public HierarchicalPermissionChecker(
//            ICurrentPrincipalAccessor principalAccessor,
//            IPermissionDefinitionManager permissionDefinitionManager,
//            ICurrentTenant currentTenant,
//            IPermissionValueProviderManager permissionValueProviderManager,
//            ISimpleStateCheckerManager<PermissionDefinition> stateCheckerManager,
//            IHierarchicalAccessRepository hierarchicalAccessRepository,
//            IHierarchicalStructureRecursiveService hierarchicalStructureRecursiveService,
//            IPermissionStore permissionStore,
//            IRoleNameService roleNameService) : base(principalAccessor, permissionDefinitionManager, currentTenant, permissionValueProviderManager, stateCheckerManager)
//        {
//            _hierarchicalAccessRepository = hierarchicalAccessRepository;
//            _hierarchicalStructureRecursiveService = hierarchicalStructureRecursiveService;
//            _permissionStore = permissionStore;
//            _roleNameService = roleNameService;
//        }

//        public override async Task<bool> IsGrantedAsync(
//            ClaimsPrincipal? claimsPrincipal,
//            string name)
//        {
//            Check.NotNull(name, nameof(name));

//            var permissionName = name;
//            Guid? hierarchyComponentId = null;
//            if (name.Contains(Separator))
//            {
//                var splitName = name.Split(Separator);
//                permissionName = splitName[0];
//                hierarchyComponentId = new Guid(splitName[1]);
//            }

//            var permission = await PermissionDefinitionManager.GetOrNullAsync(permissionName);
//            if (permission == null)
//            {
//                return false;
//            }

//            if (!permission.IsEnabled)
//            {
//                return false;
//            }

//            if (!await StateCheckerManager.IsEnabledAsync(permission))
//            {
//                return false;
//            }

//            var multiTenancySide = claimsPrincipal?.GetMultiTenancySide() ?? CurrentTenant.GetMultiTenancySide();
//            if (!permission.MultiTenancySide.HasFlag(multiTenancySide))
//            {
//                return false;
//            }

//            // Client
//            var clientId = claimsPrincipal?.FindFirst(AbpClaimTypes.ClientId)?.Value;
//            if (clientId != null)
//            {
//                if (await _permissionStore.IsGrantedAsync(permissionName, "C", clientId))
//                    return true;
//            }

//            // HierarchicalAccess
//            var userId = claimsPrincipal?.FindFirst(AbpClaimTypes.UserId)?.Value;
//            if (userId == null)
//                return false;

//            var hierarchicalAccessList = await _hierarchicalAccessRepository.GetListByUserIdAsync(new Guid(userId));

//            List<HierarchicalAccess> allowedList;
//            if (hierarchyComponentId == null)
//                allowedList = hierarchicalAccessList;
//            else
//            {
//                allowedList = new List<HierarchicalAccess>();
//                foreach (var ihierarchicalAccess in hierarchicalAccessList)
//                {
//                    if (ihierarchicalAccess.HierarchicalStructureId == null)
//                    {
//                        allowedList.Add(ihierarchicalAccess);
//                        continue;
//                    }

//                    var hierarchyComponentIdList = await _hierarchicalStructureRecursiveService.GetListHierarchyComponentIdRecursiveAsync((Guid)ihierarchicalAccess.HierarchicalStructureId);
//                    if (hierarchyComponentIdList.Contains((Guid)hierarchyComponentId))
//                        allowedList.Add(ihierarchicalAccess);
//                }
//            }

//            foreach (var iAllowed in allowedList)
//                if (await _permissionStore.IsGrantedAsync(permissionName, "R", await _roleNameService.GetByIdAsync(iAllowed.RoleId)))
//                    return true;

//            return false;
//        }

//        public new async Task<MultiplePermissionGrantResult> IsGrantedAsync(ClaimsPrincipal? claimsPrincipal, string[] names)
//        {
//            var multiplePermissionGrantResult = new MultiplePermissionGrantResult();
//            foreach (var name in names)
//                multiplePermissionGrantResult.Result.Add(name, (await IsGrantedAsync(claimsPrincipal, name)) ? PermissionGrantResult.Granted : PermissionGrantResult.Prohibited);
            
//            return multiplePermissionGrantResult;            
//        }
//    }
//}
