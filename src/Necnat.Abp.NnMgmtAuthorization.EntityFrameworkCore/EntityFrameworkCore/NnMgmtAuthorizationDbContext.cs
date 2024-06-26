using Microsoft.EntityFrameworkCore;
using Necnat.Abp.NnMgmtAuthorization.Domains;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Necnat.Abp.NnMgmtAuthorization.EntityFrameworkCore;

[ConnectionStringName(NnMgmtAuthorizationDbProperties.ConnectionStringName)]
public class NnMgmtAuthorizationDbContext : AbpDbContext<NnMgmtAuthorizationDbContext>, INnMgmtAuthorizationDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * public DbSet<Question> Questions { get; set; }
     */

    public DbSet<AuthEndpoint> AuthEndpoint { get; set; }

    public NnMgmtAuthorizationDbContext(DbContextOptions<NnMgmtAuthorizationDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureNnMgmtAuthorization();
    }
}
