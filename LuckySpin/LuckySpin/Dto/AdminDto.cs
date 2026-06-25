using System;
using System.Collections.Generic;

namespace LuckySpin.Dto;

public class StoreDto
{
    public string? Id { get; set; }
    public string? StoreLocate { get; set; }
}

public class PrizeDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string PrizeType { get; set; } = null!;
    public int ProbabilityWeight { get; set; }
    public int? Quantity { get; set; }
    public bool IsActive { get; set; }
    public string? WinnerId { get; set; }
}

public class StoreCampaignPrizeDto
{
    public string Id { get; set; } = null!;
    public string? StoreId { get; set; }
    public string? PrizeId { get; set; }
    public int ProbabilityWeight { get; set; }
    public bool? IsActive { get; set; }
    public PrizeDto? Prize { get; set; }
    public StoreDto? Store { get; set; }
}

public class AddCampaignToStoreResultDto
{
    public string StoreId { get; set; } = null!;
    public string CampaignId { get; set; } = null!;
    public int CreatedCount { get; set; }
    public List<string> CreatedIds { get; set; } = new();
}
