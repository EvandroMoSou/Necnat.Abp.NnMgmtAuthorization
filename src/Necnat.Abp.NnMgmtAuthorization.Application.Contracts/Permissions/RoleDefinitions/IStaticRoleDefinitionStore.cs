using System.Collections.Generic;
using System.Threading.Tasks;

namespace Necnat.Abp.NnMgmtAuthorization.Permissions.RoleDefinitions
{
    public interface IStaticRoleDefinitionStore
    {
        Task<RoleDefinition?> GetOrNullAsync(string name);

        Task<IReadOnlyList<RoleDefinition>> GetRolesAsync();
    }
}
