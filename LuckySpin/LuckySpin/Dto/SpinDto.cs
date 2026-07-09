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

    // ── Response ───────────────────────────────────────────────────────────────
    public class SpinResponse
    {
        public string RewardCode { get; set; } = string.Empty;

        /// <summary>Số lượt quay còn lại SAU lần quay này.</summary>
        public int RemainingSpins { get; set; }

        public PrizeResult WonPrize { get; set; } = null!;
    }


    public class PrizeResult
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? PrizeType { get; set; }
        public string? CampaignId { get; set; }
    }
}
