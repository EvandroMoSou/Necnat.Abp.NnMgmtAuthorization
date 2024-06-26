using Necnat.Abp.NnMgmtAuthorization.Localization;
using Volo.Abp.Application.Services;

namespace Necnat.Abp.NnMgmtAuthorization;

public abstract class NnMgmtAuthorizationAppService : ApplicationService
{
    protected NnMgmtAuthorizationAppService()
    {
        LocalizationResource = typeof(NnMgmtAuthorizationResource);
        ObjectMapperContext = typeof(NnMgmtAuthorizationApplicationModule);
    }
}
