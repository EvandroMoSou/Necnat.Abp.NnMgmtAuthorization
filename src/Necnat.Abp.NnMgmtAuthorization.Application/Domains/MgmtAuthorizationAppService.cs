using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Necnat.Abp.NnLibCommon.Domains;
using Necnat.Abp.NnLibCommon.Domains.NnIdentity;
using Necnat.Abp.NnMgmtAuthorization.Domains.DmHierarchicalStructure;
using Necnat.Abp.NnMgmtAuthorization.Models;
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
        protected readonly IHierarchicalAccessRepository _hierarchicalAccessRepository;
        protected readonly IHierarchicalStructureStore _hierarchicalStructureRecursiveService;
        protected readonly IHierarchyComponentService _hierarchyComponentService;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IMgmtAuthorizationService _mgmtAuthorizationService;
        protected readonly INnRoleStore _nnRoleStore;
        protected readonly INecnatEndpointStore _necnatEndpointStore;

        public MgmtAuthorizationAppService(
            ICurrentUser currentUser,
            IHierarchicalAccessRepository hierarchicalAccessRepository,
            IHierarchicalStructureStore hierarchicalStructureRecursiveService,
            IHierarchyComponentService hierarchyComponentService,
            IHttpContextAccessor httpContextAccessor,
            IMgmtAuthorizationService mgmtAuthorizationService,
            INnRoleStore nnRoleStore,
            INecnatEndpointStore necnatEndpointStore)
        {
            _currentUser = currentUser;
            _hierarchicalAccessRepository = hierarchicalAccessRepository;
            _hierarchicalStructureRecursiveService = hierarchicalStructureRecursiveService;
            _hierarchyComponentService = hierarchyComponentService;
            _httpContextAccessor = httpContextAccessor;
            _mgmtAuthorizationService = mgmtAuthorizationService;
            _nnRoleStore = nnRoleStore;
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
            var l = new List<Guid>();
            foreach (var iHierarchicalStructureId in hierarchicalStructureIdList)
                l.AddRange(await _hierarchicalStructureRecursiveService.GetListHierarchyComponentIdRecursiveAsync((Guid)iHierarchicalStructureId!, hierarchyComponentType));

            return l.Distinct().Select(x => (Guid?)x).ToList();
        }

        public virtual async Task<List<string>> GetFromEndpointsPermissionListAsync()
        {
            var necnatEndpointList = await _necnatEndpointStore.GetListAsync();

            var permissionList = new List<string>();
            foreach (var iNecnatEndpoint in necnatEndpointList.Where(x => x.IsAuthz == true))
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

        public async Task<HierarchicalAuthorizationModel> GetHierarchicalAuthorizationMyAsync()
        {
            var model = new HierarchicalAuthorizationModel();
            model.UserId = (Guid)CurrentUser.Id!;

            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            var necnatEndpointList = await _necnatEndpointStore.GetListAsync();
            foreach (var iNecnatEndpoint in necnatEndpointList.Where(x => x.IsAuthz == true))
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                    var httpResponseMessage = await client.GetAsync($"{iNecnatEndpoint.Endpoint}/api/app/mgmt-authorization/authorization-info-one-my");
                    if (!httpResponseMessage.IsSuccessStatusCode)
                        throw new Exception(await httpResponseMessage.Content.ReadAsStringAsync());

                    model.LHAC.AddRange(JsonSerializer.Deserialize<HierarchicalAuthorizationModel>(await httpResponseMessage.Content.ReadAsStringAsync())!.LHAC);
                }
            }

            var authendpoint = necnatEndpointList.Where(x => x.IsUser == true).First();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                var httpResponseMessage = await client.GetAsync($"{authendpoint.Endpoint}/api/app/mgmt-authorization/authorization-info-two");
                if (!httpResponseMessage.IsSuccessStatusCode)
                    throw new Exception(await httpResponseMessage.Content.ReadAsStringAsync());

                var partialModel = JsonSerializer.Deserialize<HierarchicalAuthorizationModel>(await httpResponseMessage.Content.ReadAsStringAsync())!;
                model.LH = partialModel.LH;
                model.LHS = partialModel.LHS;
                model.LHC = partialModel.LHC;
            }

            return model;
        }

        public async Task<HierarchicalAuthorizationModel> GetAuthorizationInfoOneMyAsync()
        {
            var model = new HierarchicalAuthorizationModel();

            var dbHierarchicalAccessList = await _hierarchicalAccessRepository.GetListByUserIdAsync((Guid)CurrentUser.Id!);
            foreach (var iDbHierarchicalAccess in dbHierarchicalAccessList)
            {
                var e = model.LHAC.Where(x => x.RId == iDbHierarchicalAccess.RoleId).FirstOrDefault();
                if (e == null)
                    model.LHAC.Add(new HAC { RId = iDbHierarchicalAccess.RoleId, LHSId = new List<Guid> { iDbHierarchicalAccess.HierarchicalStructureId } });
                else
                    e.LHSId.Add(iDbHierarchicalAccess.HierarchicalStructureId);
            }

            foreach (var iE in model.LHAC)
                iE.LPN = await _nnRoleStore.GetPermissionListByIdAsync(iE.RId);

            return model;
        }

        public async Task<HierarchicalAuthorizationModel> GetAuthorizationInfoTwoMyAsync(List<Guid> hierarchicalStructureIdList)
        {
            var model = new HierarchicalAuthorizationModel();

            var dbHierarchicalAccessList = await _hierarchicalAccessRepository.GetListByIdListAsync(hierarchicalStructureIdList);

            var allHierarchyComponentIdList = new List<Guid>();
            foreach (var iDbHierarchicalAccess in dbHierarchicalAccessList)
            {
                var hierarchyComponentIdList = await _hierarchicalStructureRecursiveService.GetListHierarchyComponentIdRecursiveAsync(iDbHierarchicalAccess.HierarchicalStructureId);
                allHierarchyComponentIdList.AddRange(hierarchyComponentIdList);

                if (!model.LHS.Any(x => x.Id == iDbHierarchicalAccess.HierarchicalStructureId))
                    model.LHS.Add(new HS { Id = iDbHierarchicalAccess.HierarchicalStructureId, LHCId = hierarchyComponentIdList });
            }

            var hierarchyComponentList = await _hierarchyComponentService.GetListHierarchyComponentAsync();
            foreach (var iHierarchyComponentId in allHierarchyComponentIdList.Distinct())
            {
                var hierarchyComponent = hierarchyComponentList.Where(x => x.Id == iHierarchyComponentId).FirstOrDefault();
                model.LHC.Add(new HC { Id = iHierarchyComponentId, Tp = hierarchyComponent?.HierarchyComponentType, Nm = hierarchyComponent?.Name });
            }

            return model;
        }
    }
}
