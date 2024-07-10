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
    [Route("api/NnMgmtAuthorization/MgmtAuthorization")]
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
        [Route("authorization-info-one-my")]
        public Task<HierarchicalAuthorizationModel> GetAuthorizationInfoOneMyAsync()
        {
            return AppService.GetAuthorizationInfoOneMyAsync();
        }

        [HttpPost]
        [Route("get-authorization-info-two")]
        public Task<HierarchicalAuthorizationModel> GetAuthorizationInfoTwoAsync(List<Guid> hierarchicalStructureIdList)
        {
            return AppService.GetAuthorizationInfoTwoAsync(hierarchicalStructureIdList);
        }
    }
}
