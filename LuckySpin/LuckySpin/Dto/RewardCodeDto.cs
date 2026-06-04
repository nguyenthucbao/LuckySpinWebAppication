namespace LuckySpin.Dto
{
    public class RewardCodeDto
    {
        public string Id { get; set; } = null!;
        public string Code { get; set; } = null!;
        public int SpinCount { get; set; }
        public int RemainingSpins { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
