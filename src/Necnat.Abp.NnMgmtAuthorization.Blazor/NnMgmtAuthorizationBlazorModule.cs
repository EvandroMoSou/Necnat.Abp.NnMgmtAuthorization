using Microsoft.Extensions.DependencyInjection;
using Necnat.Abp.NnMgmtAuthorization.Blazor.Menus;
using Volo.Abp.AspNetCore.Components.Web.Theming;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;

namespace Necnat.Abp.NnMgmtAuthorization.Blazor;

[DependsOn(
    typeof(NnMgmtAuthorizationApplicationContractsModule),
    typeof(AbpAspNetCoreComponentsWebThemingModule),
    typeof(AbpAutoMapperModule)
    )]
public class NnMgmtAuthorizationBlazorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<NnMgmtAuthorizationBlazorModule>();

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddProfile<NnMgmtAuthorizationBlazorAutoMapperProfile>(validate: true);
        });

        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new NnMgmtAuthorizationMenuContributor());
        });

        Configure<AbpRouterOptions>(options =>
        {
            options.AdditionalAssemblies.Add(typeof(NnMgmtAuthorizationBlazorModule).Assembly);
        });
    }
}
