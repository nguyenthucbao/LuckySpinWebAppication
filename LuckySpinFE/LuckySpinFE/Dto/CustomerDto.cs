namespace LuckySpinFE.Dto
{
    public class PostCustomerRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }

    public class GetAccountByPhoneRespone 
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }

    public class GetCustomerPrize
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string PrizeType { get; set; } = string.Empty;
        public int? Quantity { get; set; }
        public bool IsActive { get; set; }
        public string? SignatureKey { get; set; }
        public string? KeyId { get; set; }
    }

}
