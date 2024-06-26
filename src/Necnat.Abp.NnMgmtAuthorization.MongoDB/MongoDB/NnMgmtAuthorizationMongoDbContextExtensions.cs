using Volo.Abp;
using Volo.Abp.MongoDB;

namespace Necnat.Abp.NnMgmtAuthorization.MongoDB;

public static class NnMgmtAuthorizationMongoDbContextExtensions
{
    public static void ConfigureNnMgmtAuthorization(
        this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
    }
}
