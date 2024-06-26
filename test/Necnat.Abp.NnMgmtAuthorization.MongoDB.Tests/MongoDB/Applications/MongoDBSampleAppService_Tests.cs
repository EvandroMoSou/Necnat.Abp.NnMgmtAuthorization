using Necnat.Abp.NnMgmtAuthorization.MongoDB;
using Necnat.Abp.NnMgmtAuthorization.Samples;
using Xunit;

namespace Necnat.Abp.NnMgmtAuthorization.MongoDb.Applications;

[Collection(MongoTestCollection.Name)]
public class MongoDBSampleAppService_Tests : SampleAppService_Tests<NnMgmtAuthorizationMongoDbTestModule>
{

}
