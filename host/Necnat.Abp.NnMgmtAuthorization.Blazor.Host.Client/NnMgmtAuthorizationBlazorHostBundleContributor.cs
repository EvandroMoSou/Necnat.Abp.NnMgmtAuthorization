using Volo.Abp.Bundling;

namespace Necnat.Abp.NnMgmtAuthorization.Blazor.Host.Client;

public class NnMgmtAuthorizationBlazorHostBundleContributor : IBundleContributor
{
    public void AddScripts(BundleContext context)
    {

    }

    public void AddStyles(BundleContext context)
    {
        context.Add("main.css", true);
    }
}
