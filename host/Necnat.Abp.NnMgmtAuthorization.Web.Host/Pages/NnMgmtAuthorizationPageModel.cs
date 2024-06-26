using Necnat.Abp.NnMgmtAuthorization.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Necnat.Abp.NnMgmtAuthorization.Pages;

public abstract class NnMgmtAuthorizationPageModel : AbpPageModel
{
    protected NnMgmtAuthorizationPageModel()
    {
        LocalizationResourceType = typeof(NnMgmtAuthorizationResource);
    }
}
