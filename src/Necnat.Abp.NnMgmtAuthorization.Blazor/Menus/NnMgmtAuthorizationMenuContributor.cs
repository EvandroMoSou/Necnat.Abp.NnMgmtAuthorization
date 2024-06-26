using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;

namespace Necnat.Abp.NnMgmtAuthorization.Blazor.Menus;

public class NnMgmtAuthorizationMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        //Add main menu items.
        context.Menu.AddItem(new ApplicationMenuItem(NnMgmtAuthorizationMenus.Prefix, displayName: "NnMgmtAuthorization", "/NnMgmtAuthorization", icon: "fa fa-globe"));

        return Task.CompletedTask;
    }
}
