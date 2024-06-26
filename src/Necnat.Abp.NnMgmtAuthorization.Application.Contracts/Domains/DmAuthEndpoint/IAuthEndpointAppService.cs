using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public interface IAuthEndpointAppService :
        ICrudAppService<
            AuthEndpointDto,
            Guid,
            AuthEndpointResultRequestDto>
    {

    }
}