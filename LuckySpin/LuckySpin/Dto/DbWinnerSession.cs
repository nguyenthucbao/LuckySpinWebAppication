namespace LuckySpin.Dto;

public class AssignPrizeWinnerRequest
{
    public string WinnerId { get; set; } = string.Empty;
    public string PrizeId { get; set; } = string.Empty;
}

public class PostCustomerRequest
{
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Address { get; set; }
}

public class GetCustomerResponse
{
    public string Id { get; set; } = null!;
    public string? RewardCodeId { get; set; }
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; }
}