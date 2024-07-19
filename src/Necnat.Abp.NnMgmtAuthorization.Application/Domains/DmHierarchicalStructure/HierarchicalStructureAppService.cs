using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Necnat.Abp.NnLibCommon.Domains;
using Necnat.Abp.NnLibCommon.Domains.DmDistributedService;
using Necnat.Abp.NnLibCommon.Localization;
using Necnat.Abp.NnLibCommon.Services;
using Necnat.Abp.NnMgmtAuthorization.Models;
using Necnat.Abp.NnMgmtAuthorization.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Users;

namespace Necnat.Abp.NnMgmtAuthorization.Domains.DmHierarchicalStructure
{
    public class HierarchicalStructureAppService :
        NecnatAppService<
            HierarchicalStructure,
            HierarchicalStructureDto,
            Guid,
            HierarchicalStructureResultRequestDto,
            IHierarchicalStructureRepository>,
        IHierarchicalStructureAppService
    {
        protected readonly IConfiguration _configuration;
        protected readonly IDistributedServiceStore _distributedServiceStore;
        protected readonly IHierarchicalStructureStore _hierarchicalStructureStore;
        protected readonly IHttpClientFactory _httpClientFactory;        

        protected string _applicationName;
        protected const string _controllerbase = "nn-mgmt-authorization/hierarchical-access";

        public HierarchicalStructureAppService(
            ICurrentUser currentUser,
            IStringLocalizer<NnLibCommonResource> necnatLocalizer,
            IHierarchicalStructureRepository repository,

            IConfiguration configuration,
            IDistributedServiceStore distributedServiceStore,
            IHierarchicalStructureStore hierarchicalStructureStore,
            IHttpClientFactory httpClientFactory) : base(currentUser, necnatLocalizer, repository)
        {
            _configuration = configuration;
            _distributedServiceStore = distributedServiceStore;
            _hierarchicalStructureStore = hierarchicalStructureStore;
            _httpClientFactory = httpClientFactory;

            _applicationName = _configuration["DistributedService:ApplicationName"]!;

            GetPolicyName = NnMgmtAuthorizationPermissions.PrmHierarchicalStructure.Default;
            GetListPolicyName = NnMgmtAuthorizationPermissions.PrmHierarchicalStructure.Default;
            CreatePolicyName = NnMgmtAuthorizationPermissions.PrmHierarchicalStructure.Create;
            DeletePolicyName = NnMgmtAuthorizationPermissions.PrmHierarchicalStructure.Delete;
        }

        protected override async Task<IQueryable<HierarchicalStructure>> CreateFilteredQueryAsync(HierarchicalStructureResultRequestDto input)
        {
            //ThrowIfIsNotNull(HierarchicalStructureValidator.Validate(input, _necnatLocalizer));

            var q = await ReadOnlyRepository.GetQueryableAsync();

            if (input.UseParentId)
                q = q.Where(x => x.HierarchicalStructureIdParent == input.ParentId);

            if (input.HierarchyId != null)
                q = q.Where(x => x.HierarchyId == input.HierarchyId);

            return q;
        }

        [RemoteService(false)]
        public override Task<HierarchicalStructureDto> UpdateAsync(Guid id, HierarchicalStructureDto input)
        {
            return base.UpdateAsync(id, input);
        }

        public virtual async Task<List<HS>> GetListHsAsync(List<Guid> hierarchicalStructureIdList)
        {
            var l = new List<HS>();

            var distributedServiceList = await _distributedServiceStore.GetListAsync(tag: NnMgmtAuthorizationDistributedServiceConsts.HierarchicalStructureTag);
            foreach (var iDistributedService in distributedServiceList)
            {
                if (iDistributedService.ApplicationName == _applicationName)
                {
                    try
                    {
                        var allHierarchyComponentIdList = new List<Guid>();
                        foreach (var iHierarchicalStructureId in hierarchicalStructureIdList)
                        {
                            if (l.Any(x => x.Id == iHierarchicalStructureId))
                                continue;

                            var hierarchyComponentIdList = await _hierarchicalStructureStore.GetListHierarchyComponentIdRecursiveAsync(iHierarchicalStructureId);
                            allHierarchyComponentIdList.AddRange(hierarchyComponentIdList);

                            l.Add(new HS { Id = iHierarchicalStructureId, LHCId = hierarchyComponentIdList });
                        }
                    }
                    catch { }
                }
                else
                {
                    using (HttpClient client = _httpClientFactory.CreateClient(NnLibCommonDistributedServiceConsts.HttpClientName))
                    {
                        try
                        {
                            var httpResponseMessage = await client.PostAsJsonAsync($"{iDistributedService.Url}/api/{_controllerbase}/get-list-hs", hierarchicalStructureIdList);
                            if (httpResponseMessage.IsSuccessStatusCode)
                                l.AddRange(JsonSerializer.Deserialize<List<HS>>(await httpResponseMessage.Content.ReadAsStringAsync())!);
                        }
                        catch { }
                    }
                }
            }

            return l;
        }

        #region GetListHierarchicalStructureNode

        [HttpPost]
        public virtual async Task<List<HierarchicalStructureNode>> GetListHierarchicalStructureNodeAsync(SearchHierarchicalStructureNodeResultRequestDto input)
        {
            await CheckGetListPolicyAsync();
            input = await BeforeGetListHierarchicalStructureNodeAsync(input);

            var lHierarchicalStructure = await Repository.SearchByHierarchyIdAndHierarchicalStructureIdParentAsync(input.HierarchyId, input.HierarchicalStructureIdParent);

            var l = new List<HierarchicalStructureNode>();
            foreach (var iHierarchicalStructure in lHierarchicalStructure)
            {
                var hierarchicalStructure = MapToGetOutputDto(iHierarchicalStructure);


                l.Add(new HierarchicalStructureNode
                {
                    HierarchicalStructure = hierarchicalStructure,
                    HasChild = await Repository.AnyByHierarchyIdAndHierarchicalStructureIdParentAsync(input.HierarchyId, iHierarchicalStructure.Id)
                });
            }


            l = await AfterGetListHierarchicalStructureNodeAsync(l);
            return l;
        }

        protected virtual Task<SearchHierarchicalStructureNodeResultRequestDto> BeforeGetListHierarchicalStructureNodeAsync(SearchHierarchicalStructureNodeResultRequestDto input)
        {
            return Task.FromResult(input);
        }

        protected virtual Task<List<HierarchicalStructureNode>> AfterGetListHierarchicalStructureNodeAsync(List<HierarchicalStructureNode> entityList)
        {
            return Task.FromResult(entityList);
        }

        #endregion
    }
}
