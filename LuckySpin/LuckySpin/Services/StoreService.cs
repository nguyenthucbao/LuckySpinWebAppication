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
            var campaignStores = await _context.CampaignStores
                    .Where(cs => cs.StoreId == storeId && cs.CampaignId == campaignId)
                    .ToListAsync();


            // Get all StoreCampaignPrizes existing
            var storeCampaignPrizes = await _context.StoreCampaignPrizes
                .Include(scp => scp.Prize)
                .Where(scp => scp.StoreId == storeId && scp.Prize.CampaignId == campaignId)
                .ToListAsync();


            var prizes = await _context.Prizes
                    .Where(p => p.CampaignId == campaignId && p.StoreId == storeId)
                    .ToListAsync();

            // Remove all StoreCampaignPrizes, CampaignStore, Prize
            _context.CampaignStores.RemoveRange(campaignStores);
            _context.StoreCampaignPrizes.RemoveRange(storeCampaignPrizes);
            _context.Prizes.RemoveRange(prizes);
            await _context.SaveChangesAsync();



            return storeCampaignPrizes.Count;
        }
    }
}
