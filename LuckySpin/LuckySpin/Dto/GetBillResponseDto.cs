using LuckySpin.DTO;

namespace LuckySpin.Dto;

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
