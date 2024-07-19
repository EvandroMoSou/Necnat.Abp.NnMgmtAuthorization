//using Necnat.Abp.NnLibCommon.Domains;
//using Necnat.Abp.NnMgmtAuthorization.Domains;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Net.Http.Json;
//using System.Text.Json;
//using System.Threading.Tasks;
//using Volo.Abp.Caching;

//namespace Necnat.Abp.NnMgmtAuthorization.HierarchicalPermissions
//{
//    //ETODO - EndpointManager para chamar API so se necessario.
//    public class HierarchicalStructureEndpointStore : HierarchicalStructureStore, IHierarchicalStructureStore
//    {
//        protected readonly IHttpClientFactory _httpClientFactory;
//        protected readonly IDistributedServiceStore _distributedServiceStore;

//        public HierarchicalStructureEndpointStore(
//            IDistributedCache<HierarchyComponentIdRecursiveCacheItem> hierarchyComponentIdRecursiveCache,
//            IDistributedCache<HierarchicalStructureIdRecursiveCacheItem> hierarchicalStructureIdRecursiveCache,
//            IHierarchicalStructureRepository hierarchicalStructureRepository,
//            IHttpClientFactory httpClientFactory,
//            IDistributedServiceStore distributedServiceStore) : base(hierarchyComponentIdRecursiveCache, hierarchicalStructureIdRecursiveCache, hierarchicalStructureRepository)
//        {
//            _httpClientFactory = httpClientFactory;
//            _distributedServiceStore = distributedServiceStore;
//        }

//        protected override async Task<HierarchyComponentIdRecursiveCacheItem> GetHierarchyComponentIdRecursiveData(Guid hierarchicalStructureId)
//        {
//            var distributedServiceList = await _distributedServiceStore.GetListAsync(tag: NnMgmtAuthorizationDistributedServiceConsts.HierarchicalStructureTag);
//            var endpoint = endpointList.First();

//            using (HttpClient client = _httpClientFactory.CreateClient(NnMgmtAuthorizationConsts.HttpClientName))
//            {
//                var httpResponseMessage = await client.PostAsJsonAsync($"{endpoint.UrlUri}/api/app/mgmt-authorization/get-list-hierarchy-component-id-recursive", hierarchicalStructureId);
//                if (!httpResponseMessage.IsSuccessStatusCode)
//                    throw new Exception(await httpResponseMessage.Content.ReadAsStringAsync());

//                return new HierarchyComponentIdRecursiveCacheItem { HierarchyComponentIdList = JsonSerializer.Deserialize<List<Guid>>(await httpResponseMessage.Content.ReadAsStringAsync())! };
//            }
//        }
//    }
//}
