using Microsoft.AspNetCore.Authorization;
using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    [Authorize]
    public class MgmtAuthorizationAppService : NnMgmtAuthorizationAppService, IMgmtAuthorizationAppService
    {
        protected readonly IHierarchicalAccessAppService _hierarchicalAccessAppService;
        protected readonly IHierarchyComponentAppService _hierarchyComponentAppService;
        protected readonly IHierarchicalStructureAppService _hierarchicalStructureAppService;

        public MgmtAuthorizationAppService(
            IHierarchicalAccessAppService hierarchicalAccessAppService,
            IHierarchyComponentAppService hierarchyComponentAppService,
            IHierarchicalStructureAppService hierarchicalStructureAppService)
        {
            _hierarchicalAccessAppService = hierarchicalAccessAppService;
            _hierarchyComponentAppService = hierarchyComponentAppService;
            _hierarchicalStructureAppService = hierarchicalStructureAppService;
        }

        public virtual async Task<HierarchicalAuthorizationModel> GetHierarchicalAuthorizationMyAsync()
        {
            var model = new HierarchicalAuthorizationModel();
            model.UserId = (Guid)CurrentUser.Id!;

            model.LHA = await _hierarchicalAccessAppService.GetListHaMyAsync();

            var allHierarchyComponentIdList = model.LHA.SelectMany(x => x.LHSId).Distinct().ToList();
            model.LHS = await _hierarchicalStructureAppService.GetListHsAsync(allHierarchyComponentIdList);

            var hierarchyComponentList = (await _hierarchyComponentAppService.GetListAsync(new HierarchyComponentResultRequestDto { IsPaged = false })).Items;
            foreach (var iHierarchyComponentId in model.LHS.SelectMany(x => x.LHCId).Distinct())
            {
                var hierarchyComponent = hierarchyComponentList.Where(x => x.Id == iHierarchyComponentId).FirstOrDefault();
                model.LHC.Add(new HC { Id = iHierarchyComponentId, Tp = hierarchyComponent?.HierarchyComponentType, Nm = hierarchyComponent?.Name });
            }

            return model;
        }
    }
}
