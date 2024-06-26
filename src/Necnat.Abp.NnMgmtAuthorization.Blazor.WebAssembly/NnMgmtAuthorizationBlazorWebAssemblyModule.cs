using Volo.Abp.AspNetCore.Components.WebAssembly.Theming;
using Volo.Abp.Modularity;

namespace Necnat.Abp.NnMgmtAuthorization.Blazor.WebAssembly;

[DependsOn(
    typeof(NnMgmtAuthorizationBlazorModule),
    typeof(NnMgmtAuthorizationHttpApiClientModule),
    typeof(AbpAspNetCoreComponentsWebAssemblyThemingModule)
    )]
public class NnMgmtAuthorizationBlazorWebAssemblyModule : AbpModule
{

}
