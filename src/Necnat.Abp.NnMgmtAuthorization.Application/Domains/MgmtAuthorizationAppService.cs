using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Necnat.Abp.NnLibCommon.Domains;
using Necnat.Abp.NnLibCommon.Domains.NnIdentity;
using Necnat.Abp.NnMgmtAuthorization.Domains.DmHierarchicalStructure;
using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
        protected readonly IHierarchicalStructureStore _hierarchicalStructureStore;
        protected readonly IHierarchyComponentService _hierarchyComponentService;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly INnRoleStore _nnRoleStore;
        protected readonly INecnatEndpointStore _necnatEndpointStore;

        public MgmtAuthorizationAppService(
            ICurrentUser currentUser,
            IHierarchicalAccessRepository hierarchicalAccessRepository,
            IHierarchicalStructureStore hierarchicalStructureRecursiveService,
            IHierarchyComponentService hierarchyComponentService,
            IHttpContextAccessor httpContextAccessor,
            INnRoleStore nnRoleStore,
            INecnatEndpointStore necnatEndpointStore)
        {
            _currentUser = currentUser;
            _hierarchicalAccessRepository = hierarchicalAccessRepository;
            _hierarchicalStructureStore = hierarchicalStructureRecursiveService;
            _hierarchyComponentService = hierarchyComponentService;
            _httpContextAccessor = httpContextAccessor;
            _nnRoleStore = nnRoleStore;
            _necnatEndpointStore = necnatEndpointStore;
        }

        public async Task<HierarchicalAuthorizationModel> GetHierarchicalAuthorizationMyAsync()
        {
            var model = new HierarchicalAuthorizationModel();
            model.UserId = (Guid)CurrentUser.Id!;

            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            var necnatEndpointList = await _necnatEndpointStore.GetListAsync();
            foreach (var iNecnatEndpoint in necnatEndpointList.Where(x => x.IsAuthorization == true))
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

            var authendpoint = necnatEndpointList.Where(x => x.IsAuthServer == true).First();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                var httpResponseMessage = await client.PostAsJsonAsync($"{authendpoint.Endpoint}/api/app/mgmt-authorization/get-authorization-info-two", model.LHAC.SelectMany(x => x.LHSId));
                if (!httpResponseMessage.IsSuccessStatusCode)
                    throw new Exception(await httpResponseMessage.Content.ReadAsStringAsync());

                var partialModel = JsonSerializer.Deserialize<HierarchicalAuthorizationModel>(await httpResponseMessage.Content.ReadAsStringAsync())!;
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

        [HttpPost]
        public async Task<HierarchicalAuthorizationModel> GetAuthorizationInfoTwoAsync(List<Guid> hierarchicalStructureIdList)
        {
            var model = new HierarchicalAuthorizationModel();

            var allHierarchyComponentIdList = new List<Guid>();
            foreach (var iHierarchicalStructureId in hierarchicalStructureIdList)
            {
                if (model.LHS.Any(x => x.Id == iHierarchicalStructureId))
                    continue;

                var hierarchyComponentIdList = await _hierarchicalStructureStore.GetListHierarchyComponentIdRecursiveAsync(iHierarchicalStructureId);
                allHierarchyComponentIdList.AddRange(hierarchyComponentIdList);

                model.LHS.Add(new HS { Id = iHierarchicalStructureId, LHCId = hierarchyComponentIdList });
            }

            var hierarchyComponentList = await _hierarchyComponentService.GetListHierarchyComponentAsync();
            foreach (var iHierarchyComponentId in allHierarchyComponentIdList.Distinct())
            {
                var hierarchyComponent = hierarchyComponentList.Where(x => x.Id == iHierarchyComponentId).FirstOrDefault();
                model.LHC.Add(new HC { Id = iHierarchyComponentId, Tp = hierarchyComponent?.HierarchyComponentType, Nm = hierarchyComponent?.Name });
            }

            return model;
        }

        [HttpPost]
        public async Task<List<Guid>> GetListHierarchyComponentIdRecursiveAsync(Guid hierarchicalStructureId)
        {
            return await _hierarchicalStructureStore.GetListHierarchyComponentIdRecursiveAsync(hierarchicalStructureId);
        }

        //public virtual async Task<List<string>> GetListPermissionMyAsync()
        //{
        //    return await _mgmtAuthorizationService.GetListPermissionByUserIdAsync((Guid)CurrentUser.Id!);
        //}

        //public virtual async Task<List<Guid?>> GetListHierarchyComponentIdByPermissionNameAndHierarchyComponentTypeAsync(string permissionName, int hierarchyComponentType)
        //{
        //    var userId = CurrentUser.Id;
        //    if (userId == null)
        //        return new List<Guid?>();

        //    var hierarchicalStructureIdList = await _mgmtAuthorizationService.GetListHierarchicalStructureIdByUserIdAndPermissionNameAsync((Guid)userId, permissionName);
        //    var l = new List<Guid>();
        //    foreach (var iHierarchicalStructureId in hierarchicalStructureIdList)
        //        l.AddRange(await _hierarchicalStructureRecursiveService.GetListHierarchyComponentIdRecursiveAsync((Guid)iHierarchicalStructureId!, hierarchyComponentType));

        //    return l.Distinct().Select(x => (Guid?)x).ToList();
        //}

        //public virtual async Task<List<string>> GetFromEndpointsPermissionListAsync()
        //{
        //    var necnatEndpointList = await _necnatEndpointStore.GetListAsync();

        //    var permissionList = new List<string>();
        //    foreach (var iNecnatEndpoint in necnatEndpointList.Where(x => x.IsAuthz == true))
        //    {
        //        using (HttpClient client = new HttpClient())
        //        {
        //            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _httpContextAccessor.HttpContext.GetTokenAsync("access_token")}");
        //            var httpResponseMessage = await client.GetAsync($"{iNecnatEndpoint.Endpoint}/api/app/mgmt-authorization/permission-my");
        //            if (!httpResponseMessage.IsSuccessStatusCode)
        //                throw new Exception(await httpResponseMessage.Content.ReadAsStringAsync());

        //            permissionList.AddRange(JsonSerializer.Deserialize<List<string>>(await httpResponseMessage.Content.ReadAsStringAsync())!);
        //        }
        //    }

        //    return permissionList;
        //}
    }
}
