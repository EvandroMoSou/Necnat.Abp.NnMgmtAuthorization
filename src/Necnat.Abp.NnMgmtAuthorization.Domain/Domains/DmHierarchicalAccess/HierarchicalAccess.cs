using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class HierarchicalAccess : AuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        public virtual Guid UserId { get; set; }
        public virtual Guid RoleId { get; set; }
        public virtual Guid? HierarchicalStructureId { get; set; }
    }
}