using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public interface IHierarchyComponentService
    {
        Task<List<HierarchyComponentModel>> GetListHierarchyComponentAsync();
        Task<List<HierarchyComponentTypeModel>> GetListHierarchyComponentTypeAsync();
    }
}
