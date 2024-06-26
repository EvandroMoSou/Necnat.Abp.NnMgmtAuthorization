using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Necnat.Abp.NnLibCommon.Domains.NnIdentity;
using Necnat.Abp.NnLibCommon.Utils;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    [Authorize]
    public abstract class MgmtAuthorizationAppService : NnMgmtAuthorizationAppService, IMgmtAuthorizationAppService
    {
        protected readonly IAuthEndpointRepository _authEndpointRepository;
        protected readonly ICurrentUser _currentUser;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IdentityUserManager _identityUserManager;
        protected readonly INnIdentityUserRepository _nnIdentityUserRepository;

        public MgmtAuthorizationAppService(
            IAuthEndpointRepository authEndpointRepository,
            ICurrentUser currentUser,
            IHttpContextAccessor httpContextAccessor,
            IdentityUserManager identityUserManager,
            INnIdentityUserRepository nnIdentityUserRepository)
        {
            _authEndpointRepository = authEndpointRepository;
            _currentUser = currentUser;
            _httpContextAccessor = httpContextAccessor;
            _identityUserManager = identityUserManager;
            _nnIdentityUserRepository = nnIdentityUserRepository;
        }

        public async Task CallConsolidateAdminUserEndpointAsync(Guid adminUserId)
        {
            if (!_currentUser.IsInRole("admin"))
                return;

            var lAuthorizationEndpoint = await _authEndpointRepository.GetListAsync(x => x.IsActive && !x.IsAuthentication);
            foreach (var iAuthorizationEndpoint in lAuthorizationEndpoint)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _httpContextAccessor.HttpContext.GetTokenAsync("access_token")}");
                    var httpResponseMessage = await client.GetAsync($"{iAuthorizationEndpoint.Endpoint}/api/app/mgmt-authorization/consolidate-admin-user?adminUserId=" + adminUserId);
                    if (!httpResponseMessage.IsSuccessStatusCode)
                        throw new Exception(await httpResponseMessage.Content.ReadAsStringAsync());
                }
            }
        }

        public async Task ConsolidateAdminUserAsync(Guid adminUserId)
        {
            if (!_currentUser.IsInRole("admin"))
                return;

            var adminUser = await _identityUserManager.FindByNameAsync("admin");
            if (adminUser == null || adminUser.Id == adminUserId)
                return;

            var newAdmin = new IdentityUser(adminUserId, adminUser.UserName, adminUser.Email);
            newAdmin = ReflectionUtil.Clone(adminUser, newAdmin);

            var roleList = await _identityUserManager.GetRolesAsync(adminUser);
            await _identityUserManager.RemoveFromRolesAsync(adminUser, roleList);
            await _identityUserManager.DeleteAsync(adminUser);
            await _nnIdentityUserRepository.InsertAsync(newAdmin, true);

            adminUser = await _identityUserManager.FindByNameAsync("admin");
            await _identityUserManager.AddToRolesAsync(adminUser!, roleList);
        }
    }
}
