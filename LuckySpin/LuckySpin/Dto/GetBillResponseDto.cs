using LuckySpin.DTO;

namespace LuckySpin.Dto;

public class GetBillResponseDto
{
    public string Id { get; set; } = null!;
    public string StoreId { get; set; } = null!;
    public string StoreLocate { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = null!;
    public List<ProductDto> Products { get; set; } = new();
    public RewardCodeDto RewardCode { get; set; } = null!;
}
