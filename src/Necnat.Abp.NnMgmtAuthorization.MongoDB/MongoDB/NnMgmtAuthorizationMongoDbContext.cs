using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace Necnat.Abp.NnMgmtAuthorization.MongoDB;

[ConnectionStringName(NnMgmtAuthorizationDbProperties.ConnectionStringName)]
public class NnMgmtAuthorizationMongoDbContext : AbpMongoDbContext, INnMgmtAuthorizationMongoDbContext
{
    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        modelBuilder.ConfigureNnMgmtAuthorization();
    }
}
