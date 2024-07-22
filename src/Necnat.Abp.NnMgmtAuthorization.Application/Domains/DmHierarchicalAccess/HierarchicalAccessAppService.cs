using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Necnat.Abp.NnLibCommon.Domains;
using Necnat.Abp.NnLibCommon.Domains.DmDistributedService;
using Necnat.Abp.NnLibCommon.Domains.NnIdentity;
using Necnat.Abp.NnLibCommon.Localization;
using Necnat.Abp.NnLibCommon.Services;
using Necnat.Abp.NnMgmtAuthorization.HierarchicalPermissions;
using Necnat.Abp.NnMgmtAuthorization.Models;
using Necnat.Abp.NnMgmtAuthorization.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Users;

namespace Necnat.Abp.NnMgmtAuthorization.Domains.DmHierarchicalAccess
{
    public class HierarchicalAccessAppService :
        NecnatAppService<
            HierarchicalAccess,
            HierarchicalAccessDto,
            Guid,
            HierarchicalAccessResultRequestDto,
            IHierarchicalAccessRepository>,
        IHierarchicalAccessAppService
    {
        protected readonly IConfiguration _configuration;
        protected readonly IDistributedServiceStore _distributedServiceStore;
        protected readonly IHierarchicalAuthorizationService _hierarchicalAuthorizationService;
        protected readonly IHierarchicalStructureAppService _hierarchicalStructureAppService;
        protected readonly IHierarchicalStructureStore _hierarchicalStructureStore;
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly INnIdentityUserAppService _nnIdentityUserAppService;
        protected readonly INnRoleStore _nnRoleStore;

        protected string _applicationName;
        protected const string _controllerbase = "nn-mgmt-authorization/hierarchical-access";

        public HierarchicalAccessAppService(
            ICurrentUser currentUser,
            IStringLocalizer<NnLibCommonResource> necnatLocalizer,
            IHierarchicalAccessRepository repository,

            IConfiguration configuration,
            IDistributedServiceStore distributedServiceStore,
            IHierarchicalAuthorizationService hierarchicalAuthorizationService,
            IHierarchicalStructureAppService hierarchicalStructureAppService,
            IHierarchicalStructureStore hierarchicalStructureStore,
            IHttpClientFactory httpClientFactory,
            INnIdentityUserAppService nnIdentityUserAppService,
            INnRoleStore nnRoleStore) : base(currentUser, necnatLocalizer, repository)
        {
            _configuration = configuration;
            _distributedServiceStore = distributedServiceStore;
            _hierarchicalAuthorizationService = hierarchicalAuthorizationService;
            _hierarchicalStructureAppService = hierarchicalStructureAppService;
            _hierarchicalStructureStore = hierarchicalStructureStore;
            _httpClientFactory = httpClientFactory;
            _nnIdentityUserAppService = nnIdentityUserAppService;
            _nnRoleStore = nnRoleStore;

            _applicationName = _configuration["DistributedService:ApplicationName"]!;

            GetPolicyName = NnMgmtAuthorizationPermissions.PrmHierarchicalAccess.Default;
            GetListPolicyName = NnMgmtAuthorizationPermissions.PrmHierarchicalAccess.Default;
            CreatePolicyName = NnMgmtAuthorizationPermissions.PrmHierarchicalAccess.Create;
            DeletePolicyName = NnMgmtAuthorizationPermissions.PrmHierarchicalAccess.Delete;
        }

        protected override async Task<IQueryable<HierarchicalAccess>> CreateFilteredQueryAsync(HierarchicalAccessResultRequestDto input)
        {
            //ThrowIfIsNotNull(HierarchicalAccessValidator.Validate(input, _necnatLocalizer));

            var q = await ReadOnlyRepository.GetQueryableAsync();

            if (input.UserId != null)
                q = q.Where(x => x.UserId == input.UserId);

            if (input.RoleId != null)
                q = q.Where(x => x.RoleId == input.RoleId);

            var lHierarchicalStructureId = await _hierarchicalAuthorizationService.GetListHierarchicalStructureIdAsync(GetListPolicyName!);
            var lHierarchicalStructureIdRecursive = await _hierarchicalStructureStore.GetListHierarchicalStructureIdRecursiveAsync(lHierarchicalStructureId);

            if (input.HierarchicalStructureIdList != null)
            {
                var l = new List<Guid>();
                if (input.WithHierarchy == true)
                    l = await _hierarchicalStructureStore.GetListHierarchicalStructureIdRecursiveAsync(input.HierarchicalStructureIdList!);
                else
                    l = input.HierarchicalStructureIdList;

                lHierarchicalStructureIdRecursive = lHierarchicalStructureIdRecursive.Where(x => l.Contains(x)).ToList();
            }

            q = q.Where(x => lHierarchicalStructureIdRecursive.Contains(x.HierarchicalStructureId));

            return q;
        }

        public override async Task<HierarchicalAccessDto> GetAsync(Guid id)
        {
            var distributedServiceList = await _distributedServiceStore.GetListAsync(tag: NnMgmtAuthorizationDistributedServiceConsts.HierarchicalAccessTag);
            foreach (var iDistributedService in distributedServiceList)
            {
                if (iDistributedService.ApplicationName == _applicationName)
                {
                    try
                    {
                        var result = await base.GetAsync(id);

                        var lHierarchicalStructureId = await _hierarchicalAuthorizationService.GetListHierarchicalStructureIdAsync(GetPolicyName!);
                        if (!await _hierarchicalStructureStore.HasHierarchicalStructureIdRecursiveAsync(lHierarchicalStructureId, (Guid)result.HierarchicalStructureId!))
                            throw new UnauthorizedAccessException($"[HierarchicalStructureId] {result.HierarchicalStructureId}");

                        result.DistributedAppName = _applicationName;
                        return result;
                    }
                    catch { }
                }
                else
                {
                    using (HttpClient client = _httpClientFactory.CreateClient(NnMgmtAuthorizationDistributedServiceConsts.HierarchicalAccessTag))
                    {
                        try
                        {
                            var httpResponseMessage = await client.GetAsync($"{iDistributedService.Url}/api/{_controllerbase}/{id}");
                            if (httpResponseMessage.IsSuccessStatusCode)
                                return JsonSerializer.Deserialize<HierarchicalAccessDto>(await httpResponseMessage.Content.ReadAsStringAsync())!;
                        }
                        catch { }
                    }
                }
            }

            throw new EntityNotFoundException(typeof(HierarchicalAccessDto), id);
        }

        public override async Task<PagedResultDto<HierarchicalAccessDto>> GetListAsync(HierarchicalAccessResultRequestDto input)
        {
            var l = new List<PagedResultDto<HierarchicalAccessDto>>();

            var distributedServiceList = await _distributedServiceStore.GetListAsync(tag: NnMgmtAuthorizationDistributedServiceConsts.HierarchicalAccessTag);
            foreach (var iDistributedService in distributedServiceList)
            {
                if (iDistributedService.ApplicationName == _applicationName)
                {
                    try
                    {
                        var result = await base.GetListAsync(input);
                        var items = result.Items.ToList();
                        items.ForEach(x => x.DistributedAppName = _applicationName);
                        l.Add(new PagedResultDto<HierarchicalAccessDto>(result.TotalCount, items));
                    }
                    catch { }
                }
                else
                {
                    using (HttpClient client = _httpClientFactory.CreateClient(NnLibCommonDistributedServiceConsts.HttpClientName))
                    {
                        try
                        {
                            var httpResponseMessage = await client.PostAsJsonAsync($"{iDistributedService.Url}/api/{_controllerbase}/get-list", input);
                            if (httpResponseMessage.IsSuccessStatusCode)
                                l.Add(JsonSerializer.Deserialize<PagedResultDto<HierarchicalAccessDto>>(await httpResponseMessage.Content.ReadAsStringAsync())!);
                        }
                        catch { }
                    }
                }
            }

            return new PagedResultDto<HierarchicalAccessDto>(l.Sum(x => x.TotalCount), l.SelectMany(x => x.Items).ToList());
        }

        protected override async Task<List<HierarchicalAccessDto>> AfterGetListAsync(List<HierarchicalAccessDto> entityDtoList)
        {
            if (entityDtoList.Count < 1)
                return entityDtoList;

            var userList = await _nnIdentityUserAppService.GetListAsync(new NnIdentityUserResultRequestDto { IdList = entityDtoList.Select(x => (Guid)x.UserId!).ToList(), IsPaged = false });
            var hierarchicalStructureList = await _hierarchicalStructureAppService.GetListAsync(new HierarchicalStructureResultRequestDto { IdList = entityDtoList.Select(x => (Guid)x.HierarchicalStructureId!).ToList(), IsPaged = false });

            foreach (var iEntityDto in entityDtoList)
            {
                iEntityDto.UserName = userList.Items.First(x => x.Id == (Guid)iEntityDto.UserId!).Name;
                iEntityDto.RoleName = await _nnRoleStore.GetNameByIdAsync((Guid)iEntityDto.RoleId!);
                iEntityDto.HierarchyComponentId = hierarchicalStructureList.Items.First(x => x.Id == (Guid)iEntityDto.HierarchicalStructureId!).HierarchyComponentId;
            }

            return entityDtoList;
        }

        public virtual async Task<List<HA>> GetListHaMyAsync()
        {
            var l = new List<HA>();

            var distributedServiceList = await _distributedServiceStore.GetListAsync(tag: NnMgmtAuthorizationDistributedServiceConsts.HierarchicalAccessTag);
            foreach (var iDistributedService in distributedServiceList)
            {
                if (iDistributedService.ApplicationName == _applicationName)
                {
                    try
                    {
                        var hierarchicalAccessList = await Repository.GetListByUserIdAsync((Guid)CurrentUser.Id!);
                        foreach (var iHierarchicalAccessList in hierarchicalAccessList)
                        {
                            var ha = l.Where(x => x.RId == iHierarchicalAccessList.RoleId).FirstOrDefault();
                            if (ha == null)
                                l.Add(new HA { RId = iHierarchicalAccessList.RoleId, LHSId = new List<Guid> { iHierarchicalAccessList.HierarchicalStructureId } });
                            else
                                ha.LHSId.Add(iHierarchicalAccessList.HierarchicalStructureId);
                        }

                        foreach (var iE in l)
                            iE.LPN = await _nnRoleStore.GetPermissionListByIdAsync(iE.RId);
                    }
                    catch { }
                }
                else
                {
                    using (HttpClient client = _httpClientFactory.CreateClient(NnLibCommonDistributedServiceConsts.HttpClientName))
                    {
                        try
                        {
                            var httpResponseMessage = await client.GetAsync($"{iDistributedService.Url}/api/{_controllerbase}/get-list-ha-my");
                            if (httpResponseMessage.IsSuccessStatusCode)
                                l.AddRange(JsonSerializer.Deserialize<List<HA>>(await httpResponseMessage.Content.ReadAsStringAsync())!);
                        }
                        catch { }
                    }
                }
            }

            return l;
        }

        public override async Task<HierarchicalAccessDto> CreateAsync(HierarchicalAccessDto input)
        {
            var distributedServiceList = await _distributedServiceStore.GetListAsync(tag: NnMgmtAuthorizationDistributedServiceConsts.HierarchicalAccessTag);
            foreach (var iDistributedService in distributedServiceList)
            {
                if (iDistributedService.ApplicationName == _applicationName)
                {
                    try
                    {
                        var lHierarchicalStructureId = await _hierarchicalAuthorizationService.GetListHierarchicalStructureIdAsync(GetPolicyName!);
                        if (!await _hierarchicalStructureStore.HasHierarchicalStructureIdRecursiveAsync(lHierarchicalStructureId, (Guid)input.HierarchicalStructureId!))
                            throw new UnauthorizedAccessException($"[HierarchicalStructureId] {input.HierarchicalStructureId}");

                        var result = await base.CreateAsync(input);
                        result.DistributedAppName = _applicationName;
                        return result;
                    }
                    catch { }
                }
                else
                {
                    using (HttpClient client = _httpClientFactory.CreateClient(NnMgmtAuthorizationDistributedServiceConsts.HierarchicalAccessTag))
                    {
                        try
                        {
                            var httpResponseMessage = await client.PostAsJsonAsync($"{iDistributedService.Url}/api/{_controllerbase}", input);
                            if (httpResponseMessage.IsSuccessStatusCode)
                                return JsonSerializer.Deserialize<HierarchicalAccessDto>(await httpResponseMessage.Content.ReadAsStringAsync())!;
                        }
                        catch { }
                    }
                }
            }

            throw new ArgumentException($"[DistributedAppName] {input.DistributedAppName}");
        }

        [RemoteService(false)]
        public override Task<HierarchicalAccessDto> UpdateAsync(Guid id, HierarchicalAccessDto input)
        {
            throw new NotImplementedException();
        }

        public override async Task DeleteAsync(Guid id)
        {
            var distributedServiceList = await _distributedServiceStore.GetListAsync(tag: NnMgmtAuthorizationDistributedServiceConsts.HierarchicalAccessTag);
            foreach (var iDistributedService in distributedServiceList)
            {
                if (iDistributedService.ApplicationName == _applicationName)
                {
                    try
                    {
                        var result = await Repository.GetAsync(id);

                        var lHierarchicalStructureId = await _hierarchicalAuthorizationService.GetListHierarchicalStructureIdAsync(GetPolicyName!);
                        if (!await _hierarchicalStructureStore.HasHierarchicalStructureIdRecursiveAsync(lHierarchicalStructureId, (Guid)result.HierarchicalStructureId!))
                            throw new UnauthorizedAccessException($"[HierarchicalStructureId] {result.HierarchicalStructureId}");

                        await Repository.DeleteAsync(result);
                        return;
                    }
                    catch { }
                }
                else
                {
                    using (HttpClient client = _httpClientFactory.CreateClient(NnMgmtAuthorizationDistributedServiceConsts.HierarchicalAccessTag))
                    {
                        try
                        {
                            var httpResponseMessage = await client.DeleteAsync($"{iDistributedService.Url}/api/{_controllerbase}/{id}");
                            if (httpResponseMessage.IsSuccessStatusCode)
                                return;
                        }
                        catch { }
                    }
                }
            }

            throw new ArgumentException();
        }
    }
}