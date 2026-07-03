namespace LuckySpin.Dto
{
    public class ProbabilityChangeDto
    {
        public string StoreId { get; set; } = null!;
        public string CampaignId { get; set; } = null!;
        public string PrizeName { get; set; } = null!;
        public int NewProbabilityWeight { get; set; }
    }
}
