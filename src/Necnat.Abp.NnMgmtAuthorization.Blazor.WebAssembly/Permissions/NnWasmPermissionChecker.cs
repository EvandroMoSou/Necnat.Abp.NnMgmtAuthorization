using Necnat.Abp.NnMgmtAuthorization.Blazor.WebAssembly.Services;
using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.Client;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;

namespace Necnat.Abp.NnMgmtAuthorization.Blazor.WebAssembly.Permissions
{
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IPermissionChecker))]
    public class NnWasmPermissionChecker : IPermissionChecker, ITransientDependency
    {
        protected ICachedApplicationConfigurationClient ConfigurationClient { get; }
        protected IHierarchicalAuthorizationService HierarchicalAuthorizationService { get; }

        public const string _separator = "&";

        public NnWasmPermissionChecker(
            ICachedApplicationConfigurationClient configurationClient,
            IHierarchicalAuthorizationService hierarchicalAuthorizationService)
        {
            ConfigurationClient = configurationClient;
            HierarchicalAuthorizationService = hierarchicalAuthorizationService;
        }

        public async Task<bool> IsGrantedAsync(string name)
        {
            var permissionName = name;
            Guid? hierarchyComponentId = null;
            if (name.Contains(_separator))
            {
                var splitName = name.Split(_separator);
                permissionName = splitName[0];
                hierarchyComponentId = new Guid(splitName[1]);
            }

            if (!await HierarchicalAuthorizationService.CheckAsync())
                return false;

            return await HierarchicalAuthorizationService.IsGrantedAsync(permissionName, hierarchyComponentId);
        }

        public async Task<bool> IsGrantedAsync(ClaimsPrincipal? claimsPrincipal, string name)
        {
            /* This provider always works for the current principal. */
            return await IsGrantedAsync(name);
        }

        public async Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names)
        {
            var result = new MultiplePermissionGrantResult();

            if (!await HierarchicalAuthorizationService.CheckAsync())
            {
                foreach (var name in names)
                    result.Result.Add(name, PermissionGrantResult.Undefined);

                return result;
            }

            foreach (var name in names)
            {
                var permissionName = name;
                Guid? hierarchyComponentId = null;
                if (name.Contains(_separator))
                {
                    var splitName = name.Split(_separator);
                    permissionName = splitName[0];
                    hierarchyComponentId = new Guid(splitName[1]);
                }

                result.Result.Add(name, await HierarchicalAuthorizationService.IsGrantedAsync(permissionName, hierarchyComponentId) ?
                    PermissionGrantResult.Granted :
                    PermissionGrantResult.Undefined);
            }

            return result;
        }

        public async Task<MultiplePermissionGrantResult> IsGrantedAsync(ClaimsPrincipal? claimsPrincipal, string[] names)
        {
            /* This provider always works for the current principal. */
            return await IsGrantedAsync(names);
        }
    }
}
