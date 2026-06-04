namespace LuckySpin.Dto
{
    public class GetStoresInfoDto
    {
        public string Id { get; set; } = null!;
        public string StoreLocate { get; set; } = null!;
        public decimal StoreAmount { get; set; }
        public int StoreSpinCount { get; set; }
        public int StoreUsedSpinCount { get; set; }
    }
}
