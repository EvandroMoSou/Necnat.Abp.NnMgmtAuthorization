using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class AuthEndpoint : AuditedAggregateRoot<Guid>
    {
        public string DisplayName { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public bool IsAuthentication { get; set; }
        public bool IsActive { get; set; }
    }
}
