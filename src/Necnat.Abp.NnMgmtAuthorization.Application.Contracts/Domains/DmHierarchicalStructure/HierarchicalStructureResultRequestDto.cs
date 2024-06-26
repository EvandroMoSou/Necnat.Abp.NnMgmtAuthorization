using Necnat.Abp.NnLibCommon.Dtos;
using System;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class HierarchicalStructureResultRequestDto : OptionalPagedAndSortedResultRequestDto
    {
        public bool UseParentId { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? HierarchyId { get; set; }
    }
}
