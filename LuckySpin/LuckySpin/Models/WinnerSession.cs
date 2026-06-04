using System;
using System.Collections.Generic;

namespace LuckySpin.Models;

public partial class WinnerSession
{
    public string Id { get; set; } = null!;

    public string RewardCodeId { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual RewardCode RewardCode { get; set; } = null!;

    public virtual ICollection<WinnerItem> WinnerItems { get; set; } = new List<WinnerItem>();
}
