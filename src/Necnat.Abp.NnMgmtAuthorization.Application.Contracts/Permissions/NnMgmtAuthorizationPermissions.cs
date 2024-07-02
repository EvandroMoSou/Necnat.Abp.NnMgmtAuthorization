using Volo.Abp.Reflection;

namespace Necnat.Abp.NnMgmtAuthorization.Permissions;

public class NnMgmtAuthorizationPermissions
{
    public const string GroupName = "NnMgmtAuthorization";

    public static class PrmHierarchy
    {
        public const string Default = GroupName + ".Hierarchy";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class PrmHierarchyComponentGroup
    {
        public const string Default = GroupName + ".HierarchyComponentGroup";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class PrmHierarchicalStructure
    {
        public const string Default = GroupName + ".HierarchicalStructure";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class PrmHierarchicalAccess
    {
        public const string Default = GroupName + ".HierarchicalAccess";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(NnMgmtAuthorizationPermissions));
    }
}
