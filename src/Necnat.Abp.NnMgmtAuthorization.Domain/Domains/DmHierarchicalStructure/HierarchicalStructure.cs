using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class HierarchicalStructure : AuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        public virtual Guid? HierarchicalStructureIdParent { get; set; }
        public virtual HierarchicalStructure? HierarchicalStructureParent { get; set; }
        public virtual Guid HierarchyId { get; set; }
        public virtual Hierarchy? Hierarchy { get; set; }
        public virtual int HierarchyComponentType { get; set; }
        public virtual Guid HierarchyComponentId { get; set; }
    }
}
