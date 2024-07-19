using Necnat.Abp.NnLibCommon.Dtos;
using System;
using System.Collections.Generic;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class HierarchyComponentTypeResultRequestDto
    {
        public Guid? HierarchyId { get; set; }

        public List<int>? IdList { get; set; }
    }
}
