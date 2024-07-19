using Necnat.Abp.NnLibCommon.Dtos;
using System;
using System.Collections.Generic;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class HierarchyComponentResultRequestDto : OptionalPagedAndSortedResultRequestDto
    {
        public Guid? HierarchyId { get; set; }

        public List<Guid>? IdList { get; set; }
        public List<int>? HierarchyComponentTypeList { get; set; }
        public string? NameContains { get; set; }
        public bool? IsActive { get; set; }
    }
}
