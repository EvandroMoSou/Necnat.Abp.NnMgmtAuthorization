using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Services;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public interface IHierarchyComponentGroupAppService :
        ICrudAppService<
            HierarchyComponentGroupDto,
            Guid,
            HierarchyComponentGroupResultRequestDto>
    {
    }
}
