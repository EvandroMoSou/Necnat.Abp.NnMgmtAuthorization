using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public interface IHierarchyRepository : IRepository<Hierarchy, Guid>
    {
        Task<Hierarchy?> GetByNameAsync(string name);
    }
}
