using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public interface IMgmtAuthorizationAppService : IApplicationService, IRemoteService
    {
        Task<List<string>> GetListPermissionMyAsync();
        Task<List<Guid?>> GetListHierarchyComponentIdByPermissionNameAndHierarchyComponentTypeAsync(string permissionName, int hierarchyComponentType);
    }
}
