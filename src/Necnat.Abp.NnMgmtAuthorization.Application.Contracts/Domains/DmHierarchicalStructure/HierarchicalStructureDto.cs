using Necnat.Abp.NnLibCommon.Dtos;
using System;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class HierarchicalStructureDto : ConcurrencyEntityDto<Guid>
    {
        public Guid? HierarchicalStructureIdParent { get; set; }
        public Guid? HierarchyId { get; set; }
        public int? HierarchyComponentType { get; set; }
        public Guid? HierarchyComponentId { get; set; }
        public string? HierarchyComponentName { get; set; }
    }
}
