namespace LuckySpinFE.Dto
{
    public class SpinRequest
    {
        public string RewardCode { get; set; } = string.Empty;
        public string CampaignId { get; set; } = string.Empty;
    }

    public class SpinResponse
    {
        public string RewardCode { get; set; } = string.Empty;
        public int RemainingSpins { get; set; }
        public PrizeResult WonPrize { get; set; } = null!;
    }

    public class PrizeResult
    {
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? PrizeType { get; set; }
        public string? CampaignId { get; set; }
    }
}