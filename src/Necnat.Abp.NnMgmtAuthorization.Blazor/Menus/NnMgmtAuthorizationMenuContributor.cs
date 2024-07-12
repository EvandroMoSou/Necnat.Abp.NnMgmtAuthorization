using Necnat.Abp.NnMgmtAuthorization.Localization;
using Necnat.Abp.NnMgmtAuthorization.Permissions;
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

        if (await context.IsGrantedAsync(NnMgmtAuthorizationPermissions.PrmHierarchy.Default))
        {
            authorizationConfiguracaoMenu.AddItem(new ApplicationMenuItem(
                NnMgmtAuthorizationMenus.Configuration_Hierarchy,
                l["Menu:NnMgmtAuthorization:Configuration:Hierarchy"],
                url: "/NnMgmtAuthorization/Configuration/Hierarchies",
                order: 1
            ));
            displayAuthorizationConfiguracaoMenu = true;
        }

        if (await context.IsGrantedAsync(NnMgmtAuthorizationPermissions.PrmHierarchyComponentGroup.Default))
        {
            authorizationConfiguracaoMenu.AddItem(new ApplicationMenuItem(
                NnMgmtAuthorizationMenus.Configuration_HierarchyComponentGroup,
                l["Menu:NnMgmtAuthorization:Configuration:HierarchyComponentGroup"],
                url: "/NnMgmtAuthorization/Configuration/HierarchyComponentGroups",
                order: 2
            ));
            displayAuthorizationConfiguracaoMenu = true;
        }

        if (await context.IsGrantedAsync(NnMgmtAuthorizationPermissions.PrmHierarchicalStructure.Default))
        {
            authorizationConfiguracaoMenu.AddItem(new ApplicationMenuItem(
                NnMgmtAuthorizationMenus.Configuration_HierarchicalStructure,
                l["Menu:NnMgmtAuthorization:Configuration:HierarchicalStructure"],
                url: "/NnMgmtAuthorization/Configuration/HierarchicalStructure",
                order: 3
            ));
            displayAuthorizationConfiguracaoMenu = true;
        }

        if (displayAuthorizationConfiguracaoMenu)
            authorizationMenu.AddItem(authorizationConfiguracaoMenu);

        if (await context.IsGrantedAsync(IdentityPermissions.Users.Default))
        {
            authorizationMenu.AddItem(new ApplicationMenuItem(
                NnMgmtAuthorizationMenus.HierarchicalAccesses,
                l["Menu:NnMgmtAuthorization:HierarchicalAccess"],
                url: "/NnMgmtAuthorization/HierarchicalAccesses",
                order: 2
            ));
            displayauthorizationMenu = true;
        }

        if (displayauthorizationMenu || displayAuthorizationConfiguracaoMenu)
            context.Menu.AddItem(authorizationMenu);
    }
}
