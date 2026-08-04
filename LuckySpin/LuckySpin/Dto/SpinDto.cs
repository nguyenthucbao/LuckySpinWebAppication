using LuckySpin.Models;

namespace LuckySpin.Dto
{
    public class SpinRequest
    {
        /// <summary>Mã quay thưởng của khách hàng.</summary>
        public string RewardCode { get; set; } = string.Empty;

        /// <summary>ID campaign đang chạy.</summary>
        public string CampaignId { get; set; } = string.Empty;
    }


    public class SpinResponse
    {
        public string RewardCode { get; set; } = string.Empty;

        public int RemainingSpins { get; set; }
        public bool IsWin { get; set; }

        public PrizeResult WonPrize { get; set; } = null!;
    }


    public class PrizeResult
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? PrizeType { get; set; }
        public string? CampaignId { get; set; }
        public string? SignatureKey { get; set; }
    }
}
