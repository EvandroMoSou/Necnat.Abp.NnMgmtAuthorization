using Volo.Abp.Reflection;

namespace Necnat.Abp.NnMgmtAuthorization.Permissions;

public class NnMgmtAuthorizationPermissions
{
    public const string GroupName = "NnMgmtAuthorization";

    public static class PrmAuthEndpoint
    {
        public const string Default = GroupName + ".AuthEndpoint";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(NnMgmtAuthorizationPermissions));
    }
}
