using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using LuckySpin.Dto;
using LuckySpin.Models;
using LuckySpin.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;

namespace LuckySpin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        private readonly LuckySpinContext _context;
        private readonly ILogger<StoresController> _logger;

        private readonly IStoreService _storeService;

        public StoresController(LuckySpinContext context, ILogger<StoresController> logger, IStoreService storeService)
        {
            _context = context;
            _logger = logger;
            _storeService = storeService;
        }



        [HttpGet("admin")]
        public async Task<ActionResult<GetStores>> GetStores()
        {
            var b = await _context.Stores.ToListAsync();

            List<GetStores> store = b.Select(b => new GetStores
            {
                Id = b.Id,
                StoreLocate = b.StoreLocate
            }).ToList();

            return Ok(store);
        }


        [HttpGet("admin/getstorebyid/{id}")]
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

            var campaignStore = (await _context.CampaignStores
                .Where(cs => cs.StoreId == id)
                .Join(
                    _context.Campaigns,
                    cs => cs.CampaignId,
                    c => c.Id,
                    (cs, c) => new { cs, c }
                )
                .ToListAsync())  
                .Select(x => new GetCampaignInfo 
                {
                    Id = x.c.Id,
                    CampaignName = x.c.CampaignName,
                    StartDate = x.c.StartDate ?? DateTime.MinValue,
                    EndDate = x.c.EndDate ?? DateTime.MinValue
                })
                .ToList();



            var result = new GetStoresInfo
            {
                Id = store.Id,
                StoreLocate = store.StoreLocate,
                StoreAmount = bills.Sum(x => x.TotalAmount ?? 0),
                StoreSpinCount = rewardCodes.Sum(x => x.SpinCount ?? 0),
                StoreUsedSpinCount = rewardCodes.Sum(x => (x.SpinCount ?? 0) - (x.RemainingSpins ?? 0)),
                Campaigns = campaignStore
            };

            return Ok(result);
        }

        // Add Store
        [HttpPost("admin/addstore")]
        public async Task<IActionResult> AddStore([FromBody] Store store)
        {
            try
            {
                // Check if store already exists
                var existingStore = await _context.Stores.FindAsync(store.Id);
                if (existingStore != null)
                    return BadRequest(new { message = "Store với ID này đã tồn tại" });

                // Add new store
                _context.Stores.Add(store);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Đã thêm store thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm store mới");
                return StatusCode(500, new { message = "Lỗi hệ thống khi thêm store", error = ex.Message });
            }
        }



        //Delete Store
        [HttpDelete("admin/deletestore/{storeId}")]
        public async Task<IActionResult> DeleteStore(string storeId)
        {
            try
            {
                var store = await _context.Stores.FindAsync(storeId);
                if (store == null)
                    return NotFound(new { message = "không tìm thấy store" });

                // Get all campaigns linked to this store
                var campaignStores = await _context.CampaignStores
                    .Where(cs => cs.StoreId == storeId)
                    .ToListAsync();

                // Remove store from all campaigns
                foreach (var campaign in campaignStores)
                {
                    await _storeService.RemoveStoreCampaignAsync(storeId, campaign.CampaignId);
                }

                // Remove store
                _context.Stores.Remove(store);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Đã xóa store thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa store");
                return StatusCode(500, new { message = "Lỗi hệ thống khi xóa store", error = ex.Message });
            }
        }

        // Make change Store info
        [HttpPost("admin/changestoreinfo/{storeId}/{newLocation}")]
        public async Task<IActionResult> ChangeStoreInfo(string storeId, string newLocation)
        {
            try
            {
                var store = await _context.Stores.FindAsync(storeId);
                if (store == null)
                    return NotFound(new { message = "không tìm thấy store" });

                store.StoreLocate = newLocation;

                _context.Stores.Update(store);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Cập nhật vị trí store thành công" });
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi update");
                return StatusCode(500, new { message = "lỗi hệ thống", error = ex.Message });
            }
        }

        //thêm 1 phần thưởng vào store
        [HttpPost("admin/addprizetostore")]
        public async Task<IActionResult> AddPrize([FromBody] Prize prize)
        {
            try
            {
                var existingPrize = await _context.Prizes.FindAsync(prize.Id);
                if (existingPrize != null)
                    return BadRequest(new { message = "Phần thưởng với ID này đã trùng" });

                // 2. Kiểm tra CampaignId có tồn tại hợp lệ không
                var existingCampaign = await _context.Campaigns.FindAsync(prize.CampaignId);
                if (existingCampaign == null)
                    return BadRequest(new { message = "Campaign chưa được khởi tạo" });

                var existingStore = await _context.Stores.FindAsync(prize.StoreId);
                if (existingStore == null)
                    return BadRequest(new { message = "Cửa hàng (Store) không tồn tại trên hệ thống" });


                var campaignStores = await _context.CampaignStores
                    .Where(cs => cs.StoreId == prize.StoreId && cs.CampaignId == prize.CampaignId)
                    .ToListAsync();

                if (!campaignStores.Any())
                    return BadRequest(new { message = "Cửa hàng chưa đăng kí chương trình này" });


                // 4. Tạo thực thể liên kết StoreCampaignPrize
                var scp = new StoreCampaignPrize
                {
                    // BỎ dòng Id = Guid.NewGuid().ToString() NẾU id trong database là kiểu INT tự tăng
                    Id = Guid.NewGuid().ToString(), 
                    StoreId = prize.StoreId,
                    PrizeId = prize.Id,
                    ProbabilityWeight = prize.ProbabilityWeight,
                    IsActive = true,
                };

                // 5. Thêm vào DbContext
                _context.Prizes.Add(prize);
                _context.StoreCampaignPrizes.Add(scp);

                // 6. Lưu vào database
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                // Log chi tiết cả InnerException (nơi hiển thị rõ lỗi ràng buộc database như trùng/sai khóa ngoại)
                var detailedError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _logger.LogError(ex, "Lỗi khi thêm prize mới: {Message}", detailedError);

                return StatusCode(500, new
                {
                    message = "Lỗi hệ thống khi thêm prize",
                    error = detailedError // Trả về chi tiết để bạn dễ debug lúc này
                });
            }
        }



        // Lấy danh sách giải thưởng của một cửa hàng trong một chiến dịch (gom nhóm theo tên)
        [HttpGet("admin/storecampaignprize/{storeId}/{campaignId}")]
        public async Task<ActionResult<List<GroupedPrize>>> GetStoreCampaignPrizes(string storeId, string campaignId)
        {
            try
            {
                var store = await _context.Stores.FindAsync(storeId);
                if (store == null)
                    return NotFound(new { message = "Store not found" });

                var campaign = await _context.Campaigns.FindAsync(campaignId);
                if (campaign == null)
                    return NotFound(new { message = "Campaign not found" });

                var now = DateTime.Now;
                var isCampaignRunning = campaign.StartDate.HasValue && 
                                       campaign.EndDate.HasValue &&
                                       campaign.StartDate <= now && 
                                       now <= campaign.EndDate;
                if (!isCampaignRunning)
                    return BadRequest(new { message = "Campaign is not running" });

                var prizes = await _context.StoreCampaignPrizes
                    .Include(scp => scp.Prize)
                    .Where(scp => scp.StoreId == storeId 
                        && scp.Prize != null 
                        && scp.Prize.CampaignId == campaignId 
                        && scp.Prize.IsActive == true)
                    .ToListAsync();

                // Gom nhóm các prize giống nhau theo tên và tính tổng quantity
                var groupedPrizes = prizes
                    .GroupBy(scp => scp.Prize!.Name)
                    .Select(g => new GroupedPrizeAdmin
                    {
                        Name = g.Key,
                        PrizeType = g.First().Prize!.PrizeType,
                        Quantity = g.First().Prize!.Quantity ?? 0, // Số lượng voucher/phần thưởng mỗi cái
                        PrizeQuantity = g.Count(), // Số lượng phần thưởng giống nhau có trong store
                        ProbabilityWeight = g.First().ProbabilityWeight ?? 0,
                        IsActive = g.First().Prize!.IsActive ?? false
                    })
                    .ToList();

                return Ok(groupedPrizes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách giải thưởng", error = ex.Message });
            }
        }



        // Cập nhật ProbabilityWeight cho tất cả prize cùng tên trong 1 store/campaign
        [HttpPost("admin/changeprobability")]
        public async Task<IActionResult> ChangeProbabilityWeight([FromBody] ProbabilityChangeDto probabilityChangeDto)
        {
            try
            {
                // Validate input
                if (probabilityChangeDto == null)
                    return BadRequest(new { message = "Dữ liệu không hợp lệ" });


                if (probabilityChangeDto.NewProbabilityWeight <= 0)
                    return BadRequest(new { message = "NewProbabilityWeight phải lớn hơn 0" });

                // Validate store
                var store = await _context.Stores.FindAsync(probabilityChangeDto.StoreId);
                if (store == null)
                    return NotFound(new { message = "không tìm thấy store" });

                // Validate campaign
                var campaign = await _context.Campaigns.FindAsync(probabilityChangeDto.CampaignId);
                if (campaign == null)
                    return NotFound(new { message = "không tìm thấy campaign" });

                // Tìm tất cả Prize với tên này trong kho hang của store
                var prizes = await _context.Prizes
                    .Where(p => p.Name == probabilityChangeDto.PrizeName && p.CampaignId == probabilityChangeDto.CampaignId && p.StoreId == probabilityChangeDto.StoreId)
                    .ToListAsync();

                if (!prizes.Any())
                    return NotFound(new { message = "không tìm thấy prize với tên này trong kho hàng" });


                // Lấy ID của tất cả Prize tìm được
                var prizeIds = prizes.Select(p => p.Id).ToList();

                // Cập nhật tất cả StoreCampaignPrize liên quan trong store này
                var storeCampaignPrizes = await _context.StoreCampaignPrizes
                    .Where(scp => scp.StoreId == probabilityChangeDto.StoreId && prizeIds.Contains(scp.PrizeId!))
                    .ToListAsync();

                if (!storeCampaignPrizes.Any())
                    return NotFound(new { message = "không tìm thấy StoreCampaignPrizes" });

                // Cập nhật ProbabilityWeight cho tất cả record
                foreach (var scp in storeCampaignPrizes)
                {
                    scp.ProbabilityWeight = probabilityChangeDto.NewProbabilityWeight;
                    _context.StoreCampaignPrizes.Update(scp);
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = $"Cập nhật thành công {storeCampaignPrizes.Count} phần thưởng", updatedCount = storeCampaignPrizes.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi update ProbabilityWeight");
                return StatusCode(500, new { message = "Lỗi hệ thống", error = ex.Message });
            }
        }



        [HttpPost("admin/addcampaigntostore/{storeId}/{campaignId}")]
        public async Task<ActionResult<AddCampaignToStoreResultDto>> AddStoreCampaign(string storeId, string campaignId)
        {
            try
            {
                var store = await _context.Stores.FindAsync(storeId);
                if (store == null)
                    return NotFound(new { message = "không tìm thấy store" });

                var campaign = await _context.Campaigns.Include(c => c.Prizes).FirstOrDefaultAsync(c => c.Id == campaignId);
                if (campaign == null)
                    return NotFound(new { message = "không tìm thấy campaign" });

                // Check if campaign already exists for this store
                var campaignStores = await _context.CampaignStores
                    .Where(cs => cs.StoreId == storeId && cs.CampaignId == campaignId)
                    .ToListAsync();

                if (campaignStores.Any())
                    return BadRequest(new { message = "Campaign đã tồn tại cho cửa hàng này" });

                var campaintostore = new CampaignStore // tao 1 record trong bang CampaignStore de luu thong tin store va campaign
                {
                    Id = Guid.NewGuid().ToString(),
                    StoreId = storeId,
                    CampaignId = campaignId
                };

                _context.CampaignStores.Add(campaintostore);
                await _context.SaveChangesAsync();

                return Ok(campaintostore);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm campaign");
                return StatusCode(500, new { message = "Lỗi hệ thống khi thêm campaign", error = ex.Message });
            }
        }

        [HttpDelete("admin/removecampaignfromstore/{storeId}/{campaignId}")]
        public async Task<IActionResult> RemoveStoreCampaign(string storeId, string campaignId)
        {
            try
            {
                var store = await _context.Stores.FindAsync(storeId);
                if (store == null)
                    return NotFound(new { message = "không tìm thấy store" });

                var campaign = await _context.Campaigns.FindAsync(campaignId);
                if (campaign == null)
                    return NotFound(new { message = "không tìm thấy campaign" });

                var campaignStores = await _context.CampaignStores
                    .Where(cs => cs.StoreId == storeId && cs.CampaignId == campaignId)
                    .ToListAsync();

                if (!campaignStores.Any())
                    return NotFound(new { message = "Campaign không tồn tại cho cửa hàng này" });


                /////////////////// xóa các phần thưởng được cấu hình trong StoreCampaignPrize
                var removedCount = await _storeService.RemoveStoreCampaignAsync(storeId, campaignId);


                return Ok(new { message = $"Đã xóa {removedCount} giải thưởng khỏi campaign", removedCount = removedCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa campaign khỏi store");
                return StatusCode(500, new { message = "Lỗi hệ thống khi xóa campaign", error = ex.Message });
            }
        }



        [HttpGet("admin/export-excel")]
        public async Task<IActionResult> ExportToExcel()
        {
            // 1. Lấy trực tiếp danh sách Store từ Database qua _context
            var stores = await _context.Stores.AsNoTracking().ToListAsync();

            // 2. Khởi tạo Workbook của ClosedXML
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Danh sách cửa hàng");

                // 3. Tạo Tiêu đề các cột (Header)
                worksheet.Cell(1, 1).Value = "ID Cửa Hàng";
                worksheet.Cell(1, 2).Value = "Vị Trí Cửa Hàng";

                var headerRange = worksheet.Range("A1:B1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F4E78");
                headerRange.Style.Font.FontColor = XLColor.White;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // 4. Điền dữ liệu từ DB vào các dòng
                int currentRow = 2;
                foreach (var store in stores)
                {
                    worksheet.Cell(currentRow, 1).Value = store.Id;
                    worksheet.Cell(currentRow, 2).Value = store.StoreLocate;

                    // Định dạng dữ liệu (Căn giữa cột ID cho gọn)
                    worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    currentRow++;
                }

                // 5. Tự động căn rộng cột theo độ dài chữ
                worksheet.Columns().AdjustToContents();

                // 6. Ghi dữ liệu vào Stream để trả về file
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    string fileName = "Danh_Sach_Cua_Hang.xlsx";

                    return File(content, contentType, fileName);
                }
            }
        }


    }
}
