using System;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class SearchHierarchicalStructureNodeResultRequestDto
    {
        public Guid HierarchyId { get; set; }
        public Guid? HierarchicalStructureIdParent { get; set; }
    }
}
