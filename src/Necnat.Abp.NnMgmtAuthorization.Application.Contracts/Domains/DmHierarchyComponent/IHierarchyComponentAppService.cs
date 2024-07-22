using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public interface IHierarchyComponentAppService : IApplicationService, IRemoteService
    {
        Task<HierarchyComponentDto> GetAsync(Guid id, int? hierarchyComponentType = null);
        Task<PagedResultDto<HierarchyComponentDto>> GetListAsync(HierarchyComponentResultRequestDto input);
        Task<HierarchyComponentTypeDto> GetTypeAsync(int id);
        Task<List<HierarchyComponentTypeDto>> GetListTypeAsync(HierarchyComponentTypeResultRequestDto input);
    }
}
