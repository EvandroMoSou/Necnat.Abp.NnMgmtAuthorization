using Microsoft.EntityFrameworkCore;
using Necnat.Abp.NnMgmtAuthorization.Domains;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Identity;

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

        builder.Entity<HierarchicalAccess>(b =>
        {
            b.ToTable(NnMgmtAuthorizationDbProperties.DbTablePrefix + "HierarchicalAccess",
                NnMgmtAuthorizationDbProperties.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props

            b.HasOne<IdentityRole>().WithMany().HasForeignKey(ur => ur.RoleId).IsRequired();
            b.HasOne<IdentityUser>().WithMany().HasForeignKey(ur => ur.UserId).IsRequired();
            b.HasOne<HierarchicalStructure>().WithMany().HasForeignKey(ur => ur.HierarchicalStructureId).IsRequired();

            b.HasIndex(ur => new { ur.UserId, ur.RoleId, ur.HierarchicalStructureId }).IsUnique();
        });

        builder.Entity<HierarchicalStructure>(b =>
        {
            b.ToTable(NnMgmtAuthorizationDbProperties.DbTablePrefix + "HierarchicalStructure",
                NnMgmtAuthorizationDbProperties.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props

            b.HasOne(o => o.HierarchicalStructureParent).WithMany().HasForeignKey(x => x.HierarchicalStructureIdParent).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            b.HasOne(o => o.Hierarchy).WithMany().HasForeignKey(x => x.HierarchyId).IsRequired().OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Hierarchy>(b =>
        {
            b.ToTable(NnMgmtAuthorizationDbProperties.DbTablePrefix + "Hierarchy",
                NnMgmtAuthorizationDbProperties.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.Name).IsRequired().HasMaxLength(HierarchyConsts.MaxNameLength);
            b.Property(x => x.IsActive).IsRequired();

            b.HasIndex(x => x.Name);
        });

        builder.Entity<HierarchyComponentGroup>(b =>
        {
            b.ToTable(NnMgmtAuthorizationDbProperties.DbTablePrefix + "HierarchyComponentGroup",
                NnMgmtAuthorizationDbProperties.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.Name).IsRequired().HasMaxLength(HierarchyComponentGroupConsts.MaxNameLength);
            b.Property(x => x.IsActive).IsRequired();

            b.HasIndex(x => x.Name);
        });
    }
}
