using Necnat.Abp.NnMgmtAuthorization.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Necnat.Abp.NnMgmtAuthorization.Permissions;

public class NnMgmtAuthorizationPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(NnMgmtAuthorizationPermissions.GroupName, L("Permission:NnMgmtAuthorization"));

        var pgHierarchy = myGroup.AddPermission(NnMgmtAuthorizationPermissions.PrmHierarchy.Default, L("Permission:Hierarchy:Default"));
        pgHierarchy.AddChild(NnMgmtAuthorizationPermissions.PrmHierarchy.Create, L("Permission:Hierarchy:Create"));
        pgHierarchy.AddChild(NnMgmtAuthorizationPermissions.PrmHierarchy.Update, L("Permission:Hierarchy:Edit"));
        pgHierarchy.AddChild(NnMgmtAuthorizationPermissions.PrmHierarchy.Delete, L("Permission:Hierarchy:Delete"));

        var pgHierarchyComponentGroup = myGroup.AddPermission(NnMgmtAuthorizationPermissions.PrmHierarchyComponentGroup.Default, L("Permission:HierarchyComponentGroup:Default"));
        pgHierarchyComponentGroup.AddChild(NnMgmtAuthorizationPermissions.PrmHierarchyComponentGroup.Create, L("Permission:HierarchyComponentGroup:Create"));
        pgHierarchyComponentGroup.AddChild(NnMgmtAuthorizationPermissions.PrmHierarchyComponentGroup.Update, L("Permission:HierarchyComponentGroup:Edit"));
        pgHierarchyComponentGroup.AddChild(NnMgmtAuthorizationPermissions.PrmHierarchyComponentGroup.Delete, L("Permission:HierarchyComponentGroup:Delete"));

        var pgHierarchicalStructure = myGroup.AddPermission(NnMgmtAuthorizationPermissions.PrmHierarchicalStructure.Default, L("Permission:HierarchicalStructure:Default"));
        pgHierarchicalStructure.AddChild(NnMgmtAuthorizationPermissions.PrmHierarchicalStructure.Create, L("Permission:HierarchicalStructure:Create"));
        pgHierarchicalStructure.AddChild(NnMgmtAuthorizationPermissions.PrmHierarchicalStructure.Delete, L("Permission:HierarchicalStructure:Delete"));
        pgHierarchicalStructure.AddChild(NnMgmtAuthorizationPermissions.PrmHierarchicalStructure.HierarchyComponent, L("Permission:HierarchicalStructure:HierarchyComponent"));

        var pgHierarchicalAccess = myGroup.AddPermission(NnMgmtAuthorizationPermissions.PrmHierarchicalAccess.Default, L("Permission:HierarchicalAccess:Default"));
        pgHierarchicalAccess.AddChild(NnMgmtAuthorizationPermissions.PrmHierarchicalAccess.Create, L("Permission:HierarchicalAccess:Create"));
        pgHierarchicalAccess.AddChild(NnMgmtAuthorizationPermissions.PrmHierarchicalAccess.Update, L("Permission:HierarchicalAccess:Edit"));
        pgHierarchicalAccess.AddChild(NnMgmtAuthorizationPermissions.PrmHierarchicalAccess.Delete, L("Permission:HierarchicalAccess:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<NnMgmtAuthorizationResource>(name);
    }
}
