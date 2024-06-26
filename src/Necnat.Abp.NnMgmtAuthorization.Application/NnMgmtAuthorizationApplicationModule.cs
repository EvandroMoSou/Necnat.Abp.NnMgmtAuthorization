using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.Application;

namespace Necnat.Abp.NnMgmtAuthorization;

[DependsOn(
    typeof(NnMgmtAuthorizationDomainModule),
    typeof(NnMgmtAuthorizationApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule)
    )]
public class NnMgmtAuthorizationApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<NnMgmtAuthorizationApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<NnMgmtAuthorizationApplicationModule>(validate: true);
        });
    }
}
