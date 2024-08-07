﻿using Necnat.Abp.NnLibCommon.Dtos;
using System;
using System.Collections.Generic;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class HierarchyComponentGroupResultRequestDto : OptionalPagedAndSortedResultRequestDto
    {
        public List<Guid>? IdList { get; set; }
        public string? NameContains { get; set; }
        public bool? IsActive { get; set; }
    }
}
