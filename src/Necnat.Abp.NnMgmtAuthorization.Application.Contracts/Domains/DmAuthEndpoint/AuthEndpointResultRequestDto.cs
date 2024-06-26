using Necnat.Abp.NnLibCommon.Dtos;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class AuthEndpointResultRequestDto : OptionalPagedAndSortedResultRequestDto
    {
        public string? DisplayNameContains { get; set; }
        public string? EndpointContains { get; set; }
        public bool? IsActive { get; set; }
    }
}
