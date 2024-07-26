using System.Collections.Generic;
using Volo.Abp.Collections;

namespace Necnat.Abp.NnMgmtAuthorization.Permissions.RoleDefinitions
{
    public class AbpRoleOptions
    {
        public ITypeList<IRoleDefinitionProvider> DefinitionProviders { get; }

        public HashSet<string> DeletedRoles { get; }

        public AbpRoleOptions()
        {
            DefinitionProviders = new TypeList<IRoleDefinitionProvider>();

            DeletedRoles = new HashSet<string>();
        }
    }
}
