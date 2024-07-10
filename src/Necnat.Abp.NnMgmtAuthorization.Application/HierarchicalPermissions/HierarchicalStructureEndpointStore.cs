using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Necnat.Abp.NnLibCommon.Domains;
using Necnat.Abp.NnMgmtAuthorization.Domains;
using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Caching;

namespace Necnat.Abp.NnMgmtAuthorization.HierarchicalPermissions
{
    public class HierarchicalStructureEndpointStore : HierarchicalStructureStore, IHierarchicalStructureStore
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly INecnatEndpointStore _necnatEndpointStore;

        public HierarchicalStructureEndpointStore(
            IDistributedCache<HierarchicalStructureRecursiveCacheItem> hierarchicalStructureRecursiveCache,
            IHierarchicalStructureRepository hierarchicalStructureRepository,
            IHttpContextAccessor httpContextAccessor,
            INecnatEndpointStore necnatEndpointStore) : base(hierarchicalStructureRecursiveCache, hierarchicalStructureRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _necnatEndpointStore = necnatEndpointStore;
        }

        protected override async Task<HierarchicalStructureRecursiveCacheItem> GetDataAsync(Guid hierarchicalStructureId)
        {
            var authServerEndpoint = await _necnatEndpointStore.FindAuthServerEndpointAsync();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _httpContextAccessor.HttpContext.GetTokenAsync("access_token")}");
                var httpResponseMessage = await client.PostAsJsonAsync($"{authServerEndpoint}/api/app/mgmt-authorization/get-list-hierarchy-component-id-recursive", hierarchicalStructureId);
                if (!httpResponseMessage.IsSuccessStatusCode)
                    throw new Exception(await httpResponseMessage.Content.ReadAsStringAsync());

                var hs = new HS { Id = hierarchicalStructureId };
                hs.LHCId = JsonSerializer.Deserialize<List<Guid>>(await httpResponseMessage.Content.ReadAsStringAsync())!;

                return new HierarchicalStructureRecursiveCacheItem { HS = hs };
            }
        }
    }
}
