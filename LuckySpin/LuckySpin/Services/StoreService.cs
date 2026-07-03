using LuckySpin.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LuckySpin.Services
{
    public class StoreService : IStoreService
    {
        private readonly LuckySpinContext _context;

        public StoreService(LuckySpinContext context)
        {
            _context = context;
        }

        public async Task<int> RemoveStoreCampaignAsync(string storeId, string campaignId)
        {
            // Get all StoreCampaignPrizes existing
            var storeCampaignPrizes = await _context.StoreCampaignPrizes
                .Include(scp => scp.Prize)
                .Where(scp => scp.StoreId == storeId && scp.Prize.CampaignId == campaignId)
                .ToListAsync();

            if (storeCampaignPrizes.Count == 0)
                return 0;

            // Remove all StoreCampaignPrizes
            _context.StoreCampaignPrizes.RemoveRange(storeCampaignPrizes);
            await _context.SaveChangesAsync();

            return storeCampaignPrizes.Count;
        }
    }
}
