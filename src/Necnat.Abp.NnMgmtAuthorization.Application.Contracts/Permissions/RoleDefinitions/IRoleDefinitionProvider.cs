namespace Necnat.Abp.NnMgmtAuthorization.Permissions.RoleDefinitions
{
    public interface IRoleDefinitionProvider
    {
        void PreDefine(IRoleDefinitionContext context);

        void Define(IRoleDefinitionContext context);

        void PostDefine(IRoleDefinitionContext context);
    }
}
