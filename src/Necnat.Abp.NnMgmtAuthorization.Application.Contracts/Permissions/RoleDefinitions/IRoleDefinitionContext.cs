using System;
using Volo.Abp.Localization;

namespace Necnat.Abp.NnMgmtAuthorization.Permissions.RoleDefinitions
{
    public interface IRoleDefinitionContext
    {
        //TODO: Add Get methods to find and modify a Role or group.

        IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets a pre-defined Role group.
        /// Throws <see cref="AbpException"/> if can not find the given group.
        /// </summary>
        /// <param name="name">Name of the group</param>
        /// <returns></returns>
        RoleDefinition Get(string name);

        /// <summary>
        /// Tries to get a pre-defined Role group.
        /// Returns null if can not find the given group.
        /// </summary>
        /// <param name="name">Name of the group</param>
        /// <returns></returns>
        RoleDefinition? GetOrNull(string name);

        /// <summary>
        /// Tries to add a new Role group.
        /// Throws <see cref="AbpException"/> if there is a group with the name.
        /// <param name="name">Name of the group</param>
        /// <param name="displayName">Localized display name of the group</param>
        /// <param name="multiTenancySide">Select a multi-tenancy side</param>
        /// </summary>
        RoleDefinition Add(
            string name,
            ILocalizableString? displayName = null);

        /// <summary>
        /// Tries to remove a Role group.
        /// Throws <see cref="AbpException"/> if there is not any group with the name.
        /// <param name="name">Name of the group</param>
        /// </summary>
        void Remove(string name);
    }
}
