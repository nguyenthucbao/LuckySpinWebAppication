using LuckySpin.DTO;
using LuckySpin.Models;

namespace LuckySpin.Dto
{
    public class GetStoresInfoDto
    {
        public string Id { get; set; } = null!;
        public string StoreLocate { get; set; } = null!;
        public decimal StoreAmount { get; set; }
        public int StoreSpinCount { get; set; }
        public int StoreUsedSpinCount { get; set; }
        public List<BillWithProductsDto> BillWithProducts { get; set; }
        public List<DbCampaignDto> Campaigns { get; set; }
    }
}
