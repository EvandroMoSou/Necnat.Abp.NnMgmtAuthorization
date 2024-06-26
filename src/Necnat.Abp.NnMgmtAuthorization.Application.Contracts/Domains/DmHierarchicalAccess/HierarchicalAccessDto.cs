using Necnat.Abp.NnLibCommon.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class HierarchicalAccessDto : ConcurrencyEntityDto<Guid>
    {
        public Guid? UserId { get; set; }
        public string? UserUserName { get; set; }
        public Guid? RoleId { get; set; }
        public Guid? HierarchicalStructureId { get; set; }
    }
}
