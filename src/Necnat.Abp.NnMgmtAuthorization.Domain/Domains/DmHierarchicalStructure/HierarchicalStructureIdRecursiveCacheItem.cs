using System;
using System.Collections.Generic;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class HierarchicalStructureIdRecursiveCacheItem
    {
        public List<Guid> HierarchicalStructureIdList { get; set; } = new List<Guid>();
    }
}
