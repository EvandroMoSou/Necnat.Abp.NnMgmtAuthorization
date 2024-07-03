using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Necnat.Abp.NnMgmtAuthorization.Domains.DmHierarchicalStructure
{
    public interface IHierarchyComponentService
    {
        Task<List<HierarchyComponentModel>> GetListHierarchyComponentAsync(Guid? hierarchyId = null);
        Task<List<HierarchyComponentTypeModel>> GetListHierarchyComponentTypeAsync(Guid? hierarchyId = null);
    }
}
