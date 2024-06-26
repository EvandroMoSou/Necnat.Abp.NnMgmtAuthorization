using Necnat.Abp.NnMgmtAuthorization.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Necnat.Abp.NnMgmtAuthorization.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class NnMgmtAuthorizationPageModel : AbpPageModel
{
    protected NnMgmtAuthorizationPageModel()
    {
        LocalizationResourceType = typeof(NnMgmtAuthorizationResource);
        ObjectMapperContext = typeof(NnMgmtAuthorizationWebModule);
    }
}
