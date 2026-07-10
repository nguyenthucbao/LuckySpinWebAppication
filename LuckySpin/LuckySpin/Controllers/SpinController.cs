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
            //Validate input 



            var bill = await _context.Bills.FirstOrDefaultAsync(b => b.Id == rewardCode.BillId);

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


            var avaiablePrizes = await _context.StoreCampaignPrizes
                .Include(scp => scp.Prize)
                .Where(scp => scp.StoreId == bill.StoreId
                           && scp.Prize != null
                           && scp.Prize.CampaignId == request.CampaignId
                           && scp.IsActive == true
                           && scp.Prize.IsActive == true)
                .ToListAsync();



            // Gom nhóm theo tên giải thưởng
            var groupedPrizes = avaiablePrizes
                .GroupBy(scp => scp.Prize!.Name)
                .Select(g => new GroupedPrize
                {
                    Name = g.Key,
                    PrizeType = g.First().Prize!.PrizeType,
                    Quantity = g.First().Prize!.Quantity ?? 0, // Số lượng vật phẩm trong 1 gói giải thưởng
                    PrizeQuantity = g.Count(),                 // Số lượng gói quà loại này đang có trong Store
                    ProbabilityWeight = g.First().ProbabilityWeight,
                    IsActive = g.First().Prize!.IsActive,
                    FirstPrize = g.First().Prize
                })
                .ToList();

            if (!groupedPrizes.Any())
                return NotFound(new { message = "ko có phần thưởng nào khả dụng tại cửa hàng này." });



            using var transaction = await _context.Database.BeginTransactionAsync();

            var spinClaimed = await _context.RewardCodes
                .Where(r => r.Id == rewardCode.Id && r.RemainingSpins > 0)
                .ExecuteUpdateAsync(s => s.SetProperty(r => r.RemainingSpins, r => r.RemainingSpins - 1)); /// --1 trên db

            if (spinClaimed == 0)
            {
                await transaction.RollbackAsync();
                return BadRequest(new { message = "Mã quay thưởng đã hết lượt quay." });
            }

            var rng = new Random();
            const int maxRetries = 5;
            GroupedPrize? wonGroup = null;

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                // roll lại phần thưởng acctive
                var currentPrizes = await _context.StoreCampaignPrizes
                    .Include(scp => scp.Prize)
                    .Where(scp => scp.StoreId == bill.StoreId
                               && scp.Prize != null
                               && scp.Prize.CampaignId == request.CampaignId
                               && scp.IsActive == true
                               && scp.Prize.IsActive == true)
                    .AsNoTracking()
                    .ToListAsync();

                var currentGrouped = currentPrizes
                    .GroupBy(scp => scp.Prize!.Name)
                    .Select(g => new GroupedPrize
                    {
                        Name = g.Key,
                        PrizeType = g.First().Prize!.PrizeType,
                        Quantity = g.First().Prize!.Quantity ?? 0,
                        PrizeQuantity = g.Count(),
                        ProbabilityWeight = g.First().ProbabilityWeight,
                        IsActive = g.First().Prize!.IsActive,
                        FirstPrize = g.First().Prize
                    })
                    .ToList();

                if (!currentGrouped.Any())
                    break; // ko cồn phần thưởng khả dụng -> return NotFound


                int totalWeight = currentGrouped.Sum(p => p.ProbabilityWeight);
                int roll = rng.Next(1, totalWeight + 1);

                GroupedPrize? candidate = null;
                int cumulative = 0;
                foreach (var gp in currentGrouped)
                {
                    cumulative += gp.ProbabilityWeight;
                    if (roll <= cumulative)
                    {
                        candidate = gp;
                        break;
                    }
                }
                candidate ??= currentGrouped.Last();

                // Chốt phần thưởng có điều kiện: chỉ set IsActive=false nếu vẫn đang active
                var claimed = await _context.Prizes
                    .Where(p => p.Id == candidate.FirstPrize.Id && p.IsActive == true)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsActive, false));

                if (claimed == 1)
                {
                    wonGroup = candidate;
                    break;
                }
                // claimed == 0 nghĩa là request khác vừa lấy mất phần thưởng này -> thử lại
            }

            if (wonGroup == null)
            {
                // Không chốt được phần thưởng nào sau các lần thử -> hoàn tác luôn lượt quay đã trừ
                await transaction.RollbackAsync();
                return NotFound(new { message = "Không có phần thưởng nào khả dụng, vui lòng thử lại." });
            }

            await transaction.CommitAsync();

            return Ok(new SpinResponse
            {
                RewardCode = rewardCode.Code,
                RemainingSpins = rewardCode.RemainingSpins - 1,
                WonPrize = new PrizeResult
                {
                    Id = wonGroup.FirstPrize.Id,
                    Name = wonGroup.FirstPrize.Name,
                    PrizeType = wonGroup.FirstPrize.PrizeType,
                    CampaignId = wonGroup.FirstPrize.CampaignId,
                }
            });

        }



        //Lấy danh sách campaign theo rewardcode
        [HttpGet("getcampaign/{rewardcode}")]
        public async Task<IActionResult> GetCampaign(string rewardcode)
        {
            //Validate input 
            var rwCode = await _context.RewardCodes.FirstOrDefaultAsync(b => b.Code == rewardcode);

            if (rwCode == null)
                return NotFound(new { message = "Mã không hợp lệ.", rewardcode });

            var bill = await _context.Bills.FirstOrDefaultAsync(b => b.Id == rwCode.BillId);
            if (bill == null)
                return NotFound(new { message = "Không tìm thấy bill hợp lệ.", billId = rwCode.BillId });

            var store = await _context.Stores.FirstOrDefaultAsync(b => b.Id == bill.StoreId);
            if (store == null)
                return NotFound(new { message = "Cửa hàng không tồn tại", billId = rwCode.BillId });

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
                    RemainingSpin = rwCode.RemainingSpins,
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