using Microsoft.Extensions.Configuration;
using Necnat.Abp.NnLibCommon.Domains;
using Necnat.Abp.NnLibCommon.Domains.DmDistributedService;
using Necnat.Abp.NnLibCommon.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Necnat.Abp.NnMgmtAuthorization.Domains.DmHierarchyComponent
{
    public class HierarchyComponentAppService : NnMgmtAuthorizationAppService, IHierarchyComponentAppService
    {
        protected readonly IConfiguration _configuration;
        protected readonly IDistributedServiceStore _distributedServiceStore;
        protected readonly IHierarchyAppService _hierarchyAppService;
        protected readonly IHierarchyComponentGroupAppService _hierarchyComponentGroupAppService;
        protected readonly IHttpClientFactory _httpClientFactory;

        protected string _applicationName;
        protected const string _controllerbase = "nn-mgmt-authorization/hierarchy-component";

        public HierarchyComponentAppService(
            IConfiguration configuration,
            IDistributedServiceStore distributedServiceStore,
            IHierarchyAppService hierarchyAppService,
            IHierarchyComponentGroupAppService hierarchyComponentGroupAppService,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _distributedServiceStore = distributedServiceStore;
            _hierarchyAppService = hierarchyAppService;
            _hierarchyComponentGroupAppService = hierarchyComponentGroupAppService;
            _httpClientFactory = httpClientFactory;

            _applicationName = _configuration["DistributedService:ApplicationName"]!;
        }

        public virtual async Task<HierarchyComponentDto> GetAsync(Guid id, int? hierarchyComponentType = null)
        {
            var distributedServiceList = await _distributedServiceStore.GetListAsync(tag: NnMgmtAuthorizationDistributedServiceConsts.HierarchyComponentTag);
            foreach (var iDistributedService in distributedServiceList)
            {
                if (hierarchyComponentType != null && int.Parse(iDistributedService.GetParameter(1)) != hierarchyComponentType)
                    continue;

                if (iDistributedService.ApplicationName == _applicationName)
                {
                    try
                    {
                        var result = await GetListHierarchyComponentAsync(new HierarchyComponentResultRequestDto
                        {
                            IdList = new List<Guid> { id },
                            HierarchyComponentTypeList = hierarchyComponentType == null ? null : new List<int> { (int)hierarchyComponentType },
                            IsPaged = false
                        });

                        if (result.TotalCount > 0)
                            return result.Items.First();
                    }
                    catch { }
                }
                else
                {
                    using (HttpClient client = _httpClientFactory.CreateClient(NnLibCommonDistributedServiceConsts.HttpClientName))
                    {
                        try
                        {
                            var httpResponseMessage = await client.GetAsync($"{iDistributedService.Url}/api/{_controllerbase}/{id}{(hierarchyComponentType == null ? string.Empty : $"?hierarchyComponentType={hierarchyComponentType}")}");
                            if (httpResponseMessage.IsSuccessStatusCode)
                                return JsonSerializer.Deserialize<HierarchyComponentDto>(await httpResponseMessage.Content.ReadAsStringAsync())!;
                        }
                        catch { }
                    }
                }
            }

            throw new EntityNotFoundException(typeof(HierarchyComponentDto), id);
        }

        public virtual async Task<PagedResultDto<HierarchyComponentDto>> GetListAsync(HierarchyComponentResultRequestDto input)
        {
            var l = new List<PagedResultDto<HierarchyComponentDto>>();

            var distributedServiceList = await _distributedServiceStore.GetListAsync(tag: NnMgmtAuthorizationDistributedServiceConsts.HierarchyComponentTag);
            foreach (var iDistributedService in distributedServiceList)
            {
                if (iDistributedService.ApplicationName == _applicationName)
                {
                    try
                    {
                        l.Add(await GetListHierarchyComponentAsync(input));
                    }
                    catch { }
                }
                else
                {
                    if (input.HierarchyComponentTypeList != null && !input.HierarchyComponentTypeList.Contains(int.Parse(iDistributedService.GetParameter(1))))
                        continue;

                    using (HttpClient client = _httpClientFactory.CreateClient(NnLibCommonDistributedServiceConsts.HttpClientName))
                    {
                        try
                        {
                            var httpResponseMessage = await client.PostAsJsonAsync($"{iDistributedService.Url}/api/{_controllerbase}/get-list", input);
                            if (httpResponseMessage.IsSuccessStatusCode)
                                l.Add(JsonSerializer.Deserialize<PagedResultDto<HierarchyComponentDto>>(await httpResponseMessage.Content.ReadAsStringAsync())!);
                        }
                        catch { }
                    }
                }
            }

            return new PagedResultDto<HierarchyComponentDto>(l.Sum(x => x.TotalCount), l.SelectMany(x => x.Items).ToList());
        }

        protected virtual async Task<PagedResultDto<HierarchyComponentDto>> GetListHierarchyComponentAsync(HierarchyComponentResultRequestDto input)
        {
            var l = new List<PagedResultDto<HierarchyComponentDto>>();

            if (input.HierarchyComponentTypeList == null || input.HierarchyComponentTypeList.Contains(1))
            {
                var result = await _hierarchyAppService.GetListAsync(new HierarchyResultRequestDto
                {
                    IdList = input.IdList,
                    NameContains = input.NameContains,
                    IsActive = input.IsActive,
                    IsPaged = input.IsPaged,
                    MaxResultCount = input.MaxResultCount,
                    SkipCount = input.SkipCount,
                    Sorting = input.Sorting
                });
                l.Add(new PagedResultDto<HierarchyComponentDto>(result.TotalCount, result.Items.Select(x => new HierarchyComponentDto { HierarchyComponentType = 1, Id = x.Id, Name = x.Name }).ToList()));
            }

            if (input.HierarchyComponentTypeList == null || input.HierarchyComponentTypeList.Contains(1))
            {
                var result = await _hierarchyComponentGroupAppService.GetListAsync(new HierarchyComponentGroupResultRequestDto
                {
                    IdList = input.IdList,
                    NameContains = input.NameContains,
                    IsActive = input.IsActive,
                    IsPaged = input.IsPaged,
                    MaxResultCount = input.MaxResultCount,
                    SkipCount = input.SkipCount,
                    Sorting = input.Sorting
                });
                l.Add(new PagedResultDto<HierarchyComponentDto>(result.TotalCount, result.Items.Select(x => new HierarchyComponentDto { HierarchyComponentType = 1, Id = x.Id, Name = x.Name }).ToList()));
            }

            return new PagedResultDto<HierarchyComponentDto>(l.Sum(x => x.TotalCount), l.SelectMany(x => x.Items).ToList());
        }

        public virtual async Task<HierarchyComponentTypeDto> GetTypeAsync(int id)
        {
            var distributedServiceList = await _distributedServiceStore.GetListAsync(tag: NnMgmtAuthorizationDistributedServiceConsts.HierarchyComponentTag);
            foreach (var iDistributedService in distributedServiceList)
            {
                if (int.Parse(iDistributedService.GetParameter(1)) != id)
                    continue;

                if (iDistributedService.ApplicationName == _applicationName)
                {
                    try
                    {
                        return GetListHierarchyComponentType(new HierarchyComponentTypeResultRequestDto { IdList = new List<int> { id } }).First();
                    }
                    catch { }
                }
                else
                {
                    using (HttpClient client = _httpClientFactory.CreateClient(NnLibCommonDistributedServiceConsts.HttpClientName))
                    {
                        try
                        {
                            var httpResponseMessage = await client.GetAsync($"{iDistributedService.Url}/api/{_controllerbase}/type/{id}");
                            if (httpResponseMessage.IsSuccessStatusCode)
                                return JsonSerializer.Deserialize<HierarchyComponentTypeDto>(await httpResponseMessage.Content.ReadAsStringAsync())!;
                        }
                        catch { }
                    }
                }
            }

            throw new EntityNotFoundException(typeof(HierarchyComponentTypeDto), id);
        }

        public virtual async Task<List<HierarchyComponentTypeDto>> GetListTypeAsync(HierarchyComponentTypeResultRequestDto input)
        {
            var l = new List<HierarchyComponentTypeDto>();

            var distributedServiceList = await _distributedServiceStore.GetListAsync(tag: NnMgmtAuthorizationDistributedServiceConsts.HierarchyComponentTag);
            foreach (var iDistributedService in distributedServiceList)
            {
                if (iDistributedService.ApplicationName == _applicationName)
                {
                    try
                    {
                        l.AddRange(GetListHierarchyComponentType(input));
                    }
                    catch { }
                }
                else
                {
                    if (input.IdList != null && !input.IdList.Contains(int.Parse(iDistributedService.GetParameter(1))))
                        continue;

                    using (HttpClient client = _httpClientFactory.CreateClient(NnLibCommonDistributedServiceConsts.HttpClientName))
                    {
                        try
                        {
                            var httpResponseMessage = await client.PostAsJsonAsync($"{iDistributedService.Url}/api/{_controllerbase}/get-list-type", input);
                            if (httpResponseMessage.IsSuccessStatusCode)
                                l.AddRange(JsonSerializer.Deserialize<List<HierarchyComponentTypeDto>>(await httpResponseMessage.Content.ReadAsStringAsync())!);
                        }
                        catch { }
                    }
                }
            }

            return l;
        }

        protected virtual List<HierarchyComponentTypeDto> GetListHierarchyComponentType(HierarchyComponentTypeResultRequestDto input)
        {
            var l = new List<HierarchyComponentTypeDto>();

            if (input.IdList == null || input.IdList.Contains(1))
            {
                l.Add(new HierarchyComponentTypeDto
                {
                    Id = 1,
                    Name = L["Hierarchy"],
                    Icon = "fa-solid fa-sitemap"
                    //Icon = "fas fa-box"
                });
            }

            if (input.IdList == null || input.IdList.Contains(1))
            {
                l.Add(new HierarchyComponentTypeDto
                {
                    Id = 2,
                    Name = L["Hierarchy Component Group"],
                    Icon = "fa-solid fa-object-group"
                    //Icon = "fas fa-sitemap"
                });
            }

            return l;
        }
    }
}
