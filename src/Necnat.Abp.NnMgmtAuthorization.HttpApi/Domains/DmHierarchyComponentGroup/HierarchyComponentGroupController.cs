using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using Volo.Abp;
using Necnat.Abp.NnLibCommon.Controllers;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    [RemoteService(Name = NnMgmtAuthorizationRemoteServiceConsts.RemoteServiceName)]
    [Area(NnMgmtAuthorizationRemoteServiceConsts.ModuleName)]
    [ControllerName("HierarchyComponentGroup")]
    [Route("api/nn-mgmt-authorization/hierarchy-component-group")]
    public class HierarchyComponentGroupController : NecnatController<IHierarchyComponentGroupAppService, HierarchyComponentGroupDto, Guid, HierarchyComponentGroupResultRequestDto>, IHierarchyComponentGroupAppService
    {
        public HierarchyComponentGroupController(IHierarchyComponentGroupAppService appService) : base(appService)
        {
        }
    }
}
