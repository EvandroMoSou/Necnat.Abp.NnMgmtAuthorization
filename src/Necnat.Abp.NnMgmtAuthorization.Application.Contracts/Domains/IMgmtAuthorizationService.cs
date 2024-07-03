using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public interface IMgmtAuthorizationService : ITransientDependency
    {
        Task<List<string>> GetListPermissionByUserIdAsync(Guid userId);
        Task<List<Guid>> GetListHierarchicalStructureIdByUserIdAndPermissionNameAsync(Guid userId, string permissionName);
    }
}
