using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Necnat.Abp.NnMgmtAuthorization.Domains.DmHierarchyComponent
{
    [RemoteService(Name = NnMgmtAuthorizationRemoteServiceConsts.RemoteServiceName)]
    [Area(NnMgmtAuthorizationRemoteServiceConsts.ModuleName)]
    [ControllerName("HierarchyComponent")]
    [Route("api/nn-mgmt-authorization/hierarchy-component")]
    public class HierarchyComponentController : IHierarchyComponentAppService
    {
        protected IHierarchyComponentAppService AppService { get; }

        public HierarchyComponentController(IHierarchyComponentAppService appService)
        {
            AppService = appService;
        }

        [HttpGet]
        [Route("{id}")]
        public Task<HierarchyComponentDto> GetAsync(Guid id, int? hierarchyComponentType = null)
        {
            return AppService.GetAsync(id, hierarchyComponentType);
        }

        [HttpPost]
        [Route("get-list")]
        public Task<PagedResultDto<HierarchyComponentDto>> GetListAsync(HierarchyComponentResultRequestDto input)
        {
            return AppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("type/{id}")]
        public Task<HierarchyComponentTypeDto> GetTypeAsync(int id)
        {
            return AppService.GetTypeAsync(id);
        }

        [HttpPost]
        [Route("get-list-type")]
        public Task<List<HierarchyComponentTypeDto>> GetListTypeAsync(HierarchyComponentTypeResultRequestDto input)
        {
            return AppService.GetListTypeAsync(input);
        }
    }
}
