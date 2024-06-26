using System;
using System.Collections.Generic;
using System.Text;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class SearchHierarchicalStructureNodeResultRequestDto
    {
        public Guid HierarchyId { get; set; }
        public Guid? HierarchicalStructureIdParent { get; set; }
    }
}
