using Necnat.Abp.NnMgmtAuthorization.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class EfCoreAuthEndpointRepository : EfCoreRepository<NnMgmtAuthorizationDbContext, AuthEndpoint, Guid>, IAuthEndpointRepository
    {
        public EfCoreAuthEndpointRepository(IDbContextProvider<NnMgmtAuthorizationDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
    }
}
