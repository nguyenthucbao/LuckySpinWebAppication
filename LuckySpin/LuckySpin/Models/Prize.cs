using System;
using System.Collections.Generic;

namespace LuckySpin.Models;

public partial class Prize
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string CampaignId { get; set; } = null!;

    public string PrizeType { get; set; } = null!;

    public int ProbabilityWeight { get; set; }

    public int? Quantity { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Campaign Campaign { get; set; } = null!;

    public virtual ICollection<StoreCampaignPrize> StoreCampaignPrizes { get; set; } = new List<StoreCampaignPrize>();

    public virtual ICollection<WinnerItem> WinnerItems { get; set; } = new List<WinnerItem>();
}
