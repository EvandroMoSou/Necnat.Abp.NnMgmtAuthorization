using System;
using System.Collections.Generic;

namespace Necnat.Abp.NnMgmtAuthorization.Models
{
    public class HierarchicalAuthorizationModel
    {
        public Guid UserId { get; set; }
        public List<H> LH { get; set; } = new List<H>();
        public List<HAC> LHAC { get; set; } = new List<HAC>();
        public List<HS> LHS { get; set; } = new List<HS>();
        public List<HC> LHC { get; set; } = new List<HC>();
    }

    public class H
    {
        public Guid Id { get; set; }
        public string Nm { get; set; } = string.Empty;
        public bool At { get; set; }
    }

    public class HAC
    {
        public List<Guid> LHSId { get; set; } = new List<Guid>();
        public Guid RId { get; set; }
        public List<string> LPN { get; set; } = new List<string>();
    }

    public class HS
    {
        public Guid Id { get; set; }
        public List<Guid> LHCId { get; set; } = new List<Guid>();
    }

    public class HC
    {
        public Guid Id { get; set; }
        public int? Tp { get; set; }
        public string? Nm { get; set; }
    }
}
