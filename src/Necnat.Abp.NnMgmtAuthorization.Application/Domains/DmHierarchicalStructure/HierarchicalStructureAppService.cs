using Microsoft.Extensions.Localization;
using Necnat.Abp.NnLibCommon.Localization;
using Necnat.Abp.NnLibCommon.Services;
using Necnat.Abp.NnMgmtAuthorization.Models;
using Necnat.Abp.NnMgmtAuthorization.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Users;

namespace Necnat.Abp.NnMgmtAuthorization.Domains.DmHierarchicalStructure
{
    public class HierarchicalStructureAppService :
        NecnatAppService<
            HierarchicalStructure,
            HierarchicalStructureDto,
            Guid,
            HierarchicalStructureResultRequestDto,
            IHierarchicalStructureRepository>,
        IHierarchicalStructureAppService
    {
        protected readonly IHierarchyComponentService _hierarchyComponentService;

        public HierarchicalStructureAppService(
            ICurrentUser currentUser,
            IStringLocalizer<NnLibCommonResource> necnatLocalizer,
            IHierarchicalStructureRepository repository,
            IHierarchyComponentService hierarchyComponentService) : base(currentUser, necnatLocalizer, repository)
        {
            _hierarchyComponentService = hierarchyComponentService;

            GetPolicyName = NnMgmtAuthorizationPermissions.PrmHierarchicalStructure.Default;
            GetListPolicyName = NnMgmtAuthorizationPermissions.PrmHierarchicalStructure.Default;
            CreatePolicyName = NnMgmtAuthorizationPermissions.PrmHierarchicalStructure.Create;
            DeletePolicyName = NnMgmtAuthorizationPermissions.PrmHierarchicalStructure.Delete;
        }

        protected override async Task<IQueryable<HierarchicalStructure>> CreateFilteredQueryAsync(HierarchicalStructureResultRequestDto input)
        {
            //ThrowIfIsNotNull(HierarchicalStructureValidator.Validate(input, _necnatLocalizer));

            var q = await ReadOnlyRepository.GetQueryableAsync();

            if (input.UseParentId)
                q = q.Where(x => x.HierarchicalStructureIdParent == input.ParentId);

            if (input.HierarchyId != null)
                q = q.Where(x => x.HierarchyId == input.HierarchyId);

            return q;
        }

        public virtual async Task<List<HierarchicalStructureNode>> SearchHierarchicalStructureNodeAsync(SearchHierarchicalStructureNodeResultRequestDto input)
        {
            await CheckPolicyAsync(GetListPolicyName);

            var lHierarchicalStructure = await Repository.SearchByHierarchyIdAndHierarchicalStructureIdParentAsync(input.HierarchyId, input.HierarchicalStructureIdParent);

            var l = new List<HierarchicalStructureNode>();
            foreach (var iHierarchicalStructure in lHierarchicalStructure)
                l.Add(new HierarchicalStructureNode
                {
                    HierarchicalStructure = MapToGetOutputDto(iHierarchicalStructure),
                    HasChild = await Repository.AnyByHierarchyIdAndHierarchicalStructureIdParentAsync(input.HierarchyId, iHierarchicalStructure.Id)
                });

            return l;
        }

        [RemoteService(false)]
        public override Task<HierarchicalStructureDto> UpdateAsync(Guid id, HierarchicalStructureDto input)
        {
            return base.UpdateAsync(id, input);
        }

        public virtual async Task<List<HierarchyComponentModel>> GetListHierarchyComponentAsync()
        {
            await CheckPolicyAsync(NnMgmtAuthorizationPermissions.PrmHierarchicalStructure.HierarchyComponent);
            return await _hierarchyComponentService.GetListHierarchyComponentAsync();
        }

        public virtual async Task<List<HierarchyComponentTypeModel>> GetListHierarchyComponentTypeAsync()
        {
            return await _hierarchyComponentService.GetListHierarchyComponentTypeAsync();
        }
    }
}
