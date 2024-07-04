using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Necnat.Abp.NnMgmtAuthorization.Domains.DmHierarchicalAccess
{
    public interface IHierarchicalAccessStore
    {
        Task<List<UserHierarchicalAccess>> GetListUserHierarchicalAccessByUserIdAsync(Guid userId);
    }
}
