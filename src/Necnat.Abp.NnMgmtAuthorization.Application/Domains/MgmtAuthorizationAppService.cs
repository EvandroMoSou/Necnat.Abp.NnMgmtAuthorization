using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Users;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    [Authorize]
    public class MgmtAuthorizationAppService : NnMgmtAuthorizationAppService, IMgmtAuthorizationAppService
    {
        protected readonly ICurrentUser _currentUser;
        protected readonly IHierarchicalStructureRecursiveService _hierarchicalStructureRecursiveService;
        protected readonly IMgmtAuthorizationService _mgmtAuthorizationService;

        public MgmtAuthorizationAppService(
            ICurrentUser currentUser,
            IHierarchicalStructureRecursiveService hierarchicalStructureRecursiveService,
            IMgmtAuthorizationService mgmtAuthorizationService)
        {
            _currentUser = currentUser;
            _hierarchicalStructureRecursiveService = hierarchicalStructureRecursiveService;
            _mgmtAuthorizationService = mgmtAuthorizationService;
        }

        public virtual async Task<List<string>> GetListPermissionMyAsync()
        {
            return await _mgmtAuthorizationService.GetListPermissionByUserIdAsync((Guid)CurrentUser.Id!);
        }

        public virtual async Task<List<Guid?>> GetListHierarchyComponentIdByPermissionNameAndHierarchyComponentTypeAsync(string permissionName, int hierarchyComponentType)
        {
            var userId = CurrentUser.Id;
            if (userId == null)
                return new List<Guid?>();

            var hierarchicalStructureIdList = await _mgmtAuthorizationService.GetListHierarchicalStructureIdByUserIdAndPermissionNameAsync((Guid)userId, permissionName);
            if (hierarchicalStructureIdList.Contains(null))
                return new List<Guid?> { null };

            var l = new List<Guid>();
            foreach (var iHierarchicalStructureId in hierarchicalStructureIdList)
                l.AddRange(await _hierarchicalStructureRecursiveService.GetListHierarchyComponentIdRecursiveAsync((Guid)iHierarchicalStructureId!, hierarchyComponentType));

            return l.Distinct().Select(x => (Guid?)x).ToList();
        }
    }
}
