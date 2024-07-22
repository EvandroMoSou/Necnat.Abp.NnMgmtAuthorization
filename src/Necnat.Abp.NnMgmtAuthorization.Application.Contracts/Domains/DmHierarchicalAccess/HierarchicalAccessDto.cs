using Necnat.Abp.NnLibCommon.Dtos;
using System;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class HierarchicalAccessDto : ConcurrencyEntityDto<Guid>, IDistributedServiceDto
    {
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public Guid? RoleId { get; set; }
        public string? RoleName { get; set; }
        public Guid? HierarchicalStructureId { get; set; }
        public Guid? HierarchyComponentId { get; set; }

        public string? DistributedAppName { get; set; }
    }
}
