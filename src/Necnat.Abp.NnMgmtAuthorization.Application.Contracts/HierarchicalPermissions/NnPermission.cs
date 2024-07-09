using System;

namespace Necnat.Abp.NnMgmtAuthorization.HierarchicalPermissions
{
    public class NnPermission
    {
        public string Name { get; set; }
        public Guid? HierarchyComponentId { get; set; }

        public NnPermission()
        {
            Name = string.Empty;
        }

        public NnPermission(string name, Guid? hierarchyComponentId = null)
        {
            Name = name;
            HierarchyComponentId = hierarchyComponentId;
        }
    }
}
