using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Necnat.Abp.NnLibCommon.Domains.NnIdentity;
using Necnat.Abp.NnLibCommon.Utils;
using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    [Authorize]
    public class MgmtAuthorizationAppService : NnMgmtAuthorizationAppService, IMgmtAuthorizationAppService
    {
        protected readonly ICurrentUser _currentUser;
        protected readonly IHierarchicalAccessRepository _hierarchicalAccessRepository;
        protected readonly IHierarchicalStructureRecursiveService _hierarchicalStructureRecursiveService;
        protected readonly IHierarchicalStructureRepository _hierarchicalStructureRepository;
        protected readonly IHierarchyComponentService _hierarchyComponentService;
        protected readonly IHierarchyRepository _hierarchyRepository;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IdentityUserManager _identityUserManager;
        protected readonly IIdentityRoleRepository _identityRoleRepository;
        protected readonly INnIdentityUserRepository _nnIdentityUserRepository;

        public MgmtAuthorizationAppService(
            ICurrentUser currentUser,
            IHierarchicalAccessRepository hierarchicalAccessRepository,
            IHierarchicalStructureRecursiveService hierarchicalStructureRecursiveService,
            IHierarchicalStructureRepository hierarchicalStructureRepository,
            IHierarchyComponentService hierarchyComponentService,
            IHierarchyRepository hierarchyRepository,
            IHttpContextAccessor httpContextAccessor,
            IdentityUserManager identityUserManager,
            IIdentityRoleRepository identityRoleRepository,
            INnIdentityUserRepository nnIdentityUserRepository)
        {
            _currentUser = currentUser;
            _hierarchicalAccessRepository = hierarchicalAccessRepository;
            _hierarchicalStructureRecursiveService = hierarchicalStructureRecursiveService;
            _hierarchicalStructureRepository = hierarchicalStructureRepository;
            _hierarchyComponentService = hierarchyComponentService;
            _hierarchyRepository = hierarchyRepository;
            _httpContextAccessor = httpContextAccessor;
            _identityUserManager = identityUserManager;
            _identityRoleRepository = identityRoleRepository;
            _nnIdentityUserRepository = nnIdentityUserRepository;
        }

        //public virtual async Task CallConsolidateAdminUserEndpointAsync(Guid adminUserId)
        //{
        //    if (!_currentUser.IsInRole("admin"))
        //        return;

        //    var lAuthorizationEndpoint = await _authEndpointRepository.GetListAsync(x => x.IsActive && !x.IsAuthentication);
        //    foreach (var iAuthorizationEndpoint in lAuthorizationEndpoint)
        //    {
        //        using (HttpClient client = new HttpClient())
        //        {
        //            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _httpContextAccessor.HttpContext.GetTokenAsync("access_token")}");
        //            var httpResponseMessage = await client.GetAsync($"{iAuthorizationEndpoint.Endpoint}/api/app/mgmt-authorization/consolidate-admin-user?adminUserId=" + adminUserId);
        //            if (!httpResponseMessage.IsSuccessStatusCode)
        //                throw new Exception(await httpResponseMessage.Content.ReadAsStringAsync());
        //        }
        //    }
        //}

        //public virtual async Task ConsolidateAdminUserAsync(Guid adminUserId)
        //{
        //    if (!_currentUser.IsInRole("admin"))
        //        return;

        //    var adminUser = await _identityUserManager.FindByNameAsync("admin");
        //    if (adminUser == null || adminUser.Id == adminUserId)
        //        return;

        //    var newAdmin = new IdentityUser(adminUserId, adminUser.UserName, adminUser.Email);
        //    newAdmin = ReflectionUtil.Clone(adminUser, newAdmin);

        //    var roleList = await _identityUserManager.GetRolesAsync(adminUser);
        //    await _identityUserManager.RemoveFromRolesAsync(adminUser, roleList);
        //    await _nnIdentityUserRepository.DeleteAsync(adminUser.Id);
        //    await _nnIdentityUserRepository.InsertAsync(newAdmin, true);

        //    adminUser = await _identityUserManager.FindByNameAsync("admin");
        //    await _identityUserManager.AddToRolesAsync(adminUser!, roleList);
        //}

        public virtual async Task<HierarchicalAuthorizationModel> GetHierarchicalAuthorizationAsync()
        {
            throw new NotImplementedException();

            //var ha = new HierarchicalAuthorizationModel();
            //ha.UserId = (Guid)CurrentUser.Id!;

            ////HierarchicalAccess
            //var lHierarchicalAccess = await _hierarchicalAccessRepository.GetListAsync(x => x.UserId == CurrentUser.Id);
            //foreach (var iHierarchicalAccess in lHierarchicalAccess)
            //{
            //    var ahc = ha.LHAC.Where(x => x.RId == iHierarchicalAccess.RoleId).FirstOrDefault();
            //    if (ahc != null)
            //    {
            //        ahc.LHSId.Add(iHierarchicalAccess.HierarchicalStructureId!);
            //        continue;
            //    }

            //    var ah = new HAC
            //    {
            //        LHSId = new List<Guid> { iHierarchicalAccess.HierarchicalStructureId },
            //        RId = iHierarchicalAccess.RoleId
            //    };

            //    var role = await _identityRoleRepository.GetAsync(iHierarchicalAccess.RoleId);
            //    var lPermissionGrant = await _permissionGrantRepository.GetListAsync("R", role.Name);
            //    foreach (var iPermissionGrant in lPermissionGrant)
            //        ah.LPN.Add(iPermissionGrant.Name);

            //    ha.LHAC.Add(ah);
            //}

            ////HierarchicalStructure
            //var lHierarchicalStructure = await _hierarchicalStructureRepository.GetListAsync();
            //foreach (var lHierarchicalStructureId in ha.LHAC.Select(x => x.LHSId))
            //    foreach (var iHierarchicalStructureId in lHierarchicalStructureId)
            //    {
            //        var hierarchicalStructure = lHierarchicalStructure.First(x => x.Id == iHierarchicalStructureId);
            //        if (!ha.LHSC.Any(x => x.Id == iHierarchicalStructureId))
            //            ha.LHSC.Add(
            //                new HSC
            //                {
            //                    Id = iHierarchicalStructureId,
            //                    HId = hierarchicalStructure.HierarchyId,
            //                    LChl = (await _hierarchicalStructureRecursiveService.GetListHierarchicalStructureRecursiveAsync(iHierarchicalStructureId)).ToList()
            //                });
            //    }

            ////Hierarchy
            //foreach (var iHierarchyId in ha.LHSC.Select(x => x.HId).Distinct())
            //{
            //    var hierarchy = await _hierarchyRepository.GetAsync(iHierarchyId);
            //    ha.LH.Add(FromHierarchy(hierarchy));
            //}

            //return await LoadHierarchyComponentNameAsync(ha);
        }

        protected virtual async Task<HierarchicalAuthorizationModel> LoadHierarchyComponentNameAsync(HierarchicalAuthorizationModel ahModel)
        {
            Guid lastHierarchyId = Guid.Empty;
            List<HierarchyComponentModel> lHierarchyComponent = new List<HierarchyComponentModel>();
            foreach (var iHierarchicalStructure in ahModel.LHSC.OrderBy(x => x.HId).ThenBy(x => x.Id))
            {
                if(lastHierarchyId != iHierarchicalStructure.HId)
                {
                    lastHierarchyId = iHierarchicalStructure.HId;
                    lHierarchyComponent = await _hierarchyComponentService.GetListHierarchyComponentAsync(lastHierarchyId);
                }

                foreach (var iFilho in iHierarchicalStructure.LChl)
                    iFilho.HCNm = lHierarchyComponent.Where(x => x.Id == iFilho.HCId).First().Name;
            }

            return ahModel;
        }

        private H FromHierarchy(Hierarchy h)
        {
            return new H
            {
                Id = h.Id,
                Nm = h.Name,
                At = h.IsActive
            };
        }
    }
}
