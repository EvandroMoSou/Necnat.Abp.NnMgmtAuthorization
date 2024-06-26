using Necnat.Abp.NnLibCommon.Dtos;
using System;
using System.Collections.Generic;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class HierarchicalAccessResultRequestDto : OptionalPagedAndSortedResultRequestDto
    {
        public Guid? UserId { get; set; }
        public Guid? RoleId { get; set; }
        public List<Guid>? HierarchicalStructureIdList { get; set; }
        public bool WithHierarchy { get; set; }
    }
}
