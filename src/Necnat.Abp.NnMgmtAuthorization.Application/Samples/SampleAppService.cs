using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Necnat.Abp.NnMgmtAuthorization.Samples;

public class SampleAppService : NnMgmtAuthorizationAppService, ISampleAppService
{
    public Task<SampleDto> GetAsync()
    {
        return Task.FromResult(
            new SampleDto
            {
                Value = 42
            }
        );
    }

    [Authorize]
    public Task<SampleDto> GetAuthorizedAsync()
    {
        return Task.FromResult(
            new SampleDto
            {
                Value = 42
            }
        );
    }
}
