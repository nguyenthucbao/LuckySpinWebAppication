using System;
using System.Collections.Generic;

namespace LuckySpin.Models;

public partial class PrizeKey
{
    public string Id { get; set; } = null!;

    public string? Code { get; set; }

    public string? SignatureKey { get; set; }

    public bool? IsActive { get; set; }
}
