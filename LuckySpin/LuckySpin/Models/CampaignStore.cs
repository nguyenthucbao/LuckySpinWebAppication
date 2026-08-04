using System;
using System.Collections.Generic;

namespace LuckySpin.Models;

public partial class CampaignStore
{
    public string Id { get; set; } = null!;

    public string? StoreId { get; set; }

    public string? CampaignId { get; set; }
}
