using System;
using System.Collections.Generic;

namespace LuckySpin.Models;

public partial class RewardCode
{
    public string Id { get; set; } = null!;

    public string BillId { get; set; } = null!;

    public string Code { get; set; } = null!;

    public int SpinCount { get; set; }

    public int RemainingSpins { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Bill Bill { get; set; } = null!;

    public virtual ICollection<WinnerSession> WinnerSessions { get; set; } = new List<WinnerSession>();
}
