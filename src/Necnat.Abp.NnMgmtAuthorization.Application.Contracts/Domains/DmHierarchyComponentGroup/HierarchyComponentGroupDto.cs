using Necnat.Abp.NnLibCommon.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class HierarchyComponentGroupDto : ConcurrencyEntityDto<Guid>
    {
        public string? Name { get; set; }
        public bool? IsActive { get; set; }
    }
}
