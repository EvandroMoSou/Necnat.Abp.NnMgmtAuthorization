using Microsoft.Extensions.DependencyInjection;
using Necnat.Abp.NnMgmtAuthorization.Domains;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Necnat.Abp.NnMgmtAuthorization;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(NnMgmtAuthorizationDomainSharedModule)
)]
public class NnMgmtAuthorizationDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddTransient<IPermissionRoleIdService, PermissionRoleIdServiceCache>();
        context.Services.AddTransient<IHierarchyComponentService, HierarchyComponentService>();
        context.Services.AddTransient<IHierarchicalStructureRecursiveService, HierarchicalStructureRecursiveServiceCache>();        
    }
}
