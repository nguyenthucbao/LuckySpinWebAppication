namespace LuckySpin.Dto;

public class DbRewardCodeDto
{
    public string Id { get; set; } = null!;
    public string Code { get; set; } = null!;
    public int SpinCount { get; set; }
    public int RemainingSpins { get; set; }
    public DateTime CreatedAt { get; set; }
}
