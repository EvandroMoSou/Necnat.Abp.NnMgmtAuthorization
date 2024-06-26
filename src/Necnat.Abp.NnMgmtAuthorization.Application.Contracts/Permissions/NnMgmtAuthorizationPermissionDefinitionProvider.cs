using Necnat.Abp.NnMgmtAuthorization.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Necnat.Abp.NnMgmtAuthorization.Permissions;

public class NnMgmtAuthorizationPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(NnMgmtAuthorizationPermissions.GroupName, L("Permission:NnMgmtAuthorization"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<NnMgmtAuthorizationResource>(name);
    }
}
