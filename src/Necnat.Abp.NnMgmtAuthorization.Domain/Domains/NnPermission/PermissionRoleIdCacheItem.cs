using System;
using System.Collections.Generic;
using System.Text;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class PermissionRoleIdCacheItem
    {
        public List<Guid> RoleIdList { get; set; } = new List<Guid>();
    }
}
