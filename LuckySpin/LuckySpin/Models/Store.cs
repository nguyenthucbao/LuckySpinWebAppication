using System;
using System.Collections.Generic;

namespace LuckySpin.Models;

public partial class Store
{
    public string Id { get; set; } = null!;

    public string StoreLocate { get; set; } = null!;

    public virtual ICollection<StoreCampaignPrize> StoreCampaignPrizes { get; set; } = new List<StoreCampaignPrize>();
}
