using Volo.Abp.Application.Dtos;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class HierarchyComponentTypeDto : HierarchyComponentTypeModel, IEntityDto<int>
    {
        public new int Id { get; set; }
    }
}
