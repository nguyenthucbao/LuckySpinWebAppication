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
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GetAccountByPhone
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }

}

public class GetCustomerPrize
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? PrizeType { get; set; }
    public int? Quantity { get; set; }
    public bool? IsActive { get; set; }
    public string? SignatureKey { get; set; }
    public string? KeyId { get; set; }

}