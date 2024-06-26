using Volo.Abp.Modularity;

namespace Necnat.Abp.NnMgmtAuthorization;

/* Inherit from this class for your domain layer tests.
 * See SampleManager_Tests for example.
 */
public abstract class NnMgmtAuthorizationDomainTestBase<TStartupModule> : NnMgmtAuthorizationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
