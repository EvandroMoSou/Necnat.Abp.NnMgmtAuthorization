using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Repositories;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public interface IAuthEndpointRepository : IRepository<AuthEndpoint, Guid>
    {

    }
}
