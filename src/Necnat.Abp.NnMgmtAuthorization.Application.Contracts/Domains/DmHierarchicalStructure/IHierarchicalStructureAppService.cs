using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public interface IHierarchicalStructureAppService :
        ICrudAppService<
            HierarchicalStructureDto,
            Guid,
            HierarchicalStructureResultRequestDto>
    {
        Task<List<HS>> GetListHsAsync(List<Guid> hierarchicalStructureIdList);
        Task<List<HierarchicalStructureNode>> GetListHierarchicalStructureNodeAsync(SearchHierarchicalStructureNodeResultRequestDto input);
    }
}
