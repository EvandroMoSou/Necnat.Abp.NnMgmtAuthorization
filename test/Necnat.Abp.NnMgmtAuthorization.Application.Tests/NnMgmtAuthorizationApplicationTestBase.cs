using Volo.Abp.Modularity;

namespace Necnat.Abp.NnMgmtAuthorization;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class NnMgmtAuthorizationApplicationTestBase<TStartupModule> : NnMgmtAuthorizationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
