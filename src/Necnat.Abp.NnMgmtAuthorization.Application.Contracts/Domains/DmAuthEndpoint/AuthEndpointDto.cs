using Necnat.Abp.NnLibCommon.Dtos;
using System;
using Volo.Abp.Application.Dtos;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class AuthEndpointDto : ConcurrencyEntityDto<Guid>
    {
        public string? DisplayName { get; set; }
        public string? Endpoint { get; set; }
        public bool? IsAuthentication { get; set; }
        public bool? IsActive { get; set; }
    }
}
