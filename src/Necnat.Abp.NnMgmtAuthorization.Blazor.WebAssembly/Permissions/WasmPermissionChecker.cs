using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class RemotePermissionChecker : IPermissionChecker, ITransientDependency
    {
        protected ICachedApplicationConfigurationClient ConfigurationClient { get; }

        public const string _separator = "&";

        public RemotePermissionChecker(ICachedApplicationConfigurationClient configurationClient)
        {
            ConfigurationClient = configurationClient;
        }

        public async Task<bool> IsGrantedAsync(string name)
        {
            var configuration = await ConfigurationClient.GetAsync();

            var permissionName = name;
            Guid? hierarchyComponentId = null;
            if (name.Contains(_separator))
            {
                var splitName = name.Split(_separator);
                permissionName = splitName[0];
                hierarchyComponentId = new Guid(splitName[1]);
            }

            if (!configuration.Setting.Values.ContainsKey("ua:lhac"))
                return false;

            var lhac = JsonSerializer.Deserialize<List<HAC>>(configuration.Setting.Values["ua:lhac"]!);
            var lhs = JsonSerializer.Deserialize<List<HS>>(configuration.Setting.Values["ua:lhs"]!);

            return IsGranted(lhac!, lhs!, permissionName, hierarchyComponentId);
        }

        public async Task<bool> IsGrantedAsync(ClaimsPrincipal? claimsPrincipal, string name)
        {
            /* This provider always works for the current principal. */
            return await IsGrantedAsync(name);
        }

        public async Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names)
        {
            var result = new MultiplePermissionGrantResult();
            var configuration = await ConfigurationClient.GetAsync();

            if (!configuration.Setting.Values.ContainsKey("ua:lhac"))
            {
                foreach (var name in names)
                    result.Result.Add(name, PermissionGrantResult.Undefined);

                return result;
            }

            var lhac = JsonSerializer.Deserialize<List<HAC>>(configuration.Setting.Values["ua:lhac"]!);
            var lhs = JsonSerializer.Deserialize<List<HS>>(configuration.Setting.Values["ua:lhs"]!);

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

                result.Result.Add(name, IsGranted(lhac!, lhs!, permissionName, hierarchyComponentId) ?
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

        protected virtual bool IsGranted(List<HAC> lhac, List<HS> lhs, string permissionName, Guid? hierarchyComponentId = null)
        {
            foreach (var hac in lhac)
            {
                if (!hac.LPN.Contains(permissionName))
                    continue;

                if (hierarchyComponentId == null)
                    return true;

                foreach (var hsId in hac.LHSId)
                    if (lhs.Where(x => x.Id == hsId).First().LHCId.Contains((Guid)hierarchyComponentId))
                        return true;
            }

            return false;
        }
    }
}
