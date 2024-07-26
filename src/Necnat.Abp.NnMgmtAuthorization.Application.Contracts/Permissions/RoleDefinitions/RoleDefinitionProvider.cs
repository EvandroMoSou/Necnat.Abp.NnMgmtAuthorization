using Volo.Abp.DependencyInjection;

namespace Necnat.Abp.NnMgmtAuthorization.Permissions.RoleDefinitions
{
    public abstract class RoleDefinitionProvider : IRoleDefinitionProvider, ITransientDependency
    {
        public virtual void PreDefine(IRoleDefinitionContext context)
        {

        }

        public abstract void Define(IRoleDefinitionContext context);

        public virtual void PostDefine(IRoleDefinitionContext context)
        {

        }
    }
}
