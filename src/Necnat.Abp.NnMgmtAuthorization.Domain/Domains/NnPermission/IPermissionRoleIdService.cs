using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public interface IPermissionRoleIdService
    {
        Task<List<Guid>> SearchRoleIdAsync(string permissionName);
    }
}
