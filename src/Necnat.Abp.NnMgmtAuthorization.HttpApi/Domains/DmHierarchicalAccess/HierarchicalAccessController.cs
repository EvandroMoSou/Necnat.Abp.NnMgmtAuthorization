using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Necnat.Abp.NnLibCommon.Controllers;
using System;
using System.Threading.Tasks;
using Volo.Abp;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    [RemoteService(Name = NnMgmtAuthorizationRemoteServiceConsts.RemoteServiceName)]
    [Area(NnMgmtAuthorizationRemoteServiceConsts.ModuleName)]
    [ControllerName("HierarchicalAccess")]
    [Route("api/nn-mgmt-authorization/hierarchical-access")]
    public class HierarchicalAccessController : NecnatControllerWithoutUpdate<IHierarchicalAccessAppService, HierarchicalAccessDto, Guid, HierarchicalAccessResultRequestDto>, IHierarchicalAccessAppService
    {
        public HierarchicalAccessController(IHierarchicalAccessAppService appService) : base(appService)
        {
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public Task<HierarchicalAccessDto> UpdateAsync(Guid id, HierarchicalAccessDto input)
        {
            throw new NotImplementedException();
        }
    }
}
