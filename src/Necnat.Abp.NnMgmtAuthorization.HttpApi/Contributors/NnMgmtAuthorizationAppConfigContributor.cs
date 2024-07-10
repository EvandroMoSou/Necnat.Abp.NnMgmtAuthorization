using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Necnat.Abp.NnMgmtAuthorization.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;
using Volo.Abp.Data;
using Volo.Abp.Users;

namespace Necnat.Abp.NnMgmtAuthorization.Contributors
{
    public class NnMgmtAuthorizationAppConfigContributor : IApplicationConfigurationContributor
    {
        public async Task ContributeAsync(ApplicationConfigurationContributorContext context)
        {
            var currentUser = context.ServiceProvider.GetService<ICurrentUser>();

            if (currentUser == null || !currentUser.IsAuthenticated)
                return;
            context.ApplicationConfiguration.SetProperty(NnMgmtAuthorizationConsts.UserAuthorizationUserId, currentUser.Id.ToString());

            var httpContextAccessor = context.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
            string path = httpContextAccessor.HttpContext!.Request.Path.Value!;
            string basePath = path.Substring(0, path.IndexOf("/api/")) ;
            var accessToken = await httpContextAccessor.HttpContext.GetTokenAsync("access_token");

            HierarchicalAuthorizationModel? hierarchicalAuthorization = null;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                var httpResponseMessage = await client.GetAsync($"{basePath}/api/nn-mgmt-authorization/mgmt-authorization/hierarchical-authorization-my");
                if (httpResponseMessage.IsSuccessStatusCode)
                    hierarchicalAuthorization = JsonSerializer.Deserialize<HierarchicalAuthorizationModel>(await httpResponseMessage.Content.ReadAsStringAsync());
            }

            context.ApplicationConfiguration.SetProperty(NnMgmtAuthorizationConsts.UserAuthorizationLHAC, JsonSerializer.Serialize(hierarchicalAuthorization!.LHAC));
            context.ApplicationConfiguration.SetProperty(NnMgmtAuthorizationConsts.UserAuthorizationLHS, JsonSerializer.Serialize(hierarchicalAuthorization.LHS));
            context.ApplicationConfiguration.SetProperty(NnMgmtAuthorizationConsts.UserAuthorizationLHC, JsonSerializer.Serialize(hierarchicalAuthorization.LHC));
        }
    }
}
