﻿using System;

namespace Necnat.Abp.NnMgmtAuthorization.Domains
{
    public class HierarchyComponentModel
    {
        public Guid? Id { get; set; }
        public int? HierarchyComponentType { get; set; }
        public string? Name { get; set; }
    }
}
