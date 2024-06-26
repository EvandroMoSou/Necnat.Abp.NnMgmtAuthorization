using System.Collections.Generic;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class HierarchicalStructureNode
    {
        public HierarchicalStructureDto HierarchicalStructure { get; set; }
        public bool HasChild { get; set; }
        public List<HierarchicalStructureNode> Children { get; set; }
        public bool Expanded { get; set; }

        public HierarchicalStructureNode()
        {
            HierarchicalStructure = new HierarchicalStructureDto();
            Children = new List<HierarchicalStructureNode>();
        }

        public HierarchicalStructureNode(HierarchicalStructureDto hierarchicalStructure, bool? hasChild = null)
        {
            HierarchicalStructure = hierarchicalStructure;
            if (hasChild != null)
                HasChild = (bool)hasChild;
            Children = new List<HierarchicalStructureNode>();
        }
    }
}
