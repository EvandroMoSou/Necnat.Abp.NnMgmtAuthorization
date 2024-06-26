using System;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;

namespace Necnat.Abp.NnMgmtAuthorization.MongoDB;

[DependsOn(
    typeof(NnMgmtAuthorizationApplicationTestModule),
    typeof(NnMgmtAuthorizationMongoDbModule)
)]
public class NnMgmtAuthorizationMongoDbTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = MongoDbFixture.GetRandomConnectionString();
        });
    }
}
