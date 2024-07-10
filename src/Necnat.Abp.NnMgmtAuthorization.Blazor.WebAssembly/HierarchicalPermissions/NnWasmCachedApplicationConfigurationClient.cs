using Microsoft.JSInterop;
using Necnat.Abp.NnMgmtAuthorization.Domains;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Components.Web.Security;
using Volo.Abp.AspNetCore.Components.WebAssembly;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations.ClientProxies;
using Volo.Abp.AspNetCore.Mvc.Client;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Necnat.Abp.NnMgmtAuthorization.Blazor.WebAssembly.HierarchicalPermissions
{
    public class NnWasmCachedApplicationConfigurationClient : WebAssemblyCachedApplicationConfigurationClient, ICachedApplicationConfigurationClient
    {
        protected readonly IMgmtAuthorizationAppService mgmtAuthorizationAppService;

        public NnWasmCachedApplicationConfigurationClient(
            AbpApplicationConfigurationClientProxy applicationConfigurationClientProxy,
            ApplicationConfigurationCache cache,
            ICurrentTenantAccessor currentTenantAccessor,
            AbpApplicationLocalizationClientProxy applicationLocalizationClientProxy,
            ApplicationConfigurationChangedService applicationConfigurationChangedService,
            IJSRuntime jsRuntime,
            IMgmtAuthorizationAppService _mgmtAuthorizationAppService) : base(applicationConfigurationClientProxy, cache, currentTenantAccessor, applicationLocalizationClientProxy, applicationConfigurationChangedService, jsRuntime)
        {
            mgmtAuthorizationAppService = _mgmtAuthorizationAppService;
        }

        public override async Task InitializeAsync()
        {
            var configurationDto = await ApplicationConfigurationClientProxy.GetAsync(
                new ApplicationConfigurationRequestOptions
                {
                    IncludeLocalizationResources = false
                }
            );

            var localizationDto = await ApplicationLocalizationClientProxy.GetAsync(
                new ApplicationLocalizationRequestDto
                {
                    CultureName = configurationDto.Localization.CurrentCulture.Name,
                    OnlyDynamics = true
                }
            );

            configurationDto.Localization.Resources = localizationDto.Resources;

            if (configurationDto.CurrentUser.IsAuthenticated)
            {
                var hierarchicalAuthorization = await mgmtAuthorizationAppService.GetHierarchicalAuthorizationMyAsync();
                configurationDto.Setting.Values.Add("ua:userId", JsonSerializer.Serialize(hierarchicalAuthorization.UserId));
                configurationDto.Setting.Values.Add("ua:lhac", JsonSerializer.Serialize(hierarchicalAuthorization.LHAC));
                configurationDto.Setting.Values.Add("ua:lhs", JsonSerializer.Serialize(hierarchicalAuthorization.LHS));
                configurationDto.Setting.Values.Add("ua:lhc", JsonSerializer.Serialize(hierarchicalAuthorization.LHC));
            }

            Cache.Set(configurationDto);

            if (!configurationDto.CurrentUser.IsAuthenticated)
            {
                await JSRuntime.InvokeVoidAsync("abp.utils.removeOidcUser");
            }

            ApplicationConfigurationChangedService.NotifyChanged();

            CurrentTenantAccessor.Current = new BasicTenantInfo(
                configurationDto.CurrentTenant.Id,
                configurationDto.CurrentTenant.Name
            );
        }
    }
}
