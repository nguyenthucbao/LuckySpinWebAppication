using LuckySpin.DTO;
using LuckySpin.Models;

namespace LuckySpin.Dto
{
    public class GetStoresInfo
    {
        public string Id { get; set; } = null!;
        public string StoreLocate { get; set; } = null!;
        public decimal StoreAmount { get; set; }
        public int StoreSpinCount { get; set; }
        public int StoreUsedSpinCount { get; set; }
        public List<GetCampaignInfo> Campaigns { get; set; }
    }

    public class GetStores
    {
        public string Id { get; set; } = null!;
        public string StoreLocate { get; set; } = null!;
    }

    public class GetPrizeInStore
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string PrizeType { get; set; } = null!;
        public int Quantity { get; set; }
        public int ProbabilityWeight { get; set; }
        public bool IsActive { get; set; }
    }

    public class GroupedPrize
    {
        public string Name { get; set; } = null!;
        public string PrizeType { get; set; } = null!;
        public int Quantity { get; set; } // Số lượng phần thưởng giống nhau
        public int PrizeQuantity { get; set; } // số phần thưởng dducocojw trao
        public int ProbabilityWeight { get; set; }
        public bool IsActive { get; set; }
        public Prize FirstPrize { get; set; }
    }
    
}
