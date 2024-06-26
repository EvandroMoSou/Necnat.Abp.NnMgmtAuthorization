using Microsoft.EntityFrameworkCore;
using Necnat.Abp.NnMgmtAuthorization.Domains;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Necnat.Abp.NnMgmtAuthorization.EntityFrameworkCore;

[ConnectionStringName(NnMgmtAuthorizationDbProperties.ConnectionStringName)]
public interface INnMgmtAuthorizationDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */

    DbSet<AuthEndpoint> AuthEndpoint { get; }
    DbSet<HierarchicalAccess> HierarchicalAccess { get; }
    DbSet<HierarchicalStructure> HierarchicalStructure { get; }
    DbSet<Hierarchy> Hierarchy { get; }
    DbSet<HierarchyComponentGroup> HierarchyComponentGroup { get; }
}
