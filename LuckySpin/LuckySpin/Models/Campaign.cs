using System;
using System.Collections.Generic;

namespace LuckySpin.Models;

public partial class Campaign
{
    public string Id { get; set; } = null!;

    public string CampaignName { get; set; } = null!;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public virtual ICollection<Prize> Prizes { get; set; } = new List<Prize>();
}
