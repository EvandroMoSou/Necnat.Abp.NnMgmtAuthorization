using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Necnat.Abp.NnLibCommon.Domains;
using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
            var necnatEndpointStore = context.ServiceProvider.GetRequiredService<INecnatEndpointStore>();

            var lhac = new List<HAC>();
            var accessToken = await httpContextAccessor.HttpContext!.GetTokenAsync("access_token");
            foreach (var iEndpoint in await necnatEndpointStore.GetListAuthorizationEndpointAsync())
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                    try
                    {
                        var httpResponseMessage = await client.GetAsync($"{iEndpoint}/api/NnMgmtAuthorization/MgmtAuthorization/authorization-info-one-my");
                        if (httpResponseMessage.IsSuccessStatusCode)
                            lhac.AddRange(JsonSerializer.Deserialize<HierarchicalAuthorizationModel>(await httpResponseMessage.Content.ReadAsStringAsync())!.LHAC);
                    }
                    catch { }
                }
            }
            context.ApplicationConfiguration.SetProperty(NnMgmtAuthorizationConsts.UserAuthorizationLHAC, JsonSerializer.Serialize(lhac));

            var lhs = new List<HS>();
            var lhc = new List<HC>();
            var authServerEndpoint = await necnatEndpointStore.FindAuthServerEndpointAsync();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                var httpResponseMessage = await client.PostAsJsonAsync($"{authServerEndpoint}/api/NnMgmtAuthorization/MgmtAuthorization/get-authorization-info-two", lhac.SelectMany(x => x.LHSId.Where(x => x != null)));
                if (!httpResponseMessage.IsSuccessStatusCode)
                    throw new Exception(await httpResponseMessage.Content.ReadAsStringAsync());

                var model = JsonSerializer.Deserialize<HierarchicalAuthorizationModel>(await httpResponseMessage.Content.ReadAsStringAsync())!;
                context.ApplicationConfiguration.SetProperty(NnMgmtAuthorizationConsts.UserAuthorizationLHS, JsonSerializer.Serialize(model.LHS));
                context.ApplicationConfiguration.SetProperty(NnMgmtAuthorizationConsts.UserAuthorizationLHC, JsonSerializer.Serialize(model.LHC));
            }
        }
    }
}
