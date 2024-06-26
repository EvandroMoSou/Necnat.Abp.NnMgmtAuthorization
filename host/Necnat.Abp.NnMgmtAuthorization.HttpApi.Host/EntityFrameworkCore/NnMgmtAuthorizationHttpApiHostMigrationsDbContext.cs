using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Necnat.Abp.NnMgmtAuthorization.EntityFrameworkCore;

public class NnMgmtAuthorizationHttpApiHostMigrationsDbContext : AbpDbContext<NnMgmtAuthorizationHttpApiHostMigrationsDbContext>
{
    public NnMgmtAuthorizationHttpApiHostMigrationsDbContext(DbContextOptions<NnMgmtAuthorizationHttpApiHostMigrationsDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureNnMgmtAuthorization();
    }
}
