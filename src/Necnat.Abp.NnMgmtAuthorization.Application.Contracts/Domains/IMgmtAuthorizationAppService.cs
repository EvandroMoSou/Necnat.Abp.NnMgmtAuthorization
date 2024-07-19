using Necnat.Abp.NnMgmtAuthorization.Models;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public interface IMgmtAuthorizationAppService : IApplicationService, IRemoteService
    {
        Task<HierarchicalAuthorizationModel> GetHierarchicalAuthorizationMyAsync();
    }
}
