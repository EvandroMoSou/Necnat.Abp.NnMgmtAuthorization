using Necnat.Abp.NnMgmtAuthorization.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Necnat.Abp.NnMgmtAuthorization.Permissions;

public class NnMgmtAuthorizationPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(NnMgmtAuthorizationPermissions.GroupName, L("Permission:NnMgmtAuthorization"));

        var pgAuthEndpoint = myGroup.AddPermission(NnMgmtAuthorizationPermissions.PrmAuthEndpoint.Default, L("Permission:AuthEndpoint:Default"));
        pgAuthEndpoint.AddChild(NnMgmtAuthorizationPermissions.PrmAuthEndpoint.Create, L("Permission:AuthEndpoint:Create"));
        pgAuthEndpoint.AddChild(NnMgmtAuthorizationPermissions.PrmAuthEndpoint.Update, L("Permission:AuthEndpoint:Update"));
        pgAuthEndpoint.AddChild(NnMgmtAuthorizationPermissions.PrmAuthEndpoint.Delete, L("Permission:AuthEndpoint:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<NnMgmtAuthorizationResource>(name);
    }
}
