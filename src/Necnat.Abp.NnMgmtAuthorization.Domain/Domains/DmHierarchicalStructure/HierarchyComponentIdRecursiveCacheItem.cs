using System;
using System.Collections.Generic;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class HierarchyComponentIdRecursiveCacheItem
    {
        public List<Guid> HierarchyComponentIdList { get; set; } = new List<Guid>();
    }
}
