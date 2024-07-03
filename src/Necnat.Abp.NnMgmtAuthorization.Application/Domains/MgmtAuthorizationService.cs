using Necnat.Abp.NnLibCommon.Domains.NnIdentity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.PermissionManagement;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class MgmtAuthorizationService : IMgmtAuthorizationService
    {
        protected readonly IHierarchicalAccessRepository _hierarchicalAccessRepository;
        protected readonly IHierarchicalStructureRecursiveService _hierarchicalStructureRecursiveService;
        protected readonly IPermissionGrantRepository _permissionGrantRepository;
        protected readonly IPermissionStore _permissionStore;
        protected readonly IRoleNameService _roleNameService;

        public MgmtAuthorizationService(
            IHierarchicalAccessRepository hierarchicalAccessRepository,
            IHierarchicalStructureRecursiveService hierarchicalStructureRecursiveService,
            IPermissionGrantRepository permissionGrantRepository,
            IPermissionStore permissionStore,
            IRoleNameService roleNameService)
        {
            _hierarchicalAccessRepository = hierarchicalAccessRepository;
            _hierarchicalStructureRecursiveService = hierarchicalStructureRecursiveService;
            _permissionGrantRepository = permissionGrantRepository;
            _permissionStore = permissionStore;
            _roleNameService = roleNameService;
        }

        public async Task<List<string>> GetListPermissionByUserIdAsync(Guid userId)
        {
            var dbHierarchicalAccessList = await _hierarchicalAccessRepository.GetListByUserIdAsync(userId);

            var permissionList = new List<string>();
            foreach (var iDbHierarchicalAccess in dbHierarchicalAccessList)
                permissionList.AddRange((await _permissionGrantRepository.GetListAsync("R", await _roleNameService.GetByIdAsync(iDbHierarchicalAccess.RoleId))).Select(x => x.Name));

            return permissionList.Distinct().ToList();
        }

        public async Task<List<Guid?>> GetListHierarchicalStructureIdByUserIdAndPermissionNameAsync(Guid userId, string permissionName)
        {
            var dbHierarchicalAccessList = await _hierarchicalAccessRepository.GetListByUserIdAsync((Guid)userId);

            List<HierarchicalAccess> hierarchicalAccessList = new List<HierarchicalAccess>();
            foreach (var iDbHierarchicalAccess in dbHierarchicalAccessList)
                if (await _permissionStore.IsGrantedAsync(permissionName, "R", await _roleNameService.GetByIdAsync(iDbHierarchicalAccess.RoleId)))
                    hierarchicalAccessList.Add(iDbHierarchicalAccess);

            return hierarchicalAccessList.Select(x => x.HierarchicalStructureId).Distinct().ToList();
        }
    }
}
