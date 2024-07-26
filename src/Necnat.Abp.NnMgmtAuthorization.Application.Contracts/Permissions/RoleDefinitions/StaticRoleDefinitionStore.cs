using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Necnat.Abp.NnMgmtAuthorization.Permissions.RoleDefinitions
{
    public class StaticRoleDefinitionStore : IStaticRoleDefinitionStore, ISingletonDependency
    {
        protected IDictionary<string, RoleDefinition> RoleDefinitions => _lazyRoleDefinitions.Value;
        private readonly Lazy<Dictionary<string, RoleDefinition>> _lazyRoleDefinitions;

        protected AbpRoleOptions Options { get; }

        private readonly IServiceProvider _serviceProvider;

        public StaticRoleDefinitionStore(
            IServiceProvider serviceProvider,
            IOptions<AbpRoleOptions> options)
        {
            _serviceProvider = serviceProvider;
            Options = options.Value;

            _lazyRoleDefinitions = new Lazy<Dictionary<string, RoleDefinition>>(
                CreateRoleDefinitions,
                isThreadSafe: true
            );
        }

        protected virtual Dictionary<string, RoleDefinition> CreateRoleDefinitions()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = new RoleDefinitionContext(scope.ServiceProvider);

                var providers = Options
                    .DefinitionProviders
                    .Select(p => (scope.ServiceProvider.GetRequiredService(p) as IRoleDefinitionProvider)!)
                    .ToList();

                foreach (var provider in providers)
                {
                    provider.PreDefine(context);
                }

                foreach (var provider in providers)
                {
                    provider.Define(context);
                }

                foreach (var provider in providers)
                {
                    provider.PostDefine(context);
                }

                return context.Roles;
            }
        }

        public Task<RoleDefinition?> GetOrNullAsync(string name)
        {
            return Task.FromResult(RoleDefinitions.GetOrDefault(name));
        }

        public virtual Task<IReadOnlyList<RoleDefinition>> GetRolesAsync()
        {
            return Task.FromResult<IReadOnlyList<RoleDefinition>>(
                RoleDefinitions.Values.ToImmutableList()
            );
        }
    }
}
