using System;

namespace Necnat.Abp.NnMgmtAuthorization.Models
{
    public class MantainHierarchicalAccessByUserIdModel
    {
        public virtual Guid RoleId { get; set; }
        public virtual string RoleName { get; set; } = string.Empty;
        public virtual Guid HierarchicalStructureId { get; set; }
    }
}
