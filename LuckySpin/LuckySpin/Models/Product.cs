using System;
using System.Collections.Generic;

namespace LuckySpin.Models;

public partial class Product
{
    public string Id { get; set; } = null!;

    public string BillId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int Quantity { get; set; }

    public virtual Bill Bill { get; set; } = null!;
}
