using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public interface IHierarchicalAccessAppService :
        ICrudAppService<
            HierarchicalAccessDto,
            Guid,
            HierarchicalAccessResultRequestDto>
    {
        Task<List<HA>> GetListHaMyAsync();
    }
}
