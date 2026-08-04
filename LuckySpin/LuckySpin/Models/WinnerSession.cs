using System;
using System.Collections.Generic;

namespace LuckySpin.Models;

public partial class WinnerSession
{
    public string Id { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public DateTime? CreatedAt { get; set; }
}
