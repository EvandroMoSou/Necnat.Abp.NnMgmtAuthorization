using Volo.Abp.Modularity;

namespace Necnat.Abp.NnMgmtAuthorization;

[DependsOn(
    typeof(NnMgmtAuthorizationDomainModule),
    typeof(NnMgmtAuthorizationTestBaseModule)
)]
public class NnMgmtAuthorizationDomainTestModule : AbpModule
{

}
