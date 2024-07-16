using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Necnat.Abp.NnLibCommon.Domains;
using Necnat.Abp.NnLibCommon.Localization;
using Necnat.Abp.NnLibCommon.Services;
using Necnat.Abp.NnMgmtAuthorization.Debug;
using Necnat.Abp.NnMgmtAuthorization.Models;
using Necnat.Abp.NnMgmtAuthorization.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
        protected readonly IHierarchyComponentGroupRepository _hierarchyComponentGroupRepository;
        protected readonly IHierarchyRepository _hierarchyRepository;
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly INnEndpointStore _nnEndpointStore;

        public HierarchicalStructureAppService(
            ICurrentUser currentUser,
            IStringLocalizer<NnLibCommonResource> necnatLocalizer,
            IHierarchicalStructureRepository repository,
            IHierarchyComponentGroupRepository hierarchyComponentGroupRepository,
            IHierarchyRepository hierarchyRepository,
            IHttpClientFactory httpClientFactory,
            INnEndpointStore nnEndpointStore) : base(currentUser, necnatLocalizer, repository)
        {
            _hierarchyComponentGroupRepository = hierarchyComponentGroupRepository;
            _hierarchyRepository = hierarchyRepository;
            _httpClientFactory = httpClientFactory;
            _nnEndpointStore = nnEndpointStore;

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

        public virtual async Task<List<HierarchyComponentModel>> GetListHierarchyComponentAsync(Guid? hierarchyId = null, List<short>? hierarchyComponentTypeList = null)
        {
            await CheckPolicyAsync(NnMgmtAuthorizationPermissions.PrmHierarchicalStructure.Default);

            var l = new List<HierarchyComponentModel>();

            if (hierarchyComponentTypeList == null || hierarchyComponentTypeList.Contains(1))
            {
                var lHierarchy = await _hierarchyRepository.GetListAsync();
                foreach (var iHierarchy in lHierarchy)
                    l.Add(new HierarchyComponentModel { HierarchyComponentType = 1, Id = iHierarchy.Id, Name = iHierarchy.Name });
            }

            if (hierarchyComponentTypeList == null || hierarchyComponentTypeList.Contains(2))
            {
                var lHierarchyComponentGroup = await _hierarchyComponentGroupRepository.GetListAsync();
                foreach (var iHierarchyComponentGroup in lHierarchyComponentGroup)
                    l.Add(new HierarchyComponentModel { HierarchyComponentType = 2, Id = iHierarchyComponentGroup.Id, Name = iHierarchyComponentGroup.Name });
            }

            var nnEndpointList = await _nnEndpointStore.GetListByTagAsync(NnMgmtAuthorizationConsts.NnEndpointTagHierarchyComponentContributor);
            foreach (var iNnEndpoint in nnEndpointList)
            {
                if (!iNnEndpoint.HasParameter())
                    continue;

                using (HttpClient client = _httpClientFactory.CreateClient(NnMgmtAuthorizationConsts.HttpClientName))
                {
                    try
                    {
                        var httpResponseMessage = await client.GetAsync(
                            iNnEndpoint.IsUrl()
                            ? $"{iNnEndpoint.UrlUri}/api/nn-mgmt-authorization/hierarchical-structure/hierarchy-component-contributor?hierarchyComponentTypeId={iNnEndpoint.GetParameter(2)}"
                            : iNnEndpoint.UrlUri);

                        if (httpResponseMessage.IsSuccessStatusCode)
                            l.AddRange(JsonSerializer.Deserialize<List<HierarchyComponentModel>>(await httpResponseMessage.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!);
                    }
                    catch { }
                }
            }

            return l;
        }

        public virtual async Task<List<HierarchyComponentTypeModel>> GetListHierarchyComponentTypeAsync(Guid? hierarchyId = null)
        {
            var l = new List<HierarchyComponentTypeModel>();

            l.Add(new HierarchyComponentTypeModel
            {
                Id = 1,
                Name = L["Hierarchy"],
                Icon = "fas fa-box"
            });

            l.Add(new HierarchyComponentTypeModel
            {
                Id = 2,
                Name = L["Hierarchy Component Group"],
                Icon = "fas fa-sitemap"
            });

            var nnEndpointList = await _nnEndpointStore.GetListByTagAsync(NnMgmtAuthorizationConsts.NnEndpointTagHierarchyComponentTypeContributor);
            foreach (var iNnEndpoint in nnEndpointList)
            {
                if (!iNnEndpoint.HasParameter())
                    continue;

                using (HttpClient client = _httpClientFactory.CreateClient(NnMgmtAuthorizationConsts.HttpClientName))
                {
                    try
                    {
                        var httpResponseMessage = await client.GetAsync(
                            iNnEndpoint.IsUrl()
                            ? $"{iNnEndpoint.UrlUri}/api/nn-mgmt-authorization/hierarchical-structure/hierarchy-component-type-contributor?hierarchyComponentTypeId={iNnEndpoint.GetParameter(2)}"
                            : iNnEndpoint.UrlUri);

                        if (httpResponseMessage.IsSuccessStatusCode)
                            l.Add(JsonSerializer.Deserialize<HierarchyComponentTypeModel>(await httpResponseMessage.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!);
                    }
                    catch { }
                }
            }

            return l;
        }

        public virtual Task<List<HierarchyComponentModel>> GetListHierarchyComponentContributorAsync(short hierarchyComponentTypeId)
        {
            throw new NotImplementedException();
        }

        public virtual Task<HierarchyComponentTypeModel> GetHierarchyComponentTypeContributorAsync(short hierarchyComponentTypeId)
        {
            throw new NotImplementedException();
        }
    }
}
