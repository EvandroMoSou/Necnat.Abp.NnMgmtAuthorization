using Microsoft.Extensions.DependencyInjection;
using Necnat.Abp.NnLibCommon.Domains.NnIdentity;
using Necnat.Abp.NnMgmtAuthorization.Domains;

namespace Necnat.Abp.NnMgmtAuthorization.Extensions
{
    public static class ServiceCollectionExtension
    {

        public static IServiceCollection ConfigureAuthorizationServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IRoleNameService, RoleNameServiceCache>();
            serviceCollection.AddTransient<IHierarchyComponentService, HierarchyComponentService>();
            serviceCollection.AddTransient<IHierarchicalStructureRecursiveService, HierarchicalStructureRecursiveServiceCache>();

            return serviceCollection;
        }
    }
}
