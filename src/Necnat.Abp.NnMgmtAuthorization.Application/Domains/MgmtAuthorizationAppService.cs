using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Necnat.Abp.NnLibCommon.Domains;
using Necnat.Abp.NnLibCommon.Domains.NnIdentity;
using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    [Authorize]
    public class MgmtAuthorizationAppService : NnMgmtAuthorizationAppService, IMgmtAuthorizationAppService
    {
        protected readonly IHierarchicalAccessRepository _hierarchicalAccessRepository;
        protected readonly IHierarchicalStructureAppService _hierarchicalStructureAppService;
        protected readonly IHierarchicalStructureStore _hierarchicalStructureStore;
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly INnEndpointStore _nnEndpointStore;
        protected readonly INnRoleStore _nnRoleStore;

        public MgmtAuthorizationAppService(
            IHierarchicalAccessRepository hierarchicalAccessRepository,
            IHierarchicalStructureAppService hierarchicalStructureAppService,
            IHierarchicalStructureStore hierarchicalStructureRecursiveService,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            INnEndpointStore nnEndpointStore,
            INnRoleStore nnRoleStore)
        {
            _hierarchicalAccessRepository = hierarchicalAccessRepository;
            _hierarchicalStructureAppService = hierarchicalStructureAppService;
            _hierarchicalStructureStore = hierarchicalStructureRecursiveService;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _nnEndpointStore = nnEndpointStore;
            _nnRoleStore = nnRoleStore;
        }

        public async Task<HierarchicalAuthorizationModel> GetHierarchicalAuthorizationMyAsync()
        {
            var model = new HierarchicalAuthorizationModel();
            model.UserId = (Guid)CurrentUser.Id!;

            var userAuthzNnEndpointList = await _nnEndpointStore.GetListByTagAsync(NnMgmtAuthorizationConsts.NnEndpointTagGetUserAuthzInfoMy);
            foreach (var iEndpoint in userAuthzNnEndpointList)
            {
                using (HttpClient client = _httpClientFactory.CreateClient(NnMgmtAuthorizationConsts.HttpClientName))
                {
                    try
                    {
                        var httpResponseMessage = await client.GetAsync($"{iEndpoint.UrlUri}/api/nn-mgmt-authorization/mgmt-authorization/user-authz-info-my");
                        if (httpResponseMessage.IsSuccessStatusCode)
                            model.LHAC.AddRange(JsonSerializer.Deserialize<HierarchicalAuthorizationModel>(await httpResponseMessage.Content.ReadAsStringAsync())!.LHAC);
                    }
                    catch { }
                }
            }

            var hierarchyAuthzNnEndpointList = await _nnEndpointStore.GetListByTagAsync(NnMgmtAuthorizationConsts.NnEndpointTagGetHierarchyAuthzInfo);
            var hierarchyAuthzNnEndpoint = hierarchyAuthzNnEndpointList.First();
            using (HttpClient client = _httpClientFactory.CreateClient(NnMgmtAuthorizationConsts.HttpClientName))
            {
                var httpResponseMessage = await client.PostAsJsonAsync($"{hierarchyAuthzNnEndpoint}/api/nn-mgmt-authorization/mgmt-authorization/get-hierarchy-authz-info", model.LHAC.SelectMany(x => x.LHSId));
                if (!httpResponseMessage.IsSuccessStatusCode)
                    throw new Exception(await httpResponseMessage.Content.ReadAsStringAsync());

                var partialModel = JsonSerializer.Deserialize<HierarchicalAuthorizationModel>(await httpResponseMessage.Content.ReadAsStringAsync())!;
                model.LHS = partialModel.LHS;
                model.LHC = partialModel.LHC;
            }

            return model;
        }

        public async Task<HierarchicalAuthorizationModel> GetUserAuthzInfoMyAsync()
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
        public async Task<HierarchicalAuthorizationModel> GetHierarchyAuthzInfoAsync(List<Guid> hierarchicalStructureIdList)
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

            var hierarchyComponentList = await _hierarchicalStructureAppService.GetListHierarchyComponentAsync();
            foreach (var iHierarchyComponentId in allHierarchyComponentIdList.Distinct())
            {
                var hierarchyComponent = hierarchyComponentList.Where(x => x.Id == iHierarchyComponentId).FirstOrDefault();
                model.LHC.Add(new HC { Id = iHierarchyComponentId, Tp = hierarchyComponent?.HierarchyComponentType, Nm = hierarchyComponent?.Name });
            }

            return model;
        }

        //[HttpPost]
        //public async Task<List<Guid>> GetListHierarchyComponentIdRecursiveAsync(Guid hierarchicalStructureId)
        //{
        //    return await _hierarchicalStructureStore.GetListHierarchyComponentIdRecursiveAsync(hierarchicalStructureId);
        //}

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
        //    var nnEndpointList = await _nnEndpointStore.GetListAsync();

        //    var permissionList = new List<string>();
        //    foreach (var iNnEndpoint in nnEndpointList.Where(x => x.IsAuthz == true))
        //    {
        //        using (HttpClient client = _httpClientFactory.CreateClient(NnMgmtAuthorizationConsts.HttpClientName))
        //        {
        //            var httpResponseMessage = await client.GetAsync($"{iNnEndpoint.Endpoint}/api/app/mgmt-authorization/permission-my");
        //            if (!httpResponseMessage.IsSuccessStatusCode)
        //                throw new Exception(await httpResponseMessage.Content.ReadAsStringAsync());

        //            permissionList.AddRange(JsonSerializer.Deserialize<List<string>>(await httpResponseMessage.Content.ReadAsStringAsync())!);
        //        }
        //    }

        //    return permissionList;
        //}
    }
}
