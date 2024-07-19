using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public interface IHierarchyComponentAppService
    {
        Task<HierarchyComponentDto> GetAsync(Guid id, int? hierarchyComponentType = null);
        Task<PagedResultDto<HierarchyComponentDto>> GetListAsync(HierarchyComponentResultRequestDto input);
        Task<HierarchyComponentTypeDto> GetTypeAsync(int id);
        Task<List<HierarchyComponentTypeDto>> GetListTypeAsync(HierarchyComponentTypeResultRequestDto input);
    }
}
