using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public interface IMgmtAuthorizationAppService : IApplicationService, IRemoteService
    {
        Task CallConsolidateAdminUserEndpointAsync(Guid adminUserId);
        Task ConsolidateAdminUserAsync(Guid adminUserId);
    }
}
