using System;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace Necnat.Abp.NnMgmtAuthorization.Models
{
    public class HierarchyComponentModel
    {
        public Guid? Id { get; set; }
        public int? HierarchyComponentType { get; set; }
        public string? Name { get; set; }
    }
}
