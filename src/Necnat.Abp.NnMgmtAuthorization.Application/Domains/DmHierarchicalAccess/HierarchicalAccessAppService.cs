using Microsoft.Extensions.Localization;
using Necnat.Abp.NnLibCommon.Localization;
using Necnat.Abp.NnLibCommon.Services;
using Necnat.Abp.NnMgmtAuthorization.HierarchicalPermissions;
using Necnat.Abp.NnMgmtAuthorization.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Users;

namespace Necnat.Abp.NnMgmtAuthorization.Domains.DmHierarchicalAccess
{
    public class HierarchicalAccessAppService :
        NecnatAppService<
            HierarchicalAccess,
            HierarchicalAccessDto,
            Guid,
            HierarchicalAccessResultRequestDto,
            IHierarchicalAccessRepository>,
        IHierarchicalAccessAppService
    {
        protected readonly IHierarchicalAuthorizationService _hierarchicalAuthorizationService;
        protected readonly IHierarchicalStructureStore _hierarchicalStructureStore;


        public HierarchicalAccessAppService(
            ICurrentUser currentUser,
            IStringLocalizer<NnLibCommonResource> necnatLocalizer,
            IHierarchicalAccessRepository repository,
            IHierarchicalAuthorizationService hierarchicalAuthorizationService,
            IHierarchicalStructureStore hierarchicalStructureStore) : base(currentUser, necnatLocalizer, repository)
        {
            _hierarchicalAuthorizationService = hierarchicalAuthorizationService;
            _hierarchicalStructureStore = hierarchicalStructureStore;

            GetPolicyName = NnMgmtAuthorizationPermissions.PrmHierarchicalAccess.Default;
            GetListPolicyName = NnMgmtAuthorizationPermissions.PrmHierarchicalAccess.Default;
            CreatePolicyName = NnMgmtAuthorizationPermissions.PrmHierarchicalAccess.Create;
            DeletePolicyName = NnMgmtAuthorizationPermissions.PrmHierarchicalAccess.Delete;
        }

        protected override async Task<IQueryable<HierarchicalAccess>> CreateFilteredQueryAsync(HierarchicalAccessResultRequestDto input)
        {
            //ThrowIfIsNotNull(HierarchicalAccessValidator.Validate(input, _necnatLocalizer));

            var q = await ReadOnlyRepository.GetQueryableAsync();

            if (input.UserId != null)
                q = q.Where(x => x.UserId == input.UserId);

            if (input.RoleId != null)
                q = q.Where(x => x.RoleId == input.RoleId);

            var lHierarchicalStructureId = await _hierarchicalAuthorizationService.GetListHierarchicalStructureIdAsync(GetListPolicyName!);
            var lHierarchicalStructureIdRecursive = await _hierarchicalStructureStore.GetListHierarchicalStructureIdRecursiveAsync(lHierarchicalStructureId);

            if (input.HierarchicalStructureIdList != null)
            {
                var l = new List<Guid>();
                if (input.WithHierarchy == true)
                    l = await _hierarchicalStructureStore.GetListHierarchicalStructureIdRecursiveAsync(input.HierarchicalStructureIdList!);
                else
                    l = input.HierarchicalStructureIdList;

                lHierarchicalStructureIdRecursive = lHierarchicalStructureIdRecursive.Where(x => l.Contains(x)).ToList();
            }

            q = q.Where(x => lHierarchicalStructureIdRecursive.Contains(x.HierarchicalStructureId));

            return q;
        }

        public override async Task<HierarchicalAccessDto> GetAsync(Guid id)
        {
            var e = await base.GetAsync(id);

            var lHierarchicalStructureId = await _hierarchicalAuthorizationService.GetListHierarchicalStructureIdAsync(GetPolicyName!);
            var lHierarchicalStructureIdRecursive = await _hierarchicalStructureStore.GetListHierarchicalStructureIdRecursiveAsync(lHierarchicalStructureId);

            if (!lHierarchicalStructureIdRecursive.Contains((Guid)e.HierarchicalStructureId!))            
                throw new UnauthorizedAccessException($"[HierarchicalStructureId] {e.HierarchicalStructureId}");
            
            return e;
        }

        [RemoteService(false)]
        public override Task<HierarchicalAccessDto> UpdateAsync(Guid id, HierarchicalAccessDto input)
        {
            throw new NotImplementedException();
        }
    }
}
