using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Necnat.Abp.NnLibCommon.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Users;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    [Authorize]
    public class MgmtAuthorizationAppService : NnMgmtAuthorizationAppService, IMgmtAuthorizationAppService
    {
        protected readonly ICurrentUser _currentUser;
        protected readonly IHierarchicalStructureRecursiveService _hierarchicalStructureRecursiveService;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IMgmtAuthorizationService _mgmtAuthorizationService;
        protected readonly INecnatEndpointStore _necnatEndpointStore;

        public MgmtAuthorizationAppService(
            ICurrentUser currentUser,
            IHierarchicalStructureRecursiveService hierarchicalStructureRecursiveService,
            IHttpContextAccessor httpContextAccessor,
            IMgmtAuthorizationService mgmtAuthorizationService,
            INecnatEndpointStore necnatEndpointStore)
        {
            _currentUser = currentUser;
            _hierarchicalStructureRecursiveService = hierarchicalStructureRecursiveService;
            _httpContextAccessor = httpContextAccessor;
            _mgmtAuthorizationService = mgmtAuthorizationService;
            _necnatEndpointStore = necnatEndpointStore;
        }

        public virtual async Task<List<string>> GetListPermissionMyAsync()
        {
            return await _mgmtAuthorizationService.GetListPermissionByUserIdAsync((Guid)CurrentUser.Id!);
        }

        public virtual async Task<List<Guid?>> GetListHierarchyComponentIdByPermissionNameAndHierarchyComponentTypeAsync(string permissionName, int hierarchyComponentType)
        {
            var userId = CurrentUser.Id;
            if (userId == null)
                return new List<Guid?>();

            var hierarchicalStructureIdList = await _mgmtAuthorizationService.GetListHierarchicalStructureIdByUserIdAndPermissionNameAsync((Guid)userId, permissionName);
            if (hierarchicalStructureIdList.Contains(null))
                return new List<Guid?> { null };

            var l = new List<Guid>();
            foreach (var iHierarchicalStructureId in hierarchicalStructureIdList)
                l.AddRange(await _hierarchicalStructureRecursiveService.GetListHierarchyComponentIdRecursiveAsync((Guid)iHierarchicalStructureId!, hierarchyComponentType));

            return l.Distinct().Select(x => (Guid?)x).ToList();
        }

        public virtual async Task<List<string>> GetFromEndpointsPermissionListAsync()
        {
            var necnatEndpointList = await _necnatEndpointStore.GetListAsync();

            var permissionList = new List<string>();
            foreach (var iNecnatEndpoint in necnatEndpointList)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _httpContextAccessor.HttpContext.GetTokenAsync("access_token")}");
                    var httpResponseMessage = await client.GetAsync($"{iNecnatEndpoint.Endpoint}/api/app/mgmt-authorization/permission-my");
                    if (!httpResponseMessage.IsSuccessStatusCode)
                        throw new Exception(await httpResponseMessage.Content.ReadAsStringAsync());

                    permissionList.AddRange(JsonSerializer.Deserialize<List<string>>(await httpResponseMessage.Content.ReadAsStringAsync())!);
                }
            }

            return permissionList;
        }
    }
}
