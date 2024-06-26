using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class Hierarchy : AuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        public virtual string Name { get; set; } = string.Empty;
        public virtual bool IsActive { get; set; }
    }
}
