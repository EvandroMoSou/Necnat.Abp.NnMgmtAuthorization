using Volo.Abp.Modularity;

namespace Necnat.Abp.NnMgmtAuthorization;

[DependsOn(
    typeof(NnMgmtAuthorizationApplicationModule),
    typeof(NnMgmtAuthorizationDomainTestModule)
    )]
public class NnMgmtAuthorizationApplicationTestModule : AbpModule
{

}
