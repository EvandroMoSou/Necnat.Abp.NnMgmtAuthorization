using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Necnat.Abp.NnMgmtAuthorization.EntityFrameworkCore;

[ConnectionStringName(NnMgmtAuthorizationDbProperties.ConnectionStringName)]
public interface INnMgmtAuthorizationDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
}
