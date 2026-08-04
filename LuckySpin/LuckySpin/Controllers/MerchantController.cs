using LuckySpin.Dto;
using LuckySpin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class MerchantController : ControllerBase
{
    private readonly LuckySpinContext _context;
    public MerchantController(LuckySpinContext context)
    {
        _context = context;
    }

    [HttpGet("getstorebyid/{id}")]
    public async Task<ActionResult<GetStoresInfo>> GetStoreById(string id)
    {
        var store = await _context.Stores.FindAsync(id);
        if (store == null)
            return NotFound();


        var bills = await _context.Bills
                .Include(b => b.RewardCode)
                .Where(b => b.StoreId == store.Id)
                .ToListAsync();

        var rewardCodes = bills.Where(b => b.RewardCode != null).Select(b => b.RewardCode!).ToList();

        var storePrizes = await _context.StoreCampaignPrizes
            .Where(scp => scp.StoreId == store.Id)
            .Include(scp => scp.Prize)
            .ThenInclude(p => p.Campaign)
            .ToListAsync();

        var campaigns = storePrizes
            .Where(scp => scp.Prize != null && scp.Prize.Campaign != null)
            .GroupBy(scp => scp.Prize.Campaign)
            .Select(g => new GetCampaignInfo
            {
                Id = g.Key.Id,
                CampaignName = g.Key.CampaignName,
                StartDate = g.Key.StartDate ?? DateTime.MinValue,
                EndDate = g.Key.EndDate ?? DateTime.MinValue
            })
            .ToList();

        var result = new GetStoresInfo
        {
            Id = store.Id,
            StoreLocate = store.StoreLocate,
            StoreAmount = bills.Sum(x => x.TotalAmount ?? 0),
            StoreSpinCount = rewardCodes.Sum(x => x.SpinCount ?? 0),
            StoreUsedSpinCount = rewardCodes.Sum(x => (x.SpinCount ?? 0) - (x.RemainingSpins ?? 0)),
            Campaigns = campaigns
        };

        return Ok(result);
    }


    //[HttpPost("PendingApprove/{id}")]
    //public async Task<ActionResult<bool>> PendingApprove(string customerId, string prizeId)
    //{
    //    try
    //    {
    //        var existingCampaign = await _context.Campaigns.FindAsync(campaign.Id);
    //        if (existingCampaign != null)
    //            return BadRequest(new { message = "Chiến dịch với ID này đã tồn tại" });

    //        _context.Campaigns.Add(campaign);
    //        await _context.SaveChangesAsync();

    //        return Ok();
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Lỗi khi thêm store mới");
    //        return StatusCode(500, new { message = "Lỗi hệ thống khi thêm store", error = ex.Message });
    //    }

    //}
}


    
