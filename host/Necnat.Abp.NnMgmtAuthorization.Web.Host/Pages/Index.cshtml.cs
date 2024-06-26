using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Necnat.Abp.NnMgmtAuthorization.Pages;

public class IndexModel : NnMgmtAuthorizationPageModel
{
    public void OnGet()
    {

    }

    public async Task OnPostLoginAsync()
    {
        await HttpContext.ChallengeAsync("oidc");
    }
}
