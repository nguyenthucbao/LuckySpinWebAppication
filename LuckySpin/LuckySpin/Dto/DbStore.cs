using LuckySpin.Models;

namespace LuckySpin.Dto;

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

public class GetPrize
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string PrizeType { get; set; } = null!;
    public int Quantity { get; set; }
    public int ProbabilityWeight { get; set; }
    public bool IsActive { get; set; }
    public string? WinnerId { get; set; }
    public string? SignatureKey { get; set; }

}

public class AddCampaignToStoreResultDto
{
    public string StoreId { get; set; } = null!;
    public string CampaignId { get; set; } = null!;
    public int CreatedCount { get; set; }
    public List<string> CreatedIds { get; set; } = new();
}

public class GroupedPrizeAdmin
{
    public string Name { get; set; } = null!;
    public string PrizeType { get; set; } = null!;
    public int Quantity { get; set; } // Số lượng phần thưởng giống nhau
    public int PrizeQuantity { get; set; } // số phần thưởng dducocojw trao
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

public class ProbabilityChangeDto
{
    public string StoreId { get; set; } = null!;
    public string CampaignId { get; set; } = null!;
    public string PrizeName { get; set; } = null!;
    public int NewProbabilityWeight { get; set; }
}
