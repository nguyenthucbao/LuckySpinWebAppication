namespace LuckySpin.Dto;


public class AddCampaignRequets
{
    public string Name { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
}
public class GetCampaignInfo
{
    public string Id { get; set; }
    public string CampaignName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class GetCampaignList
{
    public int RemainingSpin { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
}

public class CampaignWithPrize
{
    public string Id { get; set; }
    public string? Name { get; set; }
    public int? TotalRoll { get; set; }
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public List<GetPrize> Prizes { get; set; }
}

