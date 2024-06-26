using Microsoft.Extensions.Localization;
using Necnat.Abp.NnLibCommon.Localization;
using Necnat.Abp.NnLibCommon.Services;
using Necnat.Abp.NnMgmtAuthorization.Permissions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Users;

namespace Necnat.Abp.NnMgmtAuthorization.Domains.DmAuthEndpoint
{
    public class AuthEndpointAppService : 
        NecnatAppService<
            AuthEndpoint,
            AuthEndpointDto,
            Guid,
            AuthEndpointResultRequestDto,
            IAuthEndpointRepository>,
        IAuthEndpointAppService
    {
        public AuthEndpointAppService(
            ICurrentUser currentUser,
            IStringLocalizer<NnLibCommonResource> necnatLocalizer,
            IAuthEndpointRepository repository) : base(currentUser, necnatLocalizer, repository)
        {
            GetPolicyName = NnMgmtAuthorizationPermissions.PrmAuthEndpoint.Default;
            GetListPolicyName = NnMgmtAuthorizationPermissions.PrmAuthEndpoint.Default;
            CreatePolicyName = NnMgmtAuthorizationPermissions.PrmAuthEndpoint.Create;
            UpdatePolicyName = NnMgmtAuthorizationPermissions.PrmAuthEndpoint.Update;
            DeletePolicyName = NnMgmtAuthorizationPermissions.PrmAuthEndpoint.Delete;
        }

        protected override async Task<IQueryable<AuthEndpoint>> CreateFilteredQueryAsync(AuthEndpointResultRequestDto input)
        {
            ThrowIfIsNotNull(AuthEndpointValidator.Validate(input, _necnatLocalizer));

            var q = await ReadOnlyRepository.GetQueryableAsync();

            if (!string.IsNullOrWhiteSpace(input.DisplayNameContains))
                q = q.Where(x => x.DisplayName.Contains(input.DisplayNameContains));

            if (!string.IsNullOrWhiteSpace(input.EndpointContains))
                q = q.Where(x => x.Endpoint.Contains(input.EndpointContains));

            if (input.IsActive != null)
                q = q.Where(x => x.IsActive == input.IsActive);

            return q;
        }

        protected override Task<AuthEndpointDto> CheckCreateInputAsync(AuthEndpointDto input)
        {
            ThrowIfIsNotNull(AuthEndpointValidator.Validate(input, _necnatLocalizer));
            return Task.FromResult(input);
        }

        protected override Task<AuthEndpointDto> CheckUpdateInputAsync(AuthEndpointDto input)
        {
            ThrowIfIsNotNull(AuthEndpointValidator.Validate(input, _necnatLocalizer));
            return Task.FromResult(input);
        }
    }
}
