namespace LuckySpinFE.Dto
{
    public class WonPrizeItem
    {
        public string PrizeId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime? WonAt { get; set; }  
        public string? SignatureKey { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsConfirming { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }


}
