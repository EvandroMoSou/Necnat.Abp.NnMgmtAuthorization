using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Necnat.Abp.NnMgmtAuthorization.EntityFrameworkCore;
using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;

namespace Necnat.Abp.NnMgmtAuthorization.Domains.DmHierarchicalAccess
{
    public class EfCoreHierarchicalAccessRepository : EfCoreRepository<INnMgmtAuthorizationDbContext, HierarchicalAccess, Guid>, IHierarchicalAccessRepository
    {
        protected readonly IHierarchicalStructureRecursiveService _hierarchicalStructureRecursiveService;
        protected readonly UserManager<IdentityUser> _userManager;

        public EfCoreHierarchicalAccessRepository(
            IDbContextProvider<INnMgmtAuthorizationDbContext> dbContextProvider,
            IHierarchicalStructureRecursiveService hierarchicalStructureRecursiveService,
            UserManager<IdentityUser> userManager) : base(dbContextProvider)
        {
            _hierarchicalStructureRecursiveService = hierarchicalStructureRecursiveService;
            _userManager = userManager;
        }

        public virtual async Task<List<HierarchicalAccess>> GetListByUserIdAsync(Guid userId)
        {
            return await(await GetDbSetAsync()).Where(x => x.UserId == userId).ToListAsync();
        }

        public virtual async Task<List<HierarchicalAccess>> GetListByUserIdAndRoleIdAsync(Guid userId, Guid roleId)
        {
            return await (await GetDbSetAsync()).Where(x => x.UserId == userId && x.RoleId == roleId).ToListAsync();
        }

        public virtual async Task<List<Guid>> SearchHierarchicalStructureIdAsync(Guid userId, string permissionName)
        {
            throw new NotImplementedException();
            //var lRoleId = await _permissionRoleIdService.SearchRoleIdAsync(permissionName);
            //if (lRoleId == null || lRoleId.Count < 1)
            //    throw new AccessViolationException($"User: {userId} does not have permission: {permissionName}.");

            //return await (await GetDbContextAsync()).HierarchicalAccess.Where(x => x.UserId == userId && lRoleId.Contains(x.RoleId)).Select(x => x.HierarchicalStructureId).ToListAsync();
        }

        public virtual async Task<List<Guid>> SearchHierarchicalStructureIdWithHierarchyAsync(Guid userId, string permissionName)
        {
            var lHierarchicalStructureId = await SearchHierarchicalStructureIdAsync(userId, permissionName);

            var l = new List<Guid>();
            foreach (var iHierarchicalStructureId in lHierarchicalStructureId)
                l.AddRange(await _hierarchicalStructureRecursiveService.GetListHierarchicalStructureIdRecursiveAsync(iHierarchicalStructureId));

            return l.Distinct().ToList();
        }

        public virtual async Task<List<Guid>> SearchHierarchyComponentIdWithHierarchyAsync(Guid userId, string permissionName)
        {
            var lHierarchicalStructureId = await SearchHierarchicalStructureIdAsync(userId, permissionName);

            var l = new List<Guid>();
            foreach (var iHierarchicalStructureId in lHierarchicalStructureId)
                l.AddRange(await _hierarchicalStructureRecursiveService.GetListHierarchyComponentIdRecursiveAsync(iHierarchicalStructureId));

            return l.Distinct().ToList();
        }

        public virtual async Task<HierarchicalAccess> InsertIfNotExistsAsync(HierarchicalAccess entity, bool autoSave = false)
        {
            var entityDb = await (await GetDbSetAsync()).Where(x => x.UserId == entity.UserId && x.RoleId == entity.RoleId && x.HierarchicalStructureId == entity.HierarchicalStructureId).FirstOrDefaultAsync();
            if (entityDb != null)
                return entityDb;

            return await InsertAsync(entity, autoSave);
        }

        public virtual async Task CheckByHierarchicalStructureIdAsync(Guid userId, string permissionName, Guid hierarchicalStructureId)
        {
            var lHierarchicalStructureId = await SearchHierarchicalStructureIdAsync(userId, permissionName);
            if (!lHierarchicalStructureId.Contains(hierarchicalStructureId))
                throw new AccessViolationException($"User: {userId} does not have permission: {permissionName} in the hierarchical structure: {hierarchicalStructureId}.");
        }

        public virtual async Task CheckByHierarchicalStructureIdWithHierarchyAsync(Guid userId, string permissionName, Guid hierarchicalStructureId)
        {
            var l = await SearchHierarchicalStructureIdWithHierarchyAsync(userId, permissionName);
            if (!l.Contains(hierarchicalStructureId))
                throw new AccessViolationException($"User: {userId} does not have permission: {permissionName} in the hierarchical structure: {hierarchicalStructureId}.");
        }

        public virtual async Task CheckByHierarchyComponentIdWithHierarchyAsync(Guid userId, string permissionName, Guid hierarchyComponentId)
        {
            var l = await SearchHierarchyComponentIdWithHierarchyAsync(userId, permissionName);
            if (!l.Contains(hierarchyComponentId))
                throw new AccessViolationException($"Usuário: {userId} não possui a permissão: {permissionName} para o componente da hierarquia: {hierarchyComponentId}.");
        }

        public virtual async Task MantainByUserIdAsync(Guid userId, List<MantainHierarchicalAccessByUserIdModel> lMaintain)
        {
            lMaintain = lMaintain.DistinctBy(x => new { x.RoleId, x.HierarchicalStructureId }).ToList();

            //Role
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new ArgumentException($"User: {userId} not found.");

            var lRoles = await _userManager.GetRolesAsync(user);

            foreach (var iRoleName in lMaintain.Select(x => x.RoleName).Distinct())
            {
                if (lRoles.Any(x => x == iRoleName))
                {
                    lRoles.RemoveAll(x => x == iRoleName);
                    continue;
                }

                await InsertIfNotExistsRoleAsync(user, iRoleName);
            }

            foreach (var iRoleName in lRoles)
                await DeleteIfExistsRoleAsync(user, iRoleName);

            //HierarchicalAccess
            var dbContext = await GetDbContextAsync();
            var lDbHierarchicalAccess = await dbContext.HierarchicalAccess.Where(x => x.UserId == userId).ToListAsync();

            foreach (var iMaintain in lMaintain)
            {
                if (lDbHierarchicalAccess.Any(x => x.RoleId == iMaintain.RoleId && x.HierarchicalStructureId == iMaintain.HierarchicalStructureId))
                {
                    lDbHierarchicalAccess.RemoveAll(x => x.RoleId == iMaintain.RoleId && x.HierarchicalStructureId == iMaintain.HierarchicalStructureId);
                    continue;
                }

                await InsertAsync(new HierarchicalAccess
                {
                    UserId = userId,
                    RoleId = iMaintain.RoleId,
                    HierarchicalStructureId = iMaintain.HierarchicalStructureId
                });
            }

            foreach (var iDbHierarchicalAccess in lDbHierarchicalAccess)
                await DeleteAsync(iDbHierarchicalAccess);
        }

        protected virtual async Task InsertIfNotExistsRoleAsync(IdentityUser user, string roleName)
        {
            if (!await _userManager.IsInRoleAsync(user, roleName))
                await _userManager.AddToRoleAsync(user, roleName);
        }

        protected virtual async Task DeleteIfExistsRoleAsync(IdentityUser user, string roleName)
        {
            if (await _userManager.IsInRoleAsync(user, roleName))
                await _userManager.RemoveFromRoleAsync(user, roleName);
        }
    }
}
