using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using NuGet.Protocol;

namespace LuckySpin.Dto
{
    public class GetCampaignInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
    }

    public class DbCampaignDto
    {
        public int RemainingSpin { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public List<DbPrizeDto> Prizes { get; set; }
    }
    public class DbPrizeDto
    {
        public string Name { get; set; }
        public string PrizeType { get; set; }
        public int Quantity { get; set; }

    }
}
