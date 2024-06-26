using Microsoft.EntityFrameworkCore;
using Necnat.Abp.NnMgmtAuthorization.Domains;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Necnat.Abp.NnMgmtAuthorization.EntityFrameworkCore;

public static class NnMgmtAuthorizationDbContextModelCreatingExtensions
{
    public static void ConfigureNnMgmtAuthorization(
        this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        /* Configure all entities here. Example:

        builder.Entity<Question>(b =>
        {
            //Configure table & schema name
            b.ToTable(NnMgmtAuthorizationDbProperties.DbTablePrefix + "Questions", NnMgmtAuthorizationDbProperties.DbSchema);

            b.ConfigureByConvention();

            //Properties
            b.Property(q => q.Title).IsRequired().HasMaxLength(QuestionConsts.MaxTitleLength);

            //Relations
            b.HasMany(question => question.Tags).WithOne().HasForeignKey(qt => qt.QuestionId);

            //Indexes
            b.HasIndex(q => q.CreationTime);
        });
        */

        builder.Entity<AuthEndpoint>(b =>
        {
            b.ToTable(NnMgmtAuthorizationDbProperties.DbTablePrefix + "AuthEndpoint",
                NnMgmtAuthorizationDbProperties.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.DisplayName).HasMaxLength(AuthEndpointConsts.MaxDisplayNameLength);
            b.Property(x => x.Endpoint).IsRequired().HasMaxLength(AuthEndpointConsts.MaxEndpointLength);
            b.Property(x => x.IsAuthentication).IsRequired();
            b.Property(x => x.IsActive).IsRequired();
        });
    }
}
