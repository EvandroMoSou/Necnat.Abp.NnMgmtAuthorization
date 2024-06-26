using Necnat.Abp.NnMgmtAuthorization.Localization;
using Volo.Abp.AspNetCore.Components;

namespace Necnat.Abp.NnMgmtAuthorization.Blazor.Server.Host;

public abstract class NnMgmtAuthorizationComponentBase : AbpComponentBase
{
    protected NnMgmtAuthorizationComponentBase()
    {
        LocalizationResource = typeof(NnMgmtAuthorizationResource);
    }
}
