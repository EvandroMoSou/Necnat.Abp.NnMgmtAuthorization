using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class HierarchicalStructureRecursiveCacheItem
    {
        public List<HS> LHSR { get; set; } = new List<HS>();
    }
}
