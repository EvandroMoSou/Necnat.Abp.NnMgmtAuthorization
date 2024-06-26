using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Necnat.Abp.NnMgmtAuthorization.Samples;

[Area(NnMgmtAuthorizationRemoteServiceConsts.ModuleName)]
[RemoteService(Name = NnMgmtAuthorizationRemoteServiceConsts.RemoteServiceName)]
[Route("api/NnMgmtAuthorization/sample")]
public class SampleController : NnMgmtAuthorizationController, ISampleAppService
{
    private readonly ISampleAppService _sampleAppService;

    public SampleController(ISampleAppService sampleAppService)
    {
        _sampleAppService = sampleAppService;
    }

    [HttpGet]
    public async Task<SampleDto> GetAsync()
    {
        return await _sampleAppService.GetAsync();
    }

    [HttpGet]
    [Route("authorized")]
    [Authorize]
    public async Task<SampleDto> GetAuthorizedAsync()
    {
        return await _sampleAppService.GetAsync();
    }
}
