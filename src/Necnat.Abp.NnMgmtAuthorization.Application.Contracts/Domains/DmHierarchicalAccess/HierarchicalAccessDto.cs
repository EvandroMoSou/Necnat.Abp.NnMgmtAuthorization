using Necnat.Abp.NnLibCommon.Dtos;
using System;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class HierarchicalAccessDto : ConcurrencyEntityDto<Guid>, IDistributedServiceDto
    {
        public Guid? UserId { get; set; }
        public Guid? RoleId { get; set; }
        public string? RoleName { get; set; }
        public Guid? HierarchicalStructureId { get; set; }
        public string? HierarchyComponentName { get; set; }

        public string? DistributedAppName { get; set; }
    }
}
