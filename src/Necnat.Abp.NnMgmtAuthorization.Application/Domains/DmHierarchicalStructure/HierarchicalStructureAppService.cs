using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Necnat.Abp.NnLibCommon.Domains;
using Necnat.Abp.NnLibCommon.Localization;
using Necnat.Abp.NnLibCommon.Services;
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
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly INecnatEndpointStore _necnatEndpointStore;

        public HierarchicalStructureAppService(
            ICurrentUser currentUser,
            IStringLocalizer<NnLibCommonResource> necnatLocalizer,
            IHierarchicalStructureRepository repository,
            IHierarchyComponentGroupRepository hierarchyComponentGroupRepository,
            IHierarchyRepository hierarchyRepository,
            IHttpContextAccessor httpContextAccessor,
            INecnatEndpointStore necnatEndpointStore) : base(currentUser, necnatLocalizer, repository)
        {
            _hierarchyComponentGroupRepository = hierarchyComponentGroupRepository;
            _hierarchyRepository = hierarchyRepository;
            _httpContextAccessor = httpContextAccessor;
            _necnatEndpointStore = necnatEndpointStore;

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

        public virtual async Task<List<HierarchicalStructureNode>> GetListHierarchicalStructureNodeAsync(SearchHierarchicalStructureNodeResultRequestDto input)
        {
            await CheckPolicyAsync(GetListPolicyName);

            var lHierarchicalStructure = await Repository.SearchByHierarchyIdAndHierarchicalStructureIdParentAsync(input.HierarchyId, input.HierarchicalStructureIdParent);

            var l = new List<HierarchicalStructureNode>();
            foreach (var iHierarchicalStructure in lHierarchicalStructure)
                l.Add(new HierarchicalStructureNode
                {
                    HierarchicalStructure = MapToGetOutputDto(iHierarchicalStructure),
                    HasChild = await Repository.AnyByHierarchyIdAndHierarchicalStructureIdParentAsync(input.HierarchyId, iHierarchicalStructure.Id)
                });

            return l;
        }

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

            foreach (var iEndpoint in await _necnatEndpointStore.GetListHierarchyComponentEndpointAsync())
                if (hierarchyComponentTypeList == null || hierarchyComponentTypeList.Contains(iEndpoint.Key))
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _httpContextAccessor.HttpContext.GetTokenAsync("access_token")}");
                        try
                        {
                            var httpResponseMessage = await client.GetAsync($"{iEndpoint.Value}/api/app/hierarchy-component/hierarchy-component-contributor?hierarchyComponentTypeId={iEndpoint.Key}");
                            if (httpResponseMessage.IsSuccessStatusCode)
                                l.AddRange(JsonSerializer.Deserialize<List<HierarchyComponentModel>>(await httpResponseMessage.Content.ReadAsStringAsync())!);
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

            foreach (var iEndpoint in await _necnatEndpointStore.GetListHierarchyComponentEndpointAsync())
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _httpContextAccessor.HttpContext.GetTokenAsync("access_token")}");
                    var httpResponseMessage = await client.GetAsync($"{iEndpoint.Value}/api/app/hierarchy-component/hierarchy-component-type-contributor?hierarchyComponentTypeId={iEndpoint.Key}");
                    if (!httpResponseMessage.IsSuccessStatusCode)
                        throw new Exception(await httpResponseMessage.Content.ReadAsStringAsync());

                    l.Add(JsonSerializer.Deserialize<HierarchyComponentTypeModel>(await httpResponseMessage.Content.ReadAsStringAsync())!);
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
