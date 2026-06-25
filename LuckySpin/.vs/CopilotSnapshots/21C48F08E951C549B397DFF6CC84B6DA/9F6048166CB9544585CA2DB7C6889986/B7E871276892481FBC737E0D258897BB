using System;
using System.Collections.Generic;

namespace LuckySpin.Models;

public partial class WinnerItem
{
    public string Id { get; set; } = null!;

    public string WinnerSessionId { get; set; } = null!;

    public string PrizeId { get; set; } = null!;

    public DateTime WonAt { get; set; }

    public virtual Prize Prize { get; set; } = null!;

    public virtual WinnerSession WinnerSession { get; set; } = null!;
}
