using System.Threading.Tasks;

namespace LuckySpin.Services
{
    public interface IStoreService
    {
        Task<int> RemoveStoreCampaignAsync(string storeId, string campaignId);
    }
}
