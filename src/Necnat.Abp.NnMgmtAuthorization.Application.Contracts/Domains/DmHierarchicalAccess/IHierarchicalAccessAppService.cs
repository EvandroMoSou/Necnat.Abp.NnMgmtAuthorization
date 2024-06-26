using System;
using Volo.Abp.Application.Services;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public interface IHierarchicalAccessAppService :
        ICrudAppService<
            HierarchicalAccessDto,
            Guid,
            HierarchicalAccessResultRequestDto>
    {

    }
}
