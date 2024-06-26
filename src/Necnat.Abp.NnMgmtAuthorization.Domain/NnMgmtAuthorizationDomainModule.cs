using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Necnat.Abp.NnMgmtAuthorization;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(NnMgmtAuthorizationDomainSharedModule)
)]
public class NnMgmtAuthorizationDomainModule : AbpModule
{

}
