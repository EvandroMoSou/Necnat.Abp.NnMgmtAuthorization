using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Localization;
using Volo.Abp;
using Necnat.Abp.NnMgmtAuthorization.Models;
using Necnat.Abp.NnMgmtAuthorization.Localization;

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

        public HierarchyComponentService(
            IHierarchyComponentGroupRepository hierarchyComponentGroupRepository,
            IHierarchyRepository hierarchyRepository)
        {
            LocalizationResource = typeof(NnMgmtAuthorizationResource);

            _hierarchyComponentGroupRepository = hierarchyComponentGroupRepository;
            _hierarchyRepository = hierarchyRepository;
        }

        public virtual async Task<List<HierarchyComponentModel>> GetListHierarchyComponentAsync()
        {
            var l = new List<HierarchyComponentModel>();

            var lHierarchy = await _hierarchyRepository.GetListAsync();
            foreach (var iHierarchy in lHierarchy)
                l.Add(new HierarchyComponentModel { HierarchyComponentType = 1, Id = iHierarchy.Id, Name = iHierarchy.Name });

            var lHierarchyComponentGroup = await _hierarchyComponentGroupRepository.GetListAsync();
            foreach (var iHierarchyComponentGroup in lHierarchyComponentGroup)
                l.Add(new HierarchyComponentModel { HierarchyComponentType = 2, Id = iHierarchyComponentGroup.Id, Name = iHierarchyComponentGroup.Name });

            return l;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public virtual async Task<List<HierarchyComponentTypeModel>> GetListHierarchyComponentTypeAsync()
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
