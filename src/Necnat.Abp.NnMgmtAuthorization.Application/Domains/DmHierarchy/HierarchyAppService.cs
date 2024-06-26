using Microsoft.Extensions.Localization;
using Necnat.Abp.NnLibCommon.Localization;
using Necnat.Abp.NnLibCommon.Services;
using Necnat.Abp.NnMgmtAuthorization.Permissions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Users;

namespace Necnat.Abp.NnMgmtAuthorization.Domains.DmHierarchy
{
    public abstract class HierarchyAppService :
        NecnatAppService<
            Hierarchy,
            HierarchyDto,
            Guid,
            HierarchyResultRequestDto,
            IHierarchyRepository>,
        IHierarchyAppService
    {
        protected HierarchyAppService(
            ICurrentUser currentUser,
            IStringLocalizer<NnLibCommonResource> necnatLocalizer,
            IHierarchyRepository repository) : base(currentUser, necnatLocalizer, repository)
        {
            GetPolicyName = NnMgmtAuthorizationPermissions.PrmHierarchy.Default;
            GetListPolicyName = NnMgmtAuthorizationPermissions.PrmHierarchy.Default;
            CreatePolicyName = NnMgmtAuthorizationPermissions.PrmHierarchy.Create;
            UpdatePolicyName = NnMgmtAuthorizationPermissions.PrmHierarchy.Update;
            DeletePolicyName = NnMgmtAuthorizationPermissions.PrmHierarchy.Delete;
        }

        protected override async Task<IQueryable<Hierarchy>> CreateFilteredQueryAsync(HierarchyResultRequestDto input)
        {
            //ThrowIfIsNotNull(HierarchyValidator.Validate(input, _necnatLocalizer));

            var q = await ReadOnlyRepository.GetQueryableAsync();

            if (!string.IsNullOrWhiteSpace(input.NameContains))
            {
                if (input.NameContains.Length > HierarchyConsts.MaxNameLength)
                    throw new OverflowException($"[NameContains] MaxNameLength: {HierarchyConsts.MaxNameLength}");

                q = q.Where(x => x.Name.Contains(input.NameContains));
            }

            if (input.IsActive != null)
                q = q.Where(x => x.IsActive == input.IsActive);

            return q;
        }
    }
}
