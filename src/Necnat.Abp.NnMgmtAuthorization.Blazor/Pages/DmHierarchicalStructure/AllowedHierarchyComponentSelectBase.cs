using Microsoft.AspNetCore.Components;
using Necnat.Abp.NnLibCommon.Blazor.Components;
using Necnat.Abp.NnMgmtAuthorization.Domains;
using Necnat.Abp.NnMgmtAuthorization.HierarchicalPermissions;
using Necnat.Abp.NnMgmtAuthorization.Localization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Necnat.Abp.NnMgmtAuthorization.Blazor.Components
{
    public abstract class AllowedHierarchyComponentSelectBase : SelectedEntityDtoCmptBase<HierarchyComponentDto, Guid>
    {
        [Inject] protected IHierarchicalAuthorizationService HierarchicalAuthorizationService { get; set; } = default!;
        [Inject] protected IHierarchicalStructureAppService HierarchicalStructureAppService { get; set; } = default!;

        [Parameter]
        public string PermissionName { get; set; } = string.Empty;

        [Parameter]
        public int? HierarchyComponentTypeId { get; set; }

        [Parameter]
        public string? Label { get; set; }

        protected int _dataCount;
        public AllowedHierarchyComponentSelectBase()
        {
            LocalizationResource = typeof(NnMgmtAuthorizationResource);
        }

        protected override async Task OnInitializedAsync()
        {
            Data = await HierarchicalAuthorizationService.GetListHierarchyComponentAsync(PermissionName, HierarchyComponentTypeId);
            _dataCount = Data.Count;

            if (Data.Count == 1)
                await OnSelectedValueChangedAsync(Data.First().Id);

            _isLoading = false;
        }
    }
}