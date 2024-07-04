using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using Volo.Abp.Application.Dtos;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class HierarchyComponentDto : HierarchyComponentModel, IEntityDto<Guid>
    {
        public new Guid Id { get; set; }
    }
}
