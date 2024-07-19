using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Necnat.Abp.NnLibCommon.Controllers;
using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    [RemoteService(Name = NnMgmtAuthorizationRemoteServiceConsts.RemoteServiceName)]
    [Area(NnMgmtAuthorizationRemoteServiceConsts.ModuleName)]
    [ControllerName("HierarchicalStructure")]
    [Route("api/nn-mgmt-authorization/hierarchical-structure")]
    public class HierarchicalStructureController : NecnatControllerWithoutUpdate<IHierarchicalStructureAppService, HierarchicalStructureDto, Guid, HierarchicalStructureResultRequestDto>, IHierarchicalStructureAppService
    {
        public HierarchicalStructureController(IHierarchicalStructureAppService appService) : base(appService)
        {
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public Task<HierarchicalStructureDto> UpdateAsync(Guid id, HierarchicalStructureDto input)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("hierarchical-structure-node")]
        public virtual Task<List<HierarchicalStructureNode>> GetListHierarchicalStructureNodeAsync(SearchHierarchicalStructureNodeResultRequestDto input)
        {
            return AppService.GetListHierarchicalStructureNodeAsync(input);
        }

        [HttpPost]
        [Route("get-list-my")]
        public Task<List<HS>> GetListHsAsync(List<Guid> hierarchicalStructureIdList)
        {
            return AppService.GetListHsAsync(hierarchicalStructureIdList);
        }
    }
}
