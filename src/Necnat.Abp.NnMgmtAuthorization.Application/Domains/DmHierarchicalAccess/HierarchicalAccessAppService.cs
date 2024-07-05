using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Necnat.Abp.NnLibCommon.Domains.NnIdentity;
using Necnat.Abp.NnLibCommon.Localization;
using Necnat.Abp.NnLibCommon.Services;
using Necnat.Abp.NnMgmtAuthorization.Permissions;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Identity;
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
        protected readonly IIdentityRoleRepository _identityRoleRepository;
        protected readonly IdentityUserManager _identityUserManager;
        protected readonly IHierarchicalStructureStore _hierarchicalStructureRecursiveService;
        protected readonly INnIdentityUserRoleRepository _nnIdentityUserRoleRepository;

        public HierarchicalAccessAppService(
            ICurrentUser currentUser,
            IStringLocalizer<NnLibCommonResource> necnatLocalizer,
            IHierarchicalAccessRepository repository,
            IIdentityRoleRepository identityRoleRepository,
            IdentityUserManager identityUserManager,
            IHierarchicalStructureStore hierarchicalStructureRecursiveService,
            INnIdentityUserRoleRepository nnIdentityUserRoleRepository) : base(currentUser, necnatLocalizer, repository)
        {
            _identityRoleRepository = identityRoleRepository;
            _identityUserManager = identityUserManager;
            _hierarchicalStructureRecursiveService = hierarchicalStructureRecursiveService;
            _nnIdentityUserRoleRepository = nnIdentityUserRoleRepository;

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

            throw new NotImplementedException();
            //if (input.HierarchicalStructureIdList != null)
            //{
            //    var l = new List<Guid>();
            //    if (input.WithHierarchy == true)
            //        l = await _hierarchicalStructureRecursiveService.GetListHierarchicalStructureIdRecursiveAsync(input.HierarchicalStructureIdList!);
            //    else
            //        l = input.HierarchicalStructureIdList;

            //    q = q.Where(x => l.Contains(x.HierarchicalStructureId));
            //}

            //var lHierarchicalStructureId = await Repository.SearchHierarchicalStructureIdAsync((Guid)CurrentUser.Id!, GetListPolicyName!);
            //q = q.Where(x => lHierarchicalStructureId.Contains(x.HierarchicalStructureId));

            //return q;
        }

        public override async Task<HierarchicalAccessDto> GetAsync(Guid id)
        {
            var e = await base.GetAsync(id);

            await Repository.CheckByHierarchicalStructureIdAsync((Guid)CurrentUser.Id!, GetPolicyName!, (Guid)e.HierarchicalStructureId!);

            return e;
        }

        public override async Task<HierarchicalAccessDto> CreateAsync(HierarchicalAccessDto input)
        {
            await Repository.CheckByHierarchicalStructureIdWithHierarchyAsync((Guid)CurrentUser.Id!, CreatePolicyName!, (Guid)input.HierarchicalStructureId!);

            var user = await _identityUserManager.GetByIdAsync((Guid)input.UserId!);
            var role = await _identityRoleRepository.GetAsync((Guid)input.RoleId!);

            var lUserRole = await _nnIdentityUserRoleRepository.GetListAsync(x => x.UserId == input.UserId && x.RoleId == input.RoleId);
            if (!lUserRole.Any())
            {
                var lRole = await _identityUserManager.GetRolesAsync(user);
                lRole.Add(role.Name);
                (await _identityUserManager.SetRolesAsync(user, lRole)).CheckErrors();
            }

            return await base.CreateAsync(input);
        }

        [RemoteService(false)]
        public override Task<HierarchicalAccessDto> UpdateAsync(Guid id, HierarchicalAccessDto input)
        {
            throw new NotImplementedException();
        }

        public override async Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
            //var e = await Repository.GetAsync(id);
            //await Repository.CheckByHierarchicalStructureIdWithHierarchyAsync((Guid)CurrentUser.Id!, DeletePolicyName!, e.HierarchicalStructureId);

            //var user = await _identityUserManager.GetByIdAsync(e.UserId);
            //var role = await _identityRoleRepository.GetAsync(e.RoleId);

            //var lUserRole = await _nnIdentityUserRoleRepository.GetListAsync(x => x.UserId == e.UserId && x.RoleId == e.RoleId);
            //if (lUserRole.Count() == 1)
            //{
            //    var lRole = await _identityUserManager.GetRolesAsync(user);
            //    lRole.Remove(role.Name);
            //    (await _identityUserManager.SetRolesAsync(user, lRole)).CheckErrors();
            //}

            //await base.DeleteAsync(id);
        }
    }
}
