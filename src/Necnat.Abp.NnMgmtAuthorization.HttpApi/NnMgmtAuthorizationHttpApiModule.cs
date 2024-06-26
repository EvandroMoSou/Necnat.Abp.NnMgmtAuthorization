using Localization.Resources.AbpUi;
using Necnat.Abp.NnMgmtAuthorization.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace Necnat.Abp.NnMgmtAuthorization;

[DependsOn(
    typeof(NnMgmtAuthorizationApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class NnMgmtAuthorizationHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(NnMgmtAuthorizationHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<NnMgmtAuthorizationResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
