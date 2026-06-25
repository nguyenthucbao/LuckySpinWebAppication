using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuckySpin.Models;
using LuckySpin.Dto;

namespace LuckySpin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpinController : ControllerBase
    {

        private readonly LuckySpinContext _context;

        public SpinController(LuckySpinContext context)
        {
            _context = context;
        }


        [HttpPost("spinaction")]
        public async Task<IActionResult> Spin([FromBody] SpinRequest request)
        {
            //Validate input 
            if (string.IsNullOrWhiteSpace(request.RewardCode) || string.IsNullOrWhiteSpace(request.CampaignId))
                return BadRequest(new { message = "RewardCode và CampaignId không được để trống." });

            var rewardCode = await _context.RewardCodes.FirstOrDefaultAsync(r => r.Code == request.RewardCode);

            if (rewardCode == null) 
                return NotFound(new { message = "Mã quay thưởng không tồn tại." });
            if (rewardCode.RemainingSpins <= 0) 
                return BadRequest(new { message = "Mã quay thưởng đã hết lượt quay." });

            //rewardCode.RemainingSpins -= 1;//////////////////////////////////////////////////////////////

            var bill = await _context.Bills
                .FirstOrDefaultAsync(b => b.Id == rewardCode.BillId);

            if (bill == null)
                return NotFound(new { message = "Không tìm thấy hóa đơn liên kết với mã này." });

            // Lấy danh sách Store_Campaign_Prizes theo store + campaign
            var storePrizes = await _context.StoreCampaignPrizes
                .Where(scp =>
                    scp.StoreId == bill.StoreId &&
                    scp.IsActive == true)
                .Join(
                    _context.Prizes.Where(p =>
                        p.CampaignId == request.CampaignId &&
                        p.IsActive == true),
                    scp => scp.PrizeId,
                    prize => prize.Id,
                    (scp, prize) => new
                    {
                        Prize = prize,
                        Weight = scp.ProbabilityWeight
                    })
                .ToListAsync();

            if (storePrizes == null || storePrizes.Count == 0)
                return NotFound(new
                {
                    message = "Không có phần thưởng nào được cấu hình cho cửa hàng này trong campaign."
                });

            // Weighted random
            var rng = new Random();
            int roll = rng.Next(1, 101);

            Prize? wonPrize = null;
            int cumulative = 0;

            foreach (var sp in storePrizes)
            {
                cumulative += sp.Weight;
                if (roll <= cumulative)
                {
                    wonPrize = sp.Prize;
                    break;
                }
            }
            wonPrize ??= storePrizes.Last().Prize;

            //await _context.SaveChangesAsync();//////////////////////////////////////////////////////////////////////////

            return Ok(new SpinResponse
            {
                RewardCode = rewardCode.Code,
                RemainingSpins = rewardCode.RemainingSpins,
                WonPrize = new PrizeResult
                {
                    Id = wonPrize.Id,
                    Name = wonPrize.Name,
                    PrizeType = wonPrize.PrizeType,
                    CampaignId = wonPrize.CampaignId,
                }
            });
        }



        [HttpGet("{rewardcode}")]
        public async Task<IActionResult> GetCampaign(string rewardcode)
        {
            //Validate input 
            if (string.IsNullOrWhiteSpace(rewardcode))
                return BadRequest(new { message = "RewardCode và CampaignId không được để trống." });


            var rwCode = await _context.RewardCodes.FirstOrDefaultAsync(b => b.Code == rewardcode);
            if (rwCode == null)
                return NotFound(new { message = "Mã không hợp lệ.", rewardcode }); // thêm rewardcode vào response

            var bill = await _context.Bills.FirstOrDefaultAsync(b => b.Id == rwCode.BillId);
            if (bill == null)
                return NotFound(new { message = "Không tìm thấy bill hợp lệ.", billId = rwCode.BillId }); // thêm billId

            var storeID = bill.StoreId;

            var storePrizeList = await _context.StoreCampaignPrizes
             .Where(scp => scp.StoreId == storeID)
             .Include(scp => scp.Prize)
             .ThenInclude(p => p.Campaign)
             .ToListAsync();

            var campaigns = storePrizeList
                .GroupBy(scp => scp.Prize.Campaign)
                .Select(g => new DbCampaignDto
                {
                    Id = g.Key.Id,
                    Name = g.Key.CampaignName,
                    StartAt = (DateTime)g.Key.StartDate,
                    EndAt = (DateTime)g.Key.EndDate,
                    Prizes = g.Select(scp => new DbPrizeDto
                    {
                        Name = scp.Prize.Name,
                        PrizeType = scp.Prize.PrizeType,
                        Quantity = (int)scp.Prize.Quantity
                    }).ToList()
                })
                .ToList();

            return Ok(campaigns);

        }


    }
}
