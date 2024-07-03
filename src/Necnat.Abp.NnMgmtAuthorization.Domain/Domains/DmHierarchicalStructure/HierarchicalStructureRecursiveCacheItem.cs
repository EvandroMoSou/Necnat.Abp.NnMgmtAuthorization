using Necnat.Abp.NnMgmtAuthorization.Models;
using System.Collections.Generic;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class HierarchicalStructureRecursiveCacheItem
    {
        public List<HS> LHSR { get; set; } = new List<HS>();
    }
}
