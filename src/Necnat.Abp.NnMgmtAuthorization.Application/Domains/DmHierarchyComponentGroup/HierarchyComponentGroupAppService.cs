using Microsoft.Extensions.Localization;
using Necnat.Abp.NnLibCommon.Localization;
using Necnat.Abp.NnLibCommon.Services;
using Necnat.Abp.NnMgmtAuthorization.Permissions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Users;

namespace Necnat.Abp.NnMgmtAuthorization.Domains.DmHierarchyComponentGroup
{
    public class HierarchyComponentGroupAppService :
        NecnatAppService<
            HierarchyComponentGroup,
            HierarchyComponentGroupDto,
            Guid,
            HierarchyComponentGroupResultRequestDto,
            IHierarchyComponentGroupRepository>,
        IHierarchyComponentGroupAppService
    {
        protected HierarchyComponentGroupAppService(
            ICurrentUser currentUser,
            IStringLocalizer<NnLibCommonResource> necnatLocalizer,
            IHierarchyComponentGroupRepository repository) : base(currentUser, necnatLocalizer, repository)
        {
            GetPolicyName = NnMgmtAuthorizationPermissions.PrmHierarchyComponentGroup.Default;
            GetListPolicyName = NnMgmtAuthorizationPermissions.PrmHierarchyComponentGroup.Default;
            CreatePolicyName = NnMgmtAuthorizationPermissions.PrmHierarchyComponentGroup.Create;
            UpdatePolicyName = NnMgmtAuthorizationPermissions.PrmHierarchyComponentGroup.Update;
            DeletePolicyName = NnMgmtAuthorizationPermissions.PrmHierarchyComponentGroup.Delete;
        }

        protected override async Task<IQueryable<HierarchyComponentGroup>> CreateFilteredQueryAsync(HierarchyComponentGroupResultRequestDto input)
        {
            //ThrowIfIsNotNull(HierarchyComponentGroupValidator.Validate(input, _necnatLocalizer));

            var q = await ReadOnlyRepository.GetQueryableAsync();

            if (!string.IsNullOrWhiteSpace(input.NameContains))
            {
                if (input.NameContains.Length > HierarchyComponentGroupConsts.MaxNameLength)
                    throw new OverflowException($"[NameContains] MaxNameLength: {HierarchyComponentGroupConsts.MaxNameLength}");

                q = q.Where(x => x.Name.Contains(input.NameContains));
            }

            if (input.IsActive != null)
                q = q.Where(x => x.IsActive == input.IsActive);

            return q;
        }
    }
}
