using Volo.Abp.AspNetCore.Components.Server.Theming;
using Volo.Abp.Modularity;

namespace Necnat.Abp.NnMgmtAuthorization.Blazor.Server;

[DependsOn(
    typeof(AbpAspNetCoreComponentsServerThemingModule),
    typeof(NnMgmtAuthorizationBlazorModule)
    )]
public class NnMgmtAuthorizationBlazorServerModule : AbpModule
{

}
