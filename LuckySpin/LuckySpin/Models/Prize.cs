using System;
using System.Collections.Generic;

namespace LuckySpin.Models;

public partial class Prize
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public string? CampaignId { get; set; }

    public string? PrizeType { get; set; }

    public int? ProbabilityWeight { get; set; }

    public int? Quantity { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? WinnerId { get; set; }

    public string? SignatureKey { get; set; }

    public string? StoreId { get; set; }

    public string? KeycodeId { get; set; }

    public virtual Campaign? Campaign { get; set; }

    public virtual ICollection<StoreCampaignPrize> StoreCampaignPrizes { get; set; } = new List<StoreCampaignPrize>();
}
