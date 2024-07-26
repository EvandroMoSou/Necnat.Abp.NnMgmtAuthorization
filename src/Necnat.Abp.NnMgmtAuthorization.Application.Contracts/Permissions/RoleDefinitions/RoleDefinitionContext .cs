using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Localization;

namespace Necnat.Abp.NnMgmtAuthorization.Permissions.RoleDefinitions
{
    public class RoleDefinitionContext : IRoleDefinitionContext
    {
        public IServiceProvider ServiceProvider { get; }

        public Dictionary<string, RoleDefinition> Roles { get; }

        public RoleDefinitionContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Roles = new Dictionary<string, RoleDefinition>();
        }

        public virtual RoleDefinition Add(
            string name,
            ILocalizableString? displayName = null)
        {
            Check.NotNull(name, nameof(name));

            if (Roles.ContainsKey(name))
            {
                throw new AbpException($"There is already an existing Role with name: {name}");
            }

            return Roles[name] = new RoleDefinition(name, displayName);
        }

        public virtual RoleDefinition Get(string name)
        {
            var group = GetOrNull(name);

            if (group == null)
            {
                throw new AbpException($"Could not find a Role definition with the given name: {name}");
            }

            return group;
        }

        public virtual RoleDefinition? GetOrNull(string name)
        {
            Check.NotNull(name, nameof(name));

            if (!Roles.ContainsKey(name))
            {
                return null;
            }

            return Roles[name];
        }

        public virtual void Remove(string name)
        {
            Check.NotNull(name, nameof(name));

            if (!Roles.ContainsKey(name))
            {
                throw new AbpException($"Not found Role with name: {name}");
            }

            Roles.Remove(name);
        }
    }
}
