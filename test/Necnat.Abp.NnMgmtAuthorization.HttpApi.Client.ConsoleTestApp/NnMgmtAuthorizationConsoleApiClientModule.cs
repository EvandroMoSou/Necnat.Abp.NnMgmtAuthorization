using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace Necnat.Abp.NnMgmtAuthorization;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(NnMgmtAuthorizationHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class NnMgmtAuthorizationConsoleApiClientModule : AbpModule
{

}
