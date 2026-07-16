namespace LuckySpinAdmin.Dto
{
    public class StoreDto
    {
        public string Id { get; set; } = "";
        public string StoreLocate { get; set; } = "";
        public int CampaignCount { get; set; }
        public int SpinCount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class CreateStoreDto
    {
        public string Id { get; set; } = "";
        public string StoreLocate { get; set; } = "";
    }

    public class UpdateStoreLocationDto
    {
        public string StoreLocate { get; set; } = "";
    }

    public class CampaignDto
    {
        public string Id { get; set; } = "";
        public string CampaignName { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class AssignCampaignsDto
    {
        public List<string> CampaignIds { get; set; } = new();
    }

    public class PrizeDto
    {
        public string Id { get; set; } = "";
        public string CampaignId { get; set; } = "";
        public string Name { get; set; } = "";
        public string PrizeType { get; set; } = "";
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
        public double ProbabilityWeight { get; set; }
        public double EditedWeight { get; set; } // Hỗ trợ binding trên FE
    }

    public class UpdatePrizeWeightDto
    {
        public double Weight { get; set; }
    }
}