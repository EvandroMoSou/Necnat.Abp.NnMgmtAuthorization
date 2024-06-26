using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.Authorization;

namespace Necnat.Abp.NnMgmtAuthorization;

[DependsOn(
    typeof(NnMgmtAuthorizationDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule)
    )]
public class NnMgmtAuthorizationApplicationContractsModule : AbpModule
{

}
