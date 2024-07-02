using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Users;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class HierarchicalAuthorizationService : IHierarchicalAuthorizationService
    {
        readonly ICurrentUser _currentUser;
        readonly IMgmtAuthorizationAppService _mgmtAuthorizationAppService;
        readonly IHierarchicalStructureAppService _hierarchicalStructureAppService;

        public HierarchicalAuthorizationModel AhModel { get; set; } = new HierarchicalAuthorizationModel();

        public HierarchicalAuthorizationService(
            ICurrentUser currentUser,            
            IHierarchicalStructureAppService hierarchicalStructureAppService,
            IMgmtAuthorizationAppService mgmtAuthorizationAppService)
        {
            _currentUser = currentUser;
            _hierarchicalStructureAppService = hierarchicalStructureAppService;
            _mgmtAuthorizationAppService = mgmtAuthorizationAppService;
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(AhModel);
        }

        public async Task InitializeAsync()
        {
            var currentUserChanged = AhModel?.UserId != _currentUser.Id;

            if (currentUserChanged)
            {
                AhModel = await _mgmtAuthorizationAppService.GetHierarchicalAuthorizationAsync();

                DListaHierarchyComponentUS = new Dictionary<string, List<Guid>>();
                foreach (var iHierarchicalStructure in AhModel.LHSC)
                    await InitializeLastSelectedAsync(iHierarchicalStructure);
            }
        }

        public bool CheckPermission(string permissionName, int hierarchyComponentType, Guid hierarchyComponentId)
        {
            var lHierarchicalStructureId = SearchHierarchicalStructureId(hierarchyComponentType, hierarchyComponentId);
            foreach (var iHierarchicalStructureId in lHierarchicalStructureId)
                if (CheckPermission(permissionName, iHierarchicalStructureId))
                    return true;

            return false;
        }

        public bool CheckPermission(string permissionName, Guid hierarchicalStructureId)
        {
            var lHierarchicalAccess = AhModel.LHAC.Where(x => x.LPN.Contains(permissionName));
            foreach (var lHierarchicalStructureId in lHierarchicalAccess.Select(x => x.LHSId))
                foreach (var iHierarchicalStructureId in lHierarchicalStructureId)
                    if (IsChild(iHierarchicalStructureId, hierarchicalStructureId))
                        return true;

            return false;
        }

        public HierarchicalStructureDto? GetHierarchicalStructure(Guid hierarchicalStructureId)
        {
            foreach (var iHierarchicalStructure in AhModel.LHSC)
            {
                var filho = iHierarchicalStructure.LChl.Where(x => x.Id == hierarchicalStructureId).FirstOrDefault();
                if (filho != null)
                    return ToHierarchicalStructureDto(filho);
            }

            return null;
        }

        public string? GetHierarchyComponentName(Guid hierarchicalStructureId)
        {
            return GetHierarchicalStructure(hierarchicalStructureId)?.HierarchyComponentName;
        }

        public List<HierarchyComponentModel> SearchHierarchyComponent(string permissionName, int? hierarchyComponentType = null)
        {
            var lHierarchicalStructure = SearchHSC(permissionName);

            var l = new List<HierarchyComponentModel>();
            foreach (var hierarchicalStructure in lHierarchicalStructure)
                foreach (var iFilho in hierarchicalStructure.LChl)
                    if (hierarchyComponentType == null || iFilho.HCT == (int)hierarchyComponentType)
                        if (!l.Any(x => x.Id == iFilho.Id))
                            l.Add(ToHierarchyComponentDto(iFilho));

            return l;
        }

        public List<HierarchicalStructureDto> SearchHierarchicalStructure()
        {
            var l = new List<HierarchicalStructureDto>();
            foreach (var iHierarchicalStructure in AhModel.LHSC)
                foreach (var iFilho in iHierarchicalStructure.LChl)
                    l.Add(ToHierarchicalStructureDto(iFilho));

            return l;
        }

        public List<HierarchicalStructureDto> SearchHierarchicalStructure(string permissionName)
        {
            var lHierarchicalStructure = SearchHSC(permissionName);

            var l = new List<HierarchicalStructureDto>();
            foreach (var hierarchicalStructure in lHierarchicalStructure)
                foreach (var iFilho in hierarchicalStructure.LChl)
                    if (!l.Any(x => x.Id == iFilho.Id))
                        l.Add(ToHierarchicalStructureDto(iFilho));

            return l;
        }

        public List<HSC> SearchHierarchicalStructureContainer(string permissionName)
        {
            var lHierarchicalStructureId = SearchHSId(permissionName);
            var lHierarchicalStructureContainer = AhModel.LHSC.Where(x => lHierarchicalStructureId.Contains(x.Id)).ToList();

            var lHierarchicalStructureContainerFiltered = new List<HSC>();
            foreach (var iHierarchicalStructureContainer in lHierarchicalStructureContainer.OrderByDescending(x => x.LChl.Count))
                if (!lHierarchicalStructureContainerFiltered.Any(x => x.LChl.Any(y => y.Id == iHierarchicalStructureContainer.Id)))
                    lHierarchicalStructureContainerFiltered.Add(iHierarchicalStructureContainer);

            return lHierarchicalStructureContainerFiltered;
        }

        public List<Guid> SearchHierarchicalStructureId(int hierarchyComponentType, Guid hierarchyComponentId)
        {
            var l = new List<Guid>();

            foreach (var iHierarchicalStructure in AhModel.LHSC)
                l.AddRange(iHierarchicalStructure.LChl.Where(x => x.HCT == (int)hierarchyComponentType && x.HCId == hierarchyComponentId).Select(x => x.Id));

            return l;
        }

        public List<Guid> SearchHierarchicalStructureId(string permissionName)
        {
            var lHierarchicalStructure = SearchHSC(permissionName);

            var lFilhoId = new List<Guid>();
            foreach (var hierarchicalStructure in lHierarchicalStructure)
                lFilhoId.AddRange(hierarchicalStructure.LChl.Select(x => x.Id));

            return lFilhoId;
        }

        public List<HierarchyDto> SearchHierarchy(string permissionName)
        {
            var lHierarchicalStructureId = SearchHSId(permissionName);
            var lHierarchyId = AhModel.LHSC.Where(x => lHierarchicalStructureId.Contains(x.Id)).Select(x => x.HId).Distinct();
            return AhModel.LH.Where(x => lHierarchyId.Contains(x.Id)).Select(ToHierarchyDto).ToList();
        }

        public bool IsChild(Guid hierarchicalStructureId, int hierarchyComponentType, Guid hierarchyComponentId)
        {
            var lHierarchicalStructureId = SearchHierarchicalStructureId(hierarchyComponentType, hierarchyComponentId);
            foreach (var iHierarchicalStructureId in lHierarchicalStructureId)
                if (IsChild(hierarchicalStructureId, iHierarchicalStructureId))
                    return true;

            return false;
        }

        public bool IsChild(Guid hierarchicalStructureId, Guid hierarchicalStructureIdFilho)
        {
            return AhModel.LHSC.Any(x => x.Id == hierarchicalStructureId && x.LChl.Any(x => x.Id == hierarchicalStructureIdFilho));
        }

        public List<HierarchicalStructureDto> SearchHierarchicalStructureHead(HSC hierarchicalStructure)
        {
            var lFilhoId = hierarchicalStructure.LChl.Select(x => x.Id).ToList();
            return hierarchicalStructure.LChl.Where(x => x.IdParent == null || !lFilhoId.Contains((Guid)x.IdParent!)).Select(ToHierarchicalStructureDto).ToList();
        }

        private IEnumerable<Guid> SearchHSId(string permissionName)
        {
            var lHierarchicalStructureId = new List<Guid>();

            var lLHSId = AhModel.LHAC.Where(x => x.LPN.Contains(permissionName)).Select(x => x.LHSId);
            foreach (var iLHSId in lLHSId)
                lHierarchicalStructureId.AddRange(iLHSId);

            return lHierarchicalStructureId;
        }

        private IEnumerable<HSC> SearchHSC(string permissionName)
        {
            var lHierarchicalStructureId = SearchHSId(permissionName);
            return AhModel.LHSC.Where(x => lHierarchicalStructureId.Contains(x.Id));
        }

        private HierarchyComponentModel ToHierarchyComponentDto(HS eh)
        {
            return new HierarchyComponentModel
            {
                Id = eh.HCId,
                HierarchyComponentType = eh.HCT,
                Name = eh.HCNm,
            };
        }

        private HierarchicalStructureDto ToHierarchicalStructureDto(HS eh)
        {
            return new HierarchicalStructureDto
            {
                Id = eh.Id,
                HierarchicalStructureIdParent = eh.IdParent,
                HierarchyComponentType = eh.HCT,
                HierarchyComponentId = eh.HCId,
                HierarchyComponentName = eh.HCNm,
            };
        }

        private HierarchyDto ToHierarchyDto(H h)
        {
            return new HierarchyDto
            {
                Id = h.Id,
                Name = h.Nm,
                IsActive = h.At
            };
        }

        #region LastSelected

        public Dictionary<string, bool> DWithHierarchyUS { get; set; } = new Dictionary<string, bool>();
        public Dictionary<string, Guid> DHierarchyComponentUS { get; set; } = new Dictionary<string, Guid>();
        public Dictionary<string, List<Guid>> DListaHierarchyComponentUS { get; set; } = new Dictionary<string, List<Guid>>();

        private async Task InitializeLastSelectedAsync(HSC hierarchicalStructure)
        {
            var lFilhoId = hierarchicalStructure.LChl.Select(x => x.Id).ToList();
            var lHierarchyComponentType = await _hierarchicalStructureAppService.GetListHierarchyComponentTypeAsync(hierarchicalStructure.HId);
            lHierarchyComponentType.Add(new HierarchyComponentTypeModel { Name = "Hierarchy Component" });
            foreach (var iHierarchyComponentType in lHierarchyComponentType)
            {
                var filho = hierarchicalStructure.LChl
                    .Where(x => (iHierarchyComponentType.Id == null || x.HCT == (int)iHierarchyComponentType.Id)
                        && (x.IdParent == null || !lFilhoId.Contains((Guid)x.IdParent!)))
                    .OrderBy(x => x.Id).FirstOrDefault();

                if (filho != null)
                {
                    DWithHierarchyUS.Add(GetDictionaryKey(hierarchicalStructure.Id, iHierarchyComponentType.Id), false);
                    DHierarchyComponentUS.Add(GetDictionaryKey(hierarchicalStructure.Id, iHierarchyComponentType.Id), filho.Id);
                    DListaHierarchyComponentUS.Add(GetDictionaryKey(hierarchicalStructure.Id, iHierarchyComponentType.Id), new List<Guid> { filho.Id });
                }
            }
        }

        public bool GetWithHierarchyLastSelected(string permissionName, int? hierarchyComponentType = null)
        {
            var lHierarchicalStructureId = SearchHSId(permissionName);
            var hierarchicalStructure = AhModel.LHSC.Where(x => lHierarchicalStructureId.Contains(x.Id)).OrderByDescending(x => x.LChl.Count()).FirstOrDefault();
            if (hierarchicalStructure == null)
                return false;

            return DWithHierarchyUS[GetDictionaryKey(hierarchicalStructure.Id, hierarchyComponentType)];
        }

        public Guid GetHierarchyComponentIdLastSelected(string permissionName, int? hierarchyComponentType = null)
        {
            var lHierarchicalStructureId = SearchHSId(permissionName);
            var hierarchicalStructure = AhModel.LHSC.Where(x => lHierarchicalStructureId.Contains(x.Id)).OrderByDescending(x => x.LChl.Count()).FirstOrDefault();
            if (hierarchicalStructure == null)
                return Guid.Empty;

            return DHierarchyComponentUS[GetDictionaryKey(hierarchicalStructure.Id, hierarchyComponentType)];
        }

        public List<Guid> GetHierarchyComponentIdListLastSelected(string permissionName, int? hierarchyComponentType = null)
        {
            var lHierarchicalStructureId = SearchHSId(permissionName);
            var hierarchicalStructure = AhModel.LHSC.Where(x => lHierarchicalStructureId.Contains(x.Id)).OrderByDescending(x => x.LChl.Count()).FirstOrDefault();
            if (hierarchicalStructure == null)
                return new List<Guid>();

            if (DListaHierarchyComponentUS.ContainsKey(GetDictionaryKey(hierarchicalStructure.Id, hierarchyComponentType)))
                return DListaHierarchyComponentUS[GetDictionaryKey(hierarchicalStructure.Id, hierarchyComponentType)];
            else
                return new List<Guid>();
        }

        public void SetWithHierarchyLastSelected(string permissionName, bool withHierarchy, int? hierarchyComponentType = null)
        {
            var lHierarchicalStructureId = SearchHSId(permissionName);
            var hierarchicalStructure = AhModel.LHSC.Where(x => lHierarchicalStructureId.Contains(x.Id)).OrderByDescending(x => x.LChl.Count()).FirstOrDefault();
            if (hierarchicalStructure == null)
                return;

            DWithHierarchyUS[GetDictionaryKey(hierarchicalStructure.Id, hierarchyComponentType)] = withHierarchy;
        }

        public void SetHierarchyComponentLastSelected(string permissionName, Guid id, int? hierarchyComponentType = null)
        {
            var lHierarchicalStructureId = SearchHSId(permissionName);
            var hierarchicalStructure = AhModel.LHSC.Where(x => lHierarchicalStructureId.Contains(x.Id)).OrderByDescending(x => x.LChl.Count()).FirstOrDefault();
            if (hierarchicalStructure == null)
                return;

            DHierarchyComponentUS[GetDictionaryKey(hierarchicalStructure.Id, hierarchyComponentType)] = id;
        }

        public void SetListHierarchyComponentLastSelected(string permissionName, List<Guid> lId, int? hierarchyComponentType = null)
        {
            var lHierarchicalStructureId = SearchHSId(permissionName);
            var hierarchicalStructure = AhModel.LHSC.Where(x => lHierarchicalStructureId.Contains(x.Id)).OrderByDescending(x => x.LChl.Count()).FirstOrDefault();
            if (hierarchicalStructure == null)
                return;

            DListaHierarchyComponentUS[GetDictionaryKey(hierarchicalStructure.Id, hierarchyComponentType)] = lId;
        }

        private string GetDictionaryKey(Guid hierarchicalStructureId, int? hierarchyComponentType)
        {
            return hierarchicalStructureId.ToString() + hierarchyComponentType;
        }

        #endregion
    }
}
