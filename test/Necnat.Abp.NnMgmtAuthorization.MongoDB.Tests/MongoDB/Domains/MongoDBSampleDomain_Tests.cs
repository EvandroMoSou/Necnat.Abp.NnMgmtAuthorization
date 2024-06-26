using Necnat.Abp.NnMgmtAuthorization.Samples;
using Xunit;

namespace Necnat.Abp.NnMgmtAuthorization.MongoDB.Domains;

[Collection(MongoTestCollection.Name)]
public class MongoDBSampleDomain_Tests : SampleManager_Tests<NnMgmtAuthorizationMongoDbTestModule>
{

}
