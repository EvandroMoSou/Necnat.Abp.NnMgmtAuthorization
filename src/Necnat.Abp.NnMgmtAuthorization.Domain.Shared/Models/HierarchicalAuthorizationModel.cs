﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Necnat.Abp.NnMgmtAuthorization.Models
{
    [Serializable]
    public class HierarchicalAuthorizationModel
    {
        [JsonPropertyName("userId")]
        public Guid UserId { get; set; }
        [JsonPropertyName("lha")]
        public List<HA> LHA { get; set; } = new List<HA>();
        [JsonPropertyName("lhs")]
        public List<HS> LHS { get; set; } = new List<HS>();
        [JsonPropertyName("lhc")]
        public List<HC> LHC { get; set; } = new List<HC>();
    }

    public class HA
    {
        [JsonPropertyName("lhsId")]
        public List<Guid> LHSId { get; set; } = new List<Guid>();
        [JsonPropertyName("rId")]
        public Guid RId { get; set; }
        [JsonPropertyName("lpn")]
        public List<string> LPN { get; set; } = new List<string>();
    }

    public class HS
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("lhcId")]
        public List<Guid> LHCId { get; set; } = new List<Guid>();
    }

    public class HC
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("tp")]
        public int? Tp { get; set; }
        [JsonPropertyName("nm")]
        public string? Nm { get; set; }
    }
}
