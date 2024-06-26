using Necnat.Abp.NnLibCommon.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class HierarchyResultRequestDto : OptionalPagedAndSortedResultRequestDto
    {
        public string? NameContains { get; set; }
        public bool? IsActive { get; set; }
    }
}
