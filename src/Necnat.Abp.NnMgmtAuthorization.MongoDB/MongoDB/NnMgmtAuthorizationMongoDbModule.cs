using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;

namespace Necnat.Abp.NnMgmtAuthorization.MongoDB;

[DependsOn(
    typeof(NnMgmtAuthorizationDomainModule),
    typeof(AbpMongoDbModule)
    )]
public class NnMgmtAuthorizationMongoDbModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMongoDbContext<NnMgmtAuthorizationMongoDbContext>(options =>
        {
                /* Add custom repositories here. Example:
                 * options.AddRepository<Question, MongoQuestionRepository>();
                 */
        });
    }
}
