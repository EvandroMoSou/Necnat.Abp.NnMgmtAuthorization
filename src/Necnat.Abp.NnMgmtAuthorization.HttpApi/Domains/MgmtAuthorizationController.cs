using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    [RemoteService(Name = NnMgmtAuthorizationRemoteServiceConsts.RemoteServiceName)]
    [Area(NnMgmtAuthorizationRemoteServiceConsts.ModuleName)]
    [ControllerName("MgmtAuthorization")]
    [Route("api/nn-mgmt-authorization/mgmt-authorization")]
    public class MgmtAuthorizationController : IMgmtAuthorizationAppService
    {
        protected IMgmtAuthorizationAppService AppService { get; }

        public MgmtAuthorizationController(IMgmtAuthorizationAppService appService)
        {
            AppService = appService;
        }

        [HttpGet]
        [Route("hierarchical-authorization-my")]
        public Task<HierarchicalAuthorizationModel> GetHierarchicalAuthorizationMyAsync()
        {
            return AppService.GetHierarchicalAuthorizationMyAsync();
        }

        [HttpGet]
        [Route("user-authz-info-my")]
        public Task<HierarchicalAuthorizationModel> GetUserAuthzInfoMyAsync()
        {
            return AppService.GetUserAuthzInfoMyAsync();
        }

        [HttpPost]
        [Route("get-hierarchy-authz-info")]
        public Task<HierarchicalAuthorizationModel> GetHierarchyAuthzInfoAsync(List<Guid> hierarchicalStructureIdList)
        {
            return AppService.GetHierarchyAuthzInfoAsync(hierarchicalStructureIdList);
        }
    }
}
