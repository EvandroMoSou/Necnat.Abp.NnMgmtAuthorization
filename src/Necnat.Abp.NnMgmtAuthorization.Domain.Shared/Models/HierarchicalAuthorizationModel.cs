using System;
using System.Collections.Generic;

namespace Necnat.Abp.NnMgmtAuthorization.Models
{
    public class HierarchicalAuthorizationModel
    {
        public Guid UserId { get; set; }
        public List<H> LH { get; set; } = new List<H>();
        public List<HAC> LHAC { get; set; } = new List<HAC>();
        public List<HSC> LHSC { get; set; } = new List<HSC>();
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

    public class HSC
    {
        public Guid Id { get; set; }
        public Guid HId { get; set; }
        public List<HS> LChl { get; set; } = new List<HS>();
    }

    public class HS
    {
        public Guid Id { get; set; }
        public Guid? IdParent { get; set; }
        public int HCT { get; set; }
        public Guid HCId { get; set; }
        public string? HCNm { get; set; }
    }
}
