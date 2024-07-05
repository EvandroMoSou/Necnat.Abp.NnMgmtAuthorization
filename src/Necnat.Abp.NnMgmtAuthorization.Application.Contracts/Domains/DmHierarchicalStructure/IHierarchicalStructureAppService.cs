﻿using Necnat.Abp.NnMgmtAuthorization.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public interface IHierarchicalStructureAppService :
        ICrudAppService<
            HierarchicalStructureDto,
            Guid,
            HierarchicalStructureResultRequestDto>
    {
        Task<List<HierarchicalStructureNode>> GetListHierarchicalStructureNodeAsync(SearchHierarchicalStructureNodeResultRequestDto input);
        Task<List<HierarchyComponentModel>> GetListHierarchyComponentAsync(Guid? hierarchyId = null);
        Task<List<HierarchyComponentTypeModel>> GetListHierarchyComponentTypeAsync(Guid? hierarchyId = null);
        Task<List<HierarchyComponentModel>> GetListHierarchyComponentContributorAsync(short hierarchyComponentTypeId);
        Task<HierarchyComponentTypeModel> GetHierarchyComponentTypeContributorAsync(short hierarchyComponentTypeId);
    }
}
