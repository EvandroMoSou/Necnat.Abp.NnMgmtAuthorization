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
    public class HierarchyAppService :
        NecnatAppService<
            Hierarchy,
            HierarchyDto,
            Guid,
            HierarchyResultRequestDto,
            IHierarchyRepository>,
        IHierarchyAppService
    {
        protected readonly IHierarchicalStructureRepository _hierarchicalStructureRepository;

        public HierarchyAppService(
            ICurrentUser currentUser,
            IStringLocalizer<NnLibCommonResource> necnatLocalizer,
            IHierarchyRepository repository,
            IHierarchicalStructureRepository hierarchicalStructureRepository) : base(currentUser, necnatLocalizer, repository)
        {
            _hierarchicalStructureRepository = hierarchicalStructureRepository;

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

            if (input.IdList != null)
                q = q.Where(x => input.IdList.Contains(x.Id));            

            if (!string.IsNullOrWhiteSpace(input.NameContains))
            {
                if (input.NameContains.Length > HierarchyConsts.MaxNameLength)
                    throw new ArgumentException($"[NameContains] MaxNameLength: {HierarchyConsts.MaxNameLength}");

                q = q.Where(x => x.Name.Contains(input.NameContains));
            }

            if (input.IsActive != null)
                q = q.Where(x => x.IsActive == input.IsActive);

            return q;
        }

        protected override async Task<Hierarchy> CheckCreateInsertedEntityAsync(Hierarchy insertedEntity, HierarchyDto? input = null)
        {
            await _hierarchicalStructureRepository.InsertAsync(new HierarchicalStructure
            {
                HierarchyId = insertedEntity.Id,
                HierarchyComponentType = 1,
                HierarchyComponentId = insertedEntity.Id
            });

            return insertedEntity;
        }

        protected override async Task<Hierarchy> CheckDeleteDbEntityAsync(Hierarchy dbEntity)
        {
            await _hierarchicalStructureRepository.DeleteAllByHierarchyIdAsync(dbEntity.Id);
            return dbEntity;
        }
    }
}
