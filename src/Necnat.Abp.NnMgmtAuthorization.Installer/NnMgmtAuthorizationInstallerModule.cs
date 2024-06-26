using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Necnat.Abp.NnMgmtAuthorization;

[DependsOn(
    typeof(AbpVirtualFileSystemModule)
    )]
public class NnMgmtAuthorizationInstallerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<NnMgmtAuthorizationInstallerModule>();
        });
    }
}
