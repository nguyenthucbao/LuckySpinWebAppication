using LuckySpin.DTO;
using System.ComponentModel.DataAnnotations;

namespace LuckySpin.Dto;

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