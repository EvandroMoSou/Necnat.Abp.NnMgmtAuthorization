using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Necnat.Abp.NnMgmtAuthorization.EntityFrameworkCore;

[DependsOn(
    typeof(NnMgmtAuthorizationDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class NnMgmtAuthorizationEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<NnMgmtAuthorizationDbContext>(options =>
        {
                /* Add custom repositories here. Example:
                 * options.AddRepository<Question, EfCoreQuestionRepository>();
                 */
        });
    }
}
