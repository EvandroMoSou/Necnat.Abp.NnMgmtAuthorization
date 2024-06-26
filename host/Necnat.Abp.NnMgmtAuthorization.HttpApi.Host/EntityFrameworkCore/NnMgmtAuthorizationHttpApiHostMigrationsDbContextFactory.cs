using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Necnat.Abp.NnMgmtAuthorization.EntityFrameworkCore;

public class NnMgmtAuthorizationHttpApiHostMigrationsDbContextFactory : IDesignTimeDbContextFactory<NnMgmtAuthorizationHttpApiHostMigrationsDbContext>
{
    public NnMgmtAuthorizationHttpApiHostMigrationsDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<NnMgmtAuthorizationHttpApiHostMigrationsDbContext>()
            .UseSqlServer(configuration.GetConnectionString("NnMgmtAuthorization"));

        return new NnMgmtAuthorizationHttpApiHostMigrationsDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
