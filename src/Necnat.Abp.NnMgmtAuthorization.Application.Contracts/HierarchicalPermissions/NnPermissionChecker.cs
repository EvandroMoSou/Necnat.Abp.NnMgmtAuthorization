using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;

namespace Necnat.Abp.NnMgmtAuthorization.HierarchicalPermissions
{
    public class NnPermissionChecker : IPermissionChecker
    {
        protected IHierarchicalAuthorizationService HierarchicalAuthorizationService { get; }

        public NnPermissionChecker(
            IHierarchicalAuthorizationService hierarchicalAuthorizationService)
        {
            HierarchicalAuthorizationService = hierarchicalAuthorizationService;
        }

        public virtual async Task<bool> IsGrantedAsync(string name)
        {
            var nnPermission = GetNnPermission(name);
            return await HierarchicalAuthorizationService.IsGrantedAsync(nnPermission.Name, nnPermission.HierarchyComponentId);
        }

        public virtual async Task<bool> IsGrantedAsync(ClaimsPrincipal? claimsPrincipal, string name)
        {
            return await IsGrantedAsync(name);
        }

        public virtual async Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names)
        {
            var result = new MultiplePermissionGrantResult();

            foreach (var name in names)
            {
                var nnPermission = GetNnPermission(name);
                result.Result.Add(name, await HierarchicalAuthorizationService.IsGrantedAsync(nnPermission.Name, nnPermission.HierarchyComponentId) ?
                    PermissionGrantResult.Granted :
                    PermissionGrantResult.Undefined);
            }

            return result;
        }

        public virtual async Task<MultiplePermissionGrantResult> IsGrantedAsync(ClaimsPrincipal? claimsPrincipal, string[] names)
        {
            return await IsGrantedAsync(names);
        }

        protected virtual NnPermission GetNnPermission(string s)
        {
            const string separator = "&";

            if (!s.Contains(separator))
                return new NnPermission(s);

            var splitName = s.Split(separator);
            return new NnPermission(splitName[0], new Guid(splitName[1]));
        }
    }
}
