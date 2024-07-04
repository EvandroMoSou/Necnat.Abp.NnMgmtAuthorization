using Necnat.Abp.NnLibCommon.Models.TypedModels;
using System;
using System.Collections.Generic;

namespace Necnat.Abp.NnMgmtAuthorization.Domains.DmHierarchicalAccess
{
    [Serializable]
    public class UserHierarchicalAccessCacheItem
    {
        public List<UserHierarchicalAccess> List { get; set; }

        public UserHierarchicalAccessCacheItem()
        {
            List = new List<UserHierarchicalAccess>();
        }

        public UserHierarchicalAccessCacheItem(List<UserHierarchicalAccess> list)
        {
            List = list;
        }
    }
}
