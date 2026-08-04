using System;
using System.Collections.Generic;

namespace LuckySpin.Models;

public partial class StoreCampaignPrize
{
    public string Id { get; set; } = null!;

    public string? StoreId { get; set; }

    public string? PrizeId { get; set; }

    public int? ProbabilityWeight { get; set; }

    public bool? IsActive { get; set; }

    public virtual Prize? Prize { get; set; }

    public virtual Store? Store { get; set; }
}
