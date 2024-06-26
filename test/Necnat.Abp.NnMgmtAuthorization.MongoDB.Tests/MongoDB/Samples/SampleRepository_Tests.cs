using Necnat.Abp.NnMgmtAuthorization.Samples;
using Xunit;

namespace Necnat.Abp.NnMgmtAuthorization.MongoDB.Samples;

[Collection(MongoTestCollection.Name)]
public class SampleRepository_Tests : SampleRepository_Tests<NnMgmtAuthorizationMongoDbTestModule>
{
    /* Don't write custom repository tests here, instead write to
     * the base class.
     * One exception can be some specific tests related to MongoDB.
     */
}
