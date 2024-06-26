using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Necnat.Abp.NnMgmtAuthorization.Localization;
using Necnat.Abp.NnMgmtAuthorization.Web.Menus;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;
using Necnat.Abp.NnMgmtAuthorization.Permissions;

namespace Necnat.Abp.NnMgmtAuthorization.Web;

[DependsOn(
    typeof(NnMgmtAuthorizationApplicationContractsModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpAutoMapperModule)
    )]
public class NnMgmtAuthorizationWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(typeof(NnMgmtAuthorizationResource), typeof(NnMgmtAuthorizationWebModule).Assembly);
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(NnMgmtAuthorizationWebModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new NnMgmtAuthorizationMenuContributor());
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<NnMgmtAuthorizationWebModule>();
        });

        context.Services.AddAutoMapperObjectMapper<NnMgmtAuthorizationWebModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<NnMgmtAuthorizationWebModule>(validate: true);
        });

        Configure<RazorPagesOptions>(options =>
        {
                //Configure authorization.
            });
    }
}
