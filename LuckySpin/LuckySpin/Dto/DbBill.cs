using LuckySpin.Dto;
using System.ComponentModel.DataAnnotations;

namespace LuckySpin.Dto;

public class BillWithProductsDto
{
    public string Id { get; set; }
    public string Code { get; set; }
    public string StoreId { get; set; }
    public string StoreLocate { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; }
    public List<DbProductDto> Products { get; set; }
}

public class PostBillRequest
{
    public string Id { get; set; } = null!;
    public string StoreId { get; set; } = null!;
    public string StoreLocate { get; set; } = null!;
    public decimal TotalAmount { get; set; }

    [RegularExpression("^(online|direct|delivery)$",
        ErrorMessage = "PaymentMethod phải là: online, direct, hoặc delivery")]
    public string PaymentMethod { get; set; } = null!;
    public List<DbProductDto> Products { get; set; } = new();
}

public class GetBillResponseDto
{
    public string Id { get; set; } = null!;
    public string StoreId { get; set; } = null!;
    public string StoreLocate { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = null!;
    public List<DbProductDto> Products { get; set; } = new();
    public DbRewardCodeDto RewardCode { get; set; } = null!;
}
