using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Services;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public interface IHierarchyAppService :
        ICrudAppService<
            HierarchyDto,
            Guid,
            HierarchyResultRequestDto>
    {
    }
}
