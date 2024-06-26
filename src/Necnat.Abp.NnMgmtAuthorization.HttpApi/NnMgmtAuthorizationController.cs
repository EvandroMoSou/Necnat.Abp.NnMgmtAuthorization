using Necnat.Abp.NnMgmtAuthorization.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Necnat.Abp.NnMgmtAuthorization;

public abstract class NnMgmtAuthorizationController : AbpControllerBase
{
    protected NnMgmtAuthorizationController()
    {
        LocalizationResource = typeof(NnMgmtAuthorizationResource);
    }
}
