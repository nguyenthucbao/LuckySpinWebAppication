using System;
using System.Collections.Generic;

namespace LuckySpin.Models;

public partial class Bill
{
    public string Id { get; set; } = null!;

    public string? StoreLocate { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? PaymentMethod { get; set; }

    public string? StoreId { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual RewardCode? RewardCode { get; set; }
}
