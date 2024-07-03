using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Necnat.Abp.NnLibCommon.Domains;
using Necnat.Abp.NnMgmtAuthorization.Localization;
using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Localization;

namespace Necnat.Abp.NnMgmtAuthorization.Domains.DmHierarchicalStructure
{
    public class HierarchyComponentService : IHierarchyComponentService, ITransientDependency
    {
        #region Localization

        public IAbpLazyServiceProvider LazyServiceProvider { get; set; } = default!;
        protected IStringLocalizerFactory StringLocalizerFactory => LazyServiceProvider.LazyGetRequiredService<IStringLocalizerFactory>();

        protected Type? LocalizationResource
        {
            get => _localizationResource;
            set
            {
                _localizationResource = value;
                _localizer = null;
            }
        }
        private Type? _localizationResource = typeof(DefaultResource);

        protected IStringLocalizer L
        {
            get
            {
                if (_localizer == null)
                {
                    _localizer = CreateLocalizer();
                }

                return _localizer;
            }
        }
        private IStringLocalizer? _localizer;

        protected virtual IStringLocalizer CreateLocalizer()
        {
            if (LocalizationResource != null)
            {
                return StringLocalizerFactory.Create(LocalizationResource);
            }

            var localizer = StringLocalizerFactory.CreateDefaultOrNull();
            if (localizer == null)
            {
                throw new AbpException($"Set {nameof(LocalizationResource)} or define the default localization resource type (by configuring the {nameof(AbpLocalizationOptions)}.{nameof(AbpLocalizationOptions.DefaultResourceType)}) to be able to use the {nameof(L)} object!");
            }

            return localizer;
        }

        #endregion Localization

        protected readonly IHierarchyComponentGroupRepository _hierarchyComponentGroupRepository;
        protected readonly IHierarchyRepository _hierarchyRepository;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly INecnatEndpointStore _necnatEndpointStore;

        public HierarchyComponentService(
            IHierarchyComponentGroupRepository hierarchyComponentGroupRepository,
            IHierarchyRepository hierarchyRepository,
            IHttpContextAccessor httpContextAccessor,
            INecnatEndpointStore necnatEndpointStore)
        {
            LocalizationResource = typeof(NnMgmtAuthorizationResource);

            _hierarchyComponentGroupRepository = hierarchyComponentGroupRepository;
            _hierarchyRepository = hierarchyRepository;
            _httpContextAccessor = httpContextAccessor;
            _necnatEndpointStore = necnatEndpointStore;
        }

        public virtual async Task<List<HierarchyComponentModel>> GetListHierarchyComponentAsync(Guid? hierarchyId = null)
        {
            var l = new List<HierarchyComponentModel>();

            var lHierarchy = await _hierarchyRepository.GetListAsync();
            foreach (var iHierarchy in lHierarchy)
                l.Add(new HierarchyComponentModel { HierarchyComponentType = 1, Id = iHierarchy.Id, Name = iHierarchy.Name });

            var lHierarchyComponentGroup = await _hierarchyComponentGroupRepository.GetListAsync();
            foreach (var iHierarchyComponentGroup in lHierarchyComponentGroup)
                l.Add(new HierarchyComponentModel { HierarchyComponentType = 2, Id = iHierarchyComponentGroup.Id, Name = iHierarchyComponentGroup.Name });

            //var necnatEndpointList = await _necnatEndpointStore.GetListAsync();
            //foreach (var iNecnatEndpoint in necnatEndpointList.Where(x => x.IsAuthz == true))
            //{
            //    using (HttpClient client = new HttpClient())
            //    {
            //        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await _httpContextAccessor.HttpContext.GetTokenAsync("access_token")}");
            //        var httpResponseMessage = await client.GetAsync($"{iNecnatEndpoint.Endpoint}/api/app/mgmt-authorization/permission-my");
            //        if (!httpResponseMessage.IsSuccessStatusCode)
            //            throw new Exception(await httpResponseMessage.Content.ReadAsStringAsync());

            //        permissionList.AddRange(JsonSerializer.Deserialize<List<string>>(await httpResponseMessage.Content.ReadAsStringAsync())!);
            //    }
            //}

            return l;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public virtual async Task<List<HierarchyComponentTypeModel>> GetListHierarchyComponentTypeAsync(Guid? hierarchyId = null)
        {
            return new List<HierarchyComponentTypeModel>
            {
                new HierarchyComponentTypeModel
                {
                    Id = 1,
                    Name = L["Hierarchy"],
                    Icon = "fas fa-box"
                },
                new HierarchyComponentTypeModel
                {
                    Id = 2,
                    Name = L["Hierarchy Component Group"],
                    Icon = "fas fa-sitemap"
                }
            };
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}
