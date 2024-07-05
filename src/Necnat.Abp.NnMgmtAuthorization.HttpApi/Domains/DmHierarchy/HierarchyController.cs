using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Necnat.Abp.NnLibCommon.Controllers;
using System;
using Volo.Abp;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    [RemoteService(Name = NnMgmtAuthorizationRemoteServiceConsts.RemoteServiceName)]
    [Area(NnMgmtAuthorizationRemoteServiceConsts.ModuleName)]
    [ControllerName("Hierarchy")]
    [Route("api/NnMgmtAuthorization/Hierarchy")]
    public class HierarchyController : NecnatController<IHierarchyAppService, HierarchyDto, Guid, HierarchyResultRequestDto>, IHierarchyAppService
    {
        public HierarchyController(IHierarchyAppService appService) : base(appService)
        {
        }
    }
}
