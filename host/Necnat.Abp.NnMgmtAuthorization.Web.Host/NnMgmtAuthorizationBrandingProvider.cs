using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace Necnat.Abp.NnMgmtAuthorization;

[Dependency(ReplaceServices = true)]
public class NnMgmtAuthorizationBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "NnMgmtAuthorization";
}
