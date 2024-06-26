using Volo.Abp.Reflection;

namespace Necnat.Abp.NnMgmtAuthorization.Permissions;

public class NnMgmtAuthorizationPermissions
{
    public const string GroupName = "NnMgmtAuthorization";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(NnMgmtAuthorizationPermissions));
    }
}
