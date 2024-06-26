using Necnat.Abp.NnMgmtAuthorization.Localization;
using System.Threading.Tasks;
using Volo.Abp.Identity;
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

    private async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<NnMgmtAuthorizationResource>();

        bool displayauthorizationMenu = false;
        var authorizationMenu = new ApplicationMenuItem(
            NnMgmtAuthorizationMenus.Prefix,
            l["Menu:NnMgmtAuthorization"],
            icon: "fas fa-user-shield"
        );

        bool displayAuthorizationConfiguracaoMenu = false;
        var authorizationConfiguracaoMenu = new ApplicationMenuItem(
            NnMgmtAuthorizationMenus.Configuration,
            l["Menu:NnMgmtAuthorization:Configuration"],
            order: 1
        );

        if (await context.IsGrantedAsync(IdentityPermissions.Users.Default))
        {
            authorizationConfiguracaoMenu.AddItem(new ApplicationMenuItem(
                NnMgmtAuthorizationMenus.Configuration_User,
                l["Menu:NnMgmtAuthorization:Configuration:User"],
                url: "/NnMgmtAuthorization/Configuration/Users",
                order: 1
            ));
            displayAuthorizationConfiguracaoMenu = true;
        }

        if (displayAuthorizationConfiguracaoMenu)
            authorizationMenu.AddItem(authorizationConfiguracaoMenu);

        if (displayauthorizationMenu || displayAuthorizationConfiguracaoMenu)
            context.Menu.AddItem(authorizationMenu);
    }
}
