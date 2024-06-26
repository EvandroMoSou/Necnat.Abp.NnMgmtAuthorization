using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Necnat.Abp.NnMgmtAuthorization.Blazor.Server.Host;

[Dependency(ReplaceServices = true)]
public class NnMgmtAuthorizationBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "NnMgmtAuthorization";
}
