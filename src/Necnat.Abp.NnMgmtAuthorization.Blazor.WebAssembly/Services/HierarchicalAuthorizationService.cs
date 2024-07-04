using Necnat.Abp.NnMgmtAuthorization.Domains;
using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;
using Volo.Abp.AspNetCore.Mvc.Client;
using Volo.Abp.DependencyInjection;

namespace Necnat.Abp.NnMgmtAuthorization.Blazor.WebAssembly.Services
{
    public class HierarchicalAuthorizationService : IHierarchicalAuthorizationService, IScopedDependency
    {
        protected readonly ICachedApplicationConfigurationClient _configurationClient;

        public HierarchicalAuthorizationService(ICachedApplicationConfigurationClient configurationClient)
        {
            _configurationClient = configurationClient;
        }

        ApplicationConfigurationDto? _applicationConfiguration;
        public async Task<ApplicationConfigurationDto> GetApplicationConfigurationAsync()
        {
            if (_applicationConfiguration != null)
                return _applicationConfiguration;

            _applicationConfiguration = await _configurationClient.GetAsync();
            return _applicationConfiguration;
        }

        List<HAC>? _lhac;
        public async Task<List<HAC>> GetLHACAsync()
        {
            if (_lhac != null)
                return _lhac;

            _lhac = JsonSerializer.Deserialize<List<HAC>>((await GetApplicationConfigurationAsync()).Setting.Values["ua:lhac"]!)!;
            return _lhac;
        }

        List<HS>? _lhs;
        public async Task<List<HS>> GetLHSAsync()
        {
            if (_lhs != null)
                return _lhs;

            _lhs = JsonSerializer.Deserialize<List<HS>>((await GetApplicationConfigurationAsync()).Setting.Values["ua:lhs"]!)!;
            return _lhs;
        }

        List<HC>? _lhc;
        public async Task<List<HC>> GetLHCAsync()
        {
            if (_lhc != null)
                return _lhc;

            _lhc = JsonSerializer.Deserialize<List<HC>>((await GetApplicationConfigurationAsync()).Setting.Values["ua:lhc"]!)!;
            return _lhc;
        }

        public async Task<bool> CheckAsync()
        {
            return (await GetApplicationConfigurationAsync()).Setting.Values.ContainsKey("ua:lhac");
        }

        public async Task<List<HierarchyComponentDto>> GetListHierarchyComponentAsync(string permissionName, int? hierarchyComponentTypeId = null)
        {
            return await GetListHierarchyComponent(await GetListHierarchyComponentId(await GetListHierarchicalStructureId(permissionName)));
        }

        public async Task<List<Guid>> GetListHierarchicalStructureId(string permissionName)
        {
            var list = new List<Guid>();

            foreach (var hac in await GetLHACAsync())
                if (hac.LPN.Contains(permissionName))
                    list.AddRange(hac.LHSId);

            return list.Distinct().ToList();
        }

        public async Task<List<Guid>> GetListHierarchyComponentId(List<Guid> hierarchicalStructureIdList)
        {
            var list = new List<Guid>();

            foreach (var iHierarchicalStructureId in hierarchicalStructureIdList)
            {
                var hs = (await GetLHSAsync()).Where(x => x.Id == iHierarchicalStructureId).First();
                list.AddRange(hs.LHCId);
            }

            return list.Distinct().ToList();
        }

        public async Task<List<HierarchyComponentDto>> GetListHierarchyComponent(List<Guid> hierarchyComponentIdList, int? hierarchyComponentTypeId = null)
        {
            return (await GetLHCAsync()).Where(x => hierarchyComponentIdList.Contains(x.Id)
                && (hierarchyComponentTypeId == null || x.Tp == hierarchyComponentTypeId))
                .Select(x => new HierarchyComponentDto { Id = x.Id, HierarchyComponentType = x.Tp, Name = x.Nm }).ToList();
        }

        public async Task<bool> IsGrantedAsync(string permissionName, Guid? hierarchyComponentId = null)
        {
            foreach (var hac in await GetLHACAsync())
            {
                if (!hac.LPN.Contains(permissionName))
                    continue;

                if (hierarchyComponentId == null)
                    return true;

                foreach (var hsId in hac.LHSId)
                    if ((await GetLHSAsync()).Where(x => x.Id == hsId).First().LHCId.Contains((Guid)hierarchyComponentId))
                        return true;
            }

            return false;
        }
    }
}
